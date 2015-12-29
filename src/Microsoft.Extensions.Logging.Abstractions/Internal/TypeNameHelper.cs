// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.Logging.Abstractions.Internal
{
    internal class TypeNameHelper
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
                }
                else
                {
                    name = type.GetGenericTypeDefinition().Name;
                }
                return name.Substring(0, name.IndexOf('`'));
            }
            if (_builtInTypeNames.ContainsKey(type))
            {
                return _builtInTypeNames[type];
            }
            else
            {
                return fullName ? type.FullName : type.Name;
            }
        }
    }
}