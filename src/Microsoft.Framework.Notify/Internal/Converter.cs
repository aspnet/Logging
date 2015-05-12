﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if NET45 || DNX451

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.Framework.Notify.Internal
{
    public class Converter
    {
        private static int _counter = 0;

        private static AssemblyBuilder AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("ProxyHolderAssembly"), AssemblyBuilderAccess.Run);
        private static ModuleBuilder ModuleBuilder = AssemblyBuilder.DefineDynamicModule("Main Module");

        public static object Convert(ConverterCache cache, Type outputType, Type inputType, object input)
        {
            if (input == null)
            {
                return null;
            }

            if (inputType == outputType)
            {
                return input;
            }

            if (outputType.IsAssignableFrom(inputType))
            {
                return input;
            }

            // If we get to this point, all of the trivial conversions have been tried. We don't attempt value
            // conversions such as int -> double. The only thing left is proxy generation, which requires that
            // the destination type be an interface.
            //
            // We should always end up with a proxy type or an exception
            var proxyType = GetProxyType(cache, outputType, inputType);
            Debug.Assert(proxyType != null);

            return Activator.CreateInstance(proxyType, input);
        }

        private static Type GetProxyType(ConverterCache cache, Type tout, Type tin)
        {
            var key = new Tuple<Type, Type>(tin, tout);

            CacheResult result;
            if (!cache.TryGetValue(key, out result))
            {
                var context = new ProxyBuilderContext(cache, tout, tin);

                // Check that all required types are proxy-able - this will create the TypeBuilder, Constructor,
                // and property mappings.
                //
                // We need to create the TypeBuilder and Constructor up front to deal with cycles that can occur
                // when generating the proxy properties.
                if (!VerifyProxySupport(context, context.Key))
                {
                    var error = cache[key];
                    Debug.Assert(error != null && error.IsError);
                    throw new InvalidOperationException(error.Error);
                }

                Debug.Assert(context.Visited.ContainsKey(context.Key));

                // Now that we've generated all of the constructors for the proxies, we can generate the rest
                // of the type.
                foreach (var verificationResult in context.Visited)
                {
                    AddProperties(
                        context,
                        verificationResult.Value.TypeBuilder,
                        verificationResult.Value.Mappings);

                    verificationResult.Value.TypeBuilder.CreateType();
                }

                // We only want to publish the results after all of the proxies are totally generated.
                foreach (var verificationResult in context.Visited)
                {
                    cache[verificationResult.Key] = CacheResult.FromTypeBuilder(
                        verificationResult.Key, 
                        verificationResult.Value.TypeBuilder,
                        verificationResult.Value.ConstructorBuilder);
                }

                return context.Visited[context.Key].TypeBuilder.CreateType();
            }
            else if (result.IsError)
            {
                throw new InvalidOperationException(result.Error);
            }
            else if (result.TypeBuilder == null)
            {
                // This is an identity convertion
                return null;
            }
            else
            {
                return result.TypeBuilder.CreateType();
            }
        }

        private static bool VerifyProxySupport(ProxyBuilderContext context, Tuple<Type, Type> key)
        {
            var sourceType = key.Item1;
            var targetType = key.Item2;

            if (context.Visited.ContainsKey(key))
            {
                // We've already seen this combination and so far so good.
                return true;
            }

            CacheResult cacheResult;
            if (context.Cache.TryGetValue(key, out cacheResult))
            {
                // If we get here we've got a published conversion or error, so we can stop searching.
                return !cacheResult.IsError;
            }

            if (targetType == sourceType || targetType.IsAssignableFrom(sourceType))
            {
                // If we find a trivial conversion, then that will work. 
                return true;
            }

            if (!targetType.IsInterface)
            {
                var message = Resources.FormatConverter_TypeMustBeInterface(targetType.FullName, sourceType.FullName);
                context.Cache[key] = CacheResult.FromError(key, message);

                return false;
            }

            // This is a combination we haven't seen before, and it *might* support proxy generation, so let's 
            // start trying.
            var verificationResult = new VerificationResult();
            context.Visited.Add(key, verificationResult);

            var propertyMappings = new List<KeyValuePair<PropertyInfo, PropertyInfo>>();

            var sourceProperties = sourceType.GetRuntimeProperties();
            foreach (var targetProperty in targetType.GetRuntimeProperties())
            {
                if (!targetProperty.CanRead)
                {
                    var message = Resources.FormatConverter_PropertyMustHaveGetter(
                        targetProperty.Name,
                        targetType.FullName);
                    context.Cache[key] = CacheResult.FromError(key, message);

                    return false;
                }

                if (targetProperty.CanWrite)
                {
                    var message = Resources.FormatConverter_PropertyMustNotHaveSetter(
                        targetProperty.Name,
                        targetType.FullName);
                    context.Cache[key] = CacheResult.FromError(key, message);

                    return false;
                }

                if (targetProperty.GetIndexParameters()?.Length > 0)
                {
                    var message = Resources.FormatConverter_PropertyMustNotHaveIndexParameters(
                        targetProperty.Name,
                        targetType.FullName);
                    context.Cache[key] = CacheResult.FromError(key, message);

                    return false;
                }

                // To allow for flexible versioning, we want to allow missing properties in the source. 
                //
                // For now we'll just store null, and later generate a stub getter that returns default(T).
                var sourceProperty = sourceProperties.Where(p => p.Name == targetProperty.Name).FirstOrDefault();
                if (sourceProperty != null)
                {
                    var propertyKey = new Tuple<Type, Type>(sourceProperty.PropertyType, targetProperty.PropertyType);
                    if (!VerifyProxySupport(context, propertyKey))
                    {
                        // There's an error here, so bubble it up and cache it.
                        var error = context.Cache[propertyKey];
                        Debug.Assert(error != null && error.IsError);

                        context.Cache[key] = CacheResult.FromError(key, error.Error);
                        return false;
                    }
                }

                propertyMappings.Add(new KeyValuePair<PropertyInfo, PropertyInfo>(targetProperty, sourceProperty));
            }

            verificationResult.Mappings = propertyMappings;

            var baseType = typeof(ProxyBase<>).MakeGenericType(sourceType);
            var typeBuilder = ModuleBuilder.DefineType(
                "ProxyType" + _counter++ + " wrapping:" + sourceType.Name + " to look like:" + targetType.Name,
                TypeAttributes.Class,
                baseType,
                new Type[] { targetType });

            var constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new Type[] { sourceType });

            var il = constructorBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, sourceType);
            il.Emit(OpCodes.Call, baseType.GetConstructor(new Type[] { sourceType }));
            il.Emit(OpCodes.Ret);

            verificationResult.ConstructorBuilder = constructorBuilder;
            verificationResult.TypeBuilder = typeBuilder;

            return true;
        }

        private static void AddProperties(
            ProxyBuilderContext context,
            TypeBuilder typeBuilder,
            IEnumerable<KeyValuePair<PropertyInfo, PropertyInfo>> properties)
        {
            foreach (var property in properties)
            {
                var targetProperty = property.Key;
                var sourceProperty = property.Value;

                var propertyBuilder = typeBuilder.DefineProperty(
                    targetProperty.Name,
                    PropertyAttributes.None,
                    property.Key.PropertyType,
                    Type.EmptyTypes);

                var methodBuilder = typeBuilder.DefineMethod(
                    targetProperty.GetMethod.Name,
                    targetProperty.GetMethod.Attributes & ~MethodAttributes.Abstract,
                    targetProperty.GetMethod.CallingConvention,
                    targetProperty.GetMethod.ReturnType,
                    Type.EmptyTypes);
                propertyBuilder.SetGetMethod(methodBuilder);
                typeBuilder.DefineMethodOverride(methodBuilder, targetProperty.GetMethod);

                var il = methodBuilder.GetILGenerator();
                if (sourceProperty == null)
                {
                    // Return a default(T) value.
                    il.Emit(OpCodes.Initobj, targetProperty.PropertyType);
                    il.Emit(OpCodes.Ret);
                }
                else
                {
                    // Push 'this' and get the underlying instance.
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, typeBuilder.BaseType.GetField("instance", BindingFlags.NonPublic | BindingFlags.Instance));

                    // Call the source property.
                    il.EmitCall(OpCodes.Callvirt, sourceProperty.GetMethod, null);

                    // Create a proxy for the value returned by source property (if necessary).
                    EmitProxy(context, il, targetProperty.PropertyType, sourceProperty.PropertyType);
                    il.Emit(OpCodes.Ret);
                }
            }
        }

        private static void EmitProxy(ProxyBuilderContext context, ILGenerator il, Type targetType, Type sourceType)
        {
            if (sourceType == targetType)
            {
                // Do nothing.
                return;
            }
            else if (targetType.IsAssignableFrom(sourceType))
            {
                il.Emit(OpCodes.Castclass, targetType);
                return;
            }

            // If we get here, then we actually need a proxy.
            var key = new Tuple<Type, Type>(sourceType, targetType);

            ConstructorBuilder constructorBuilder = null;
            CacheResult cacheResult;
            VerificationResult verificationResult;
            if (context.Cache.TryGetValue(key, out cacheResult))
            {
                Debug.Assert(!cacheResult.IsError);
                Debug.Assert(cacheResult.ConstructorBuilder != null);

                // This means we've got a fully-built (published) type.
                constructorBuilder = cacheResult.ConstructorBuilder;
            }
            else if (context.Visited.TryGetValue(key, out verificationResult))
            {
                Debug.Assert(verificationResult.ConstructorBuilder != null);
                constructorBuilder = verificationResult.ConstructorBuilder;
            }

            Debug.Assert(constructorBuilder != null);

            var endLabel = il.DefineLabel();
            var createProxyLabel = il.DefineLabel();

            // If the 'source' value is null, then just return it.
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Brfalse_S, endLabel);

            // If the 'source' value isn't a proxy then we need to create one.
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Isinst, typeof(ProxyBase));
            il.Emit(OpCodes.Brfalse_S, createProxyLabel);

            // If the 'source' value is-a proxy then get the wrapped value.
            il.Emit(OpCodes.Isinst, typeof(ProxyBase));
            il.EmitCall(OpCodes.Callvirt, typeof(ProxyBase).GetMethod("get_UnderlyingInstanceAsObject"), null);
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Isinst, targetType);
            il.Emit(OpCodes.Brtrue_S, endLabel);

            il.MarkLabel(createProxyLabel);

            // Create the proxy.
            il.Emit(OpCodes.Newobj, constructorBuilder);

            il.MarkLabel(endLabel);
        }

        private class ProxyBuilderContext
        {
            public ProxyBuilderContext(ConverterCache cache, Type targetType, Type sourceType)
            {
                Cache = cache;

                Key = new Tuple<Type, Type>(sourceType, targetType);
                Visited = new Dictionary<Tuple<Type, Type>, VerificationResult>();
            }

            public ConverterCache Cache { get; }

            public Tuple<Type, Type> Key { get; }

            public Type SourceType => Key.Item1;

            public Type TargetType => Key.Item2;

            public Dictionary<Tuple<Type, Type>, VerificationResult> Visited { get; }
        }

        private class VerificationResult
        {
            public ConstructorBuilder ConstructorBuilder { get; set; }

            public IEnumerable<KeyValuePair<PropertyInfo, PropertyInfo>> Mappings { get; set; }

            public TypeBuilder TypeBuilder { get; set; }
        }
    }
}
#endif
