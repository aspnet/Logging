// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Microsoft.Extensions.Logging
{
    public class LoggerFactory : ILoggerFactory, IDisposable
    {
        public LoggerFactory() { }

        public LogLevel MinimumLevel { get; set; } = LogLevel.Verbose;

        public virtual ILogger CreateLogger(string name)
        {
            Logger ret;
            if (!_cache.TryGetValue(name, out ret))
            {
                ret = new Logger(name, MinimumLevel);

                if (_loggerCreated != null)
                    _loggerCreated(ret);

                _cache.Add(name, ret);
            }

            return ret;
        }

        public virtual void Dispose()
        {
            var factoryDispose = FactoryDispose;
            if (factoryDispose != null)
            {
                factoryDispose();
                FactoryDispose = null;
            }
        }

        public IDisposable Subscribe(IObserver<KeyValuePair<string, object>> target, Func<string, LogLevel, bool> filter = null)
        {
            IDisposable asDisposable = target as IDisposable;
            if (asDisposable != null)
                FactoryDispose += asDisposable.Dispose;

            return new SubscriptionList(this, target, filter);
        }
        
        internal virtual event Action<Logger> LoggerCreated
        {
            add
            {
                foreach (Logger logger in _cache.Values)
                    value(logger);
                _loggerCreated = (Action<Logger>)Delegate.Combine(_loggerCreated, value);
            }
            remove
            {
                _loggerCreated = (Action<Logger>)Delegate.Remove(_loggerCreated, value);
            }
        }
        internal virtual event Action FactoryDispose;

        #region private

        Dictionary<string, Logger> _cache = new Dictionary<string, Logger>();
        Action<Logger> _loggerCreated;

        /// <summary>
        /// TODO This does not belong here it is just an example of how to use it. 
        /// </summary>
        class SubscriptionList : IDisposable
        {
            // Can have state that the the rest of the stuff uses.  

            public SubscriptionList(LoggerFactory factory, IObserver<KeyValuePair<string, object>> target, Func<string, LogLevel, bool> filter)
            {
                _factory = factory;
                _target = target;
                _filter = filter;
                _loggerSubscriptions = new List<IDisposable>();
                _factory.LoggerCreated += OnLoggerCreated;
            }

            public void Dispose()
            {
                foreach (var subscription in _loggerSubscriptions)
                    subscription.Dispose();
                _loggerSubscriptions.Clear();

                // Unsubscribe from the factory.  
                _factory.LoggerCreated -= OnLoggerCreated;
            }

            #region private
            private void OnLoggerCreated(Logger newLogger)
            {
                _loggerSubscriptions.Add(newLogger.Subscribe(_target, _filter));
            }

            List<IDisposable> _loggerSubscriptions;
            LoggerFactory _factory;
            Func<string, LogLevel, bool> _filter;
            IObserver<KeyValuePair<string, object>> _target;
            #endregion
        }
        #endregion 
    }
}