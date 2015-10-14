// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Microsoft.Extensions.Logging
{
    public class LoggerFactory: IObservable<Logger>, ILoggerFactory, IDisposable
    {
        public LogLevel MaximumLevel { get; set; } = LogLevel.Debug;

        public virtual ILogger CreateLogger(string name)
        {
            Logger ret;
            lock(_sync)
            {
                if (!_cache.TryGetValue(name, out ret))
                {
                    ret = new Logger(name, MaximumLevel);

                    foreach (var observer in _observers)
                    {
                        observer.OnNext(ret);
                    }
                    _cache.Add(name, ret);
                }
            }
            return ret;
        }

        public IDisposable Subscribe(IObserver<Logger> observer)
        {
            lock(_sync)
            {
                foreach (var logger in _cache.Values)
                {
                    observer.OnNext(logger);
                }
                _observers.Add(observer);
            }
            return new Unsubscriber(observer, this);
        }

        public void Dispose()
        {
            lock(_sync)
            {
                foreach (var observer in _observers)
                {
                    if (observer is IDisposable)
                    {
                        (observer as IDisposable).Dispose();
                    }
                }
                _observers.RemoveAll(o => true);
            }
        }

        private Dictionary<string, Logger> _cache = new Dictionary<string, Logger>();
        private List<IObserver<Logger>> _observers = new List<IObserver<Logger>>();
        private readonly object _sync = new object();
        private class Unsubscriber: IDisposable
        {
            private IObserver<Logger> _observer;
            private LoggerFactory _factory;
            public Unsubscriber(IObserver<Logger> observer, LoggerFactory factory)
            {
                _observer = observer;
                _factory = factory;
            }

            public void Dispose()
            {
                lock(_factory._sync)
                {
                    _factory._observers.Remove(_observer);
                }
            }
        }
    }
}