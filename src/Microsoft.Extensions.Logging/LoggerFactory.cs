// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Summary description for LoggerFactory
    /// </summary>
    public class LoggerFactory : ILoggerFactory
    {
        private readonly LoggerObservable _observable;
        private readonly Dictionary<string, Logger> _loggers = new Dictionary<string, Logger>(StringComparer.Ordinal);
        private readonly object _sync = new object();

        public LoggerFactory()
        {
            _observable = new LoggerObservable();
        }

        public ILogger CreateLogger(string categoryName)
        {
            Logger logger;
            lock (_sync)
            {
                if (!_loggers.TryGetValue(categoryName, out logger))
                {
                    logger = new Logger(_observable, categoryName);
                    _loggers[categoryName] = logger;
                }
            }
            return logger;
        }

        public void AddProvider(ILoggerProvider provider)
        {
            _observable.AddProvider(provider);
        }

        public void Dispose()
        {
            _observable?.Dispose();
        }
    }
}