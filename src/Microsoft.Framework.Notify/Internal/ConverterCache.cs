// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if NET45 || DNX451

using System;
using System.Collections.Concurrent;

namespace Microsoft.Framework.Notify.Internal
{
    public class ConverterCache : ConcurrentDictionary<Tuple<Type, Type>, CacheResult>
    {
    }
}
#endif
