﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if NET45 || DNX451

using System;
using System.Reflection.Emit;

namespace Microsoft.Framework.Notify.Internal
{
    public sealed class CacheResult
    {
        public static CacheResult FromError(Tuple<Type, Type> key, string error)
        {
            return new CacheResult()
            {
                Key = key,
                Error = error,
            };
        }

        public static CacheResult FromTypeBuilder(
            Tuple<Type, Type> key,
            TypeBuilder typeBuilder,
            ConstructorBuilder constructorBuilder)
        {
            return new CacheResult()
            {
                Key = key,
                TypeBuilder = typeBuilder,
                ConstructorBuilder = constructorBuilder,
            };
        }

        public ConstructorBuilder ConstructorBuilder { get; private set; }

        public string Error { get; private set; }

        public bool IsError => Error != null;

        public Tuple<Type, Type> Key { get; private set; }

        public TypeBuilder TypeBuilder { get; private set; }
    }
}
#endif
