// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if NET45 || DNX451

using Microsoft.Framework.Internal;

namespace Microsoft.Framework.Notify.Internal
{
    public class ProxyBase<T> : ProxyBase where T : class
    {
        protected T instance;

        public ProxyBase([NotNull] T inst)
            : base(typeof(T))
        {
            this.instance = inst;
        }

        public T UnderlyingInstance
        {
            get
            {
                return instance;
            }
        }

        public override object UnderlyingInstanceAsObject
        {
            get
            {
                return instance;
            }
        }
    }
}

#endif
