// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.Logging
{
    internal class LoggerObservable : IDisposable
    {
        private static readonly NullScope _nullScope = new NullScope();

        private ILoggerProvider[] _providers = new ILoggerProvider[0];
        private readonly object _sync = new object();
        private bool _disposed;

        public void AddProvider(ILoggerProvider provider)
        {
            lock (_sync)
            {
                _providers = _providers.Concat(new[] { provider }).ToArray();
            }
        }

        public bool IsEnabled(string categoryName, LogLevel logLevel)
        {
            if (_providers == null)
            {
                return false;
            }

            return true;
            //List<Exception> exceptions = null;
            //foreach (var provider in _providers)
            //{
            //    try
            //    {
            //        if (provider.IsEnabled(logLevel))
            //        {
            //            return true;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        if (exceptions == null)
            //        {
            //            exceptions = new List<Exception>();
            //        }

            //        exceptions.Add(ex);
            //    }
            //}

            //if (exceptions != null && exceptions.Count > 0)
            //{
            //    throw new AggregateException(
            //        message: "An error occurred while writing to provider(s).",
            //        innerExceptions: exceptions);
            //}

            //return false;
        }

        public void Log<TState>(string categoryName, LogLevel logLevel, EventId eventId, TState state, Exception exception, 
            Func<TState, Exception, string> formatter)
        {
            if (_providers == null || _providers.Length == 0)
            {
                return;
            }

            List<Exception> exceptions = null;
            foreach (var provider in _providers)
            {
                try
                {
                    provider.Log(categoryName, logLevel, eventId, state, exception, formatter);
                }
                catch (Exception ex)
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }

                    exceptions.Add(ex);
                }
            }

            if (exceptions != null && exceptions.Count > 0)
            {
                throw new AggregateException(
                    message: "An error occurred while writing to logger(s).", innerExceptions: exceptions);
            }
        }

        public IDisposable BeginScopeImpl(string categoryName, object state)
        {
            if (_providers == null || _providers.Length == 0)
            {
                return _nullScope;
            }

            if (_providers.Length == 1)
            {
                return _providers[0].BeginScopeImpl(categoryName, state);
            }

            var providers = _providers;

            var scope = new Scope(providers.Length);
            List<Exception> exceptions = null;
            for (var index = 0; index < providers.Length; index++)
            {
                try
                {
                    var disposable = providers[index].BeginScopeImpl(categoryName, state);
                    scope.SetDisposable(index, disposable);
                }
                catch (Exception ex)
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }

                    exceptions.Add(ex);
                }
            }

            if (exceptions != null && exceptions.Count > 0)
            {
                throw new AggregateException(
                    message: "An error occurred while writing to logger(s).", innerExceptions: exceptions);
            }

            return scope;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                foreach (var provider in _providers)
                {
                    try
                    {
                        provider.Dispose();
                    }
                    catch
                    {
                        // Swallow exceptions on dispose
                    }
                }

                _disposed = true;
            }
        }

        private class Scope : IDisposable
        {
            private bool _isDisposed;

            private IDisposable _disposable0;
            private IDisposable _disposable1;
            private readonly IDisposable[] _disposable;

            public Scope(int count)
            {
                if (count > 2)
                {
                    _disposable = new IDisposable[count - 2];
                }
            }

            public void SetDisposable(int index, IDisposable disposable)
            {
                if (index == 0)
                {
                    _disposable0 = disposable;
                }
                else if (index == 1)
                {
                    _disposable1 = disposable;
                }
                else
                {
                    _disposable[index - 2] = disposable;
                }
            }

            public void Dispose()
            {
                if (!_isDisposed)
                {
                    if (_disposable0 != null)
                    {
                        _disposable0.Dispose();
                    }
                    if (_disposable1 != null)
                    {
                        _disposable1.Dispose();
                    }
                    if (_disposable != null)
                    {
                        var count = _disposable.Length;
                        for (var index = 0; index != count; ++index)
                        {
                            if (_disposable[index] != null)
                            {
                                _disposable[index].Dispose();
                            }
                        }
                    }

                    _isDisposed = true;
                }
            }
        }

        private class NullScope : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
