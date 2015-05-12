// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if NET45 || DNX451

using System;

namespace Microsoft.Framework.Notify.Internal
{
    public abstract class ProxyBase
    {
        public readonly Type WrappedType;

        protected ProxyBase(Type wrappedType)
        {
            WrappedType = wrappedType;
        }

        public abstract object UnderlyingInstanceAsObject
        {
            get;
        }
    }
}

#endif
