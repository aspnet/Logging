// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.Logging.Abstractions.Internal
{
    public class TypeNameHelper
    {
        private static readonly Dictionary<Type, string> _builtInTypeNames = new Dictionary<Type, string>
            {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(object), "object" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(string), "string" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(ushort), "ushort" }
            };

        public static string GetTypeDisplayName(object item, bool fullName = true)
        {
            return item == null ? null : GetTypeDisplayName(item.GetType(), fullName);
        }

        public static string GetTypeDisplayName(Type type, bool fullName = true)
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                string name;
                if (fullName)
                {
                    name = type.GetGenericTypeDefinition().FullName;

                    if (type.IsNested)
                    {
                        name = name.Replace('+', '.');
                    }
                }
                else
                {
                    name = type.GetGenericTypeDefinition().Name;
                }

                var tildaIndex = name.IndexOf('`');
                if (tildaIndex >= 0)
                {
                    // Since '.' is typically used to filter log messages in a hierarchy kind of scenario,
                    // do not include any generic type information as part of the name.
                    // Example:
                    // Microsoft.AspNet.Mvc -> logl level set as Warning
                    // Microsoft.AspNet.Mvc.ModelBinding -> log level set as Verbose
                    return name.Substring(0, tildaIndex);
                }

                return name;
            }

            if (_builtInTypeNames.ContainsKey(type))
            {
                return _builtInTypeNames[type];
            }
            else
            {
                if (fullName)
                {
                    var name = type.FullName;

                    if (type.IsNested)
                    {
                        name = name.Replace('+', '.');
                    }

                    return name;
                }

                return type.Name;
            }
        }
    }
}