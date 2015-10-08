// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Tracing;
using Microsoft.Framework.Logging.Observer;

namespace Microsoft.Framework.Logging
{
    /// <summary>
    /// Summary description for LoggerFactory
    /// </summary>
    public class LoggerFactory : ILoggerFactory, IObservable<KeyValuePair<string, object>>, IDisposable
    {
        private readonly Dictionary<string, Logger> _loggers = new Dictionary<string, Logger>(StringComparer.Ordinal);
        private ILoggerProvider[] _providers = new ILoggerProvider[0];
        private readonly object _sync = new object();
        private bool _disposed = false;
        private readonly Dictionary<string, System.Diagnostics.Tracing.Logger> _systemLoggers = new Dictionary<string, System.Diagnostics.Tracing.Logger>(StringComparer.Ordinal);
        private List<IObserver<KeyValuePair<string, object>>> _observers = new List<IObserver<KeyValuePair<string, object>>>();

        public System.Diagnostics.Tracing.Logger CreateSystemLogger(string categoryName)
        {
            System.Diagnostics.Tracing.Logger logger;
            lock (_sync)
            {
                if (!_systemLoggers.TryGetValue(categoryName, out logger))
                {
                    logger = new System.Diagnostics.Tracing.Logger(categoryName);
                    _systemLoggers[categoryName] = logger;
                    _loggers[categoryName] = new Logger(this, categoryName);

                    // pipe messages from systemLogger to logger
                    logger.Subscribe(new LoggerObserver(_loggers[categoryName]), System.Diagnostics.Tracing.LogLevel.Verbose);

                    // subscribe all existing observers to the new logger
                    foreach (var observer in _observers)
                        //TODO IDisposable is lost
                        logger.Subscribe(observer);
                }
            }
            return logger;
        }

        public IDisposable Subscribe(IObserver<KeyValuePair<string, object>> observer)
        {
            List<IDisposable> subsribers = new List<IDisposable>();
            lock (_sync)
            {
                _observers.Add(observer);
                foreach (var logger in _systemLoggers.Values)
                    subsribers.Add(logger.Subscribe(observer, System.Diagnostics.Tracing.LogLevel.Verbose));
            }
            return new DisposeAll(subsribers);
        }

        public ILogger CreateLogger(string categoryName)
        {
            Logger logger;
            lock (_sync)
            {
                if (!_loggers.TryGetValue(categoryName, out logger))
                {
                    logger = new Logger(this, categoryName);
                    _loggers[categoryName] = logger;
                }
            }
            return logger;
        }

        public LogLevel MinimumLevel { get; set; } = LogLevel.Verbose;

        public void AddProvider(ILoggerProvider provider)
        {
            lock (_sync)
            {
                _providers = _providers.Concat(new[] { provider }).ToArray();
                foreach (var logger in _loggers)
                {
                    logger.Value.AddProvider(provider);
                }
            }
        }

        internal ILoggerProvider[] GetProviders()
        {
            return _providers;
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

                if (_systemLoggers != null)
                {
                    foreach (var logger in _systemLoggers.Values)
                        logger.Dispose();
                    _systemLoggers.Clear();
                }

                _disposed = true;
            }
        }

        private class DisposeAll : IDisposable
        {
            public DisposeAll(List<IDisposable> toDispose)
            {
                _toDispose = toDispose;

            }

            public void Dispose()
            {
                var toDispose = _toDispose;
                if (toDispose != null)
                {
                    _toDispose = null;
                    foreach (var subscription in toDispose)
                        subscription.Dispose();
                }
            }
            List<IDisposable> _toDispose;
        }
    }
}