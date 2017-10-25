// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Logging
{
    public class LoggerExternalScopeProvider : IExternalScopeProvider
    {
        private readonly AsyncLocal<Scope> _currentScope = new AsyncLocal<Scope>();

        public void CollectScope<T>(Action<object, T> callback, T state)
        {
            var curent = _currentScope.Value;
            while (curent != null)
            {
                callback(curent.State, state);
                curent = curent.Parent;
            }
        }

        public IDisposable Push(object state)
        {
            var parent = _currentScope.Value;
            var newScope = new Scope(this, state, parent);
            _currentScope.Value = newScope;

            return newScope;
        }

        private class Scope: IDisposable
        {
            private readonly LoggerExternalScopeProvider _provider;

            internal Scope(LoggerExternalScopeProvider provider, object state, Scope parent)
            {
                _provider = provider;
                State = state;
                Parent = parent;
            }

            public Scope Parent { get; }

            public object State { get; }

            public override string ToString()
            {
                return State?.ToString();
            }

            public void Dispose()
            {
                _provider._currentScope.Value = Parent;
            }
        }
    }
}