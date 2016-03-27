// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Logging
{
    internal class Logger : ILogger
    {
        private static readonly NullScope _nullScope = new NullScope();

        private readonly ILogSinkProvider _logSinkProvider;
        private readonly string _name;

        public Logger(ILogSinkProvider logSinkProvider, string name)
        {
            _logSinkProvider = logSinkProvider;
            _name = name;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (_logSinkProvider.Sinks == null)
            {
                return;
            }

            List<Exception> exceptions = null;
            foreach (var sink in _logSinkProvider.Sinks)
            {
                try
                {
                    sink.Log(_name, logLevel, eventId, state, exception, formatter);
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

        public bool IsEnabled(LogLevel logLevel)
        {
            if (_logSinkProvider.Sinks == null)
            {
                return false;
            }

            List<Exception> exceptions = null;
            foreach (var sink in _logSinkProvider.Sinks)
            {
                try
                {
                    if (sink.IsEnabled(_name, logLevel))
                    {
                        return true;
                    }
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
                    message: "An error occurred while writing to logger(s).",
                    innerExceptions: exceptions);
            }

            return false;
        }

        public IDisposable BeginScopeImpl(object state)
        {
            if (_logSinkProvider.Sinks == null)
            {
                return _nullScope;
            }

            if (_logSinkProvider.Sinks.Length == 1)
            {
                return _logSinkProvider.Sinks[0].BeginScope(_name, state);
            }

            var sinks = _logSinkProvider.Sinks;

            var scope = new Scope(sinks.Length);
            List<Exception> exceptions = null;
            for (var index = 0; index < sinks.Length; index++)
            {
                try
                {
                    var disposable = sinks[index].BeginScope(_name, state);
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

            internal void Add(IDisposable disposable)
            {
                throw new NotImplementedException();
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