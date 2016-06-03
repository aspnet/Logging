// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.Logging
{
    internal class Logger : ILogger
    {
        private static readonly NullScope _nullScope = new NullScope();

        private readonly LoggerFactory _loggerFactory;
        private readonly string _name;
        private ILogger[] _loggers;
        private Dictionary<ILoggerProvider, ILogger> _providerLoggerMap;

        public Logger(LoggerFactory loggerFactory, string name)
        {
            _loggerFactory = loggerFactory;
            _name = name;

            var providers = loggerFactory.GetProviders();

            if (providers.Length > 0)
            {
                _providerLoggerMap = new Dictionary<ILoggerProvider, ILogger>(providers.Length);
                _loggers = new ILogger[providers.Length];
                for (var index = 0; index != providers.Length; index++)
                {
                    var provider = providers[index];
                    var logger = provider.CreateLogger(name);
                    _loggers[index] = logger;
                    _providerLoggerMap[provider] = logger;
                }
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (_loggers == null)
            {
                return;
            }

            List<Exception> exceptions = null;
            foreach (var logger in _loggers)
            {
                try
                {
                    logger.Log(logLevel, eventId, state, exception, formatter);
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
            if (_loggers == null)
            {
                return false;
            }

            List<Exception> exceptions = null;
            foreach (var logger in _loggers)
            {
                try
                {
                    if (logger.IsEnabled(logLevel))
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

        public IDisposable BeginScope<TState>(TState state)
        {
            if (_loggers == null)
            {
                return _nullScope;
            }

            if (_loggers.Length == 1)
            {
                return _loggers[0].BeginScope(state);
            }

            var loggers = _loggers;

            var scope = new Scope(loggers.Length);
            List<Exception> exceptions = null;
            for (var index = 0; index < loggers.Length; index++)
            {
                try
                {
                    var disposable = loggers[index].BeginScope(state);
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

        internal void AddProvider(ILoggerProvider provider)
        {
            if (_providerLoggerMap == null) _providerLoggerMap = new Dictionary<ILoggerProvider, ILogger>(1);
            var logger = provider.CreateLogger(_name);
            _providerLoggerMap[provider] = logger;
            int logIndex;
            if (_loggers == null)
            {
                logIndex = 0;
                _loggers = new ILogger[1];
            }
            else
            {
                logIndex = _loggers.Length;
                Array.Resize(ref _loggers, logIndex + 1);
            }
            _loggers[logIndex] = logger;
        }
        internal void RemoveProvider(ILoggerProvider provider)
        {
            if (_providerLoggerMap != null)
            {
                var logger = _providerLoggerMap[provider];
                RemoveLogger(logger);
            }
        }

        private void RemoveLogger(ILogger logger)
        {
            _loggers = _loggers.Where(x => x != logger).ToArray();
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