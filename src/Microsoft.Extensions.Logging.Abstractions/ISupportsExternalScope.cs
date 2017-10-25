// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// An interface for configuring logging providers.
    /// </summary>
    public interface ISupportsExternalScope
    {
        void SetScopeProvider(IExternalScopeProvider scopeProvider);
    }

    public interface IExternalScopeProvider
    {
        void CollectScope<T>(Action<object, T> callback, T state);
        IDisposable Push(object state);
    }
}