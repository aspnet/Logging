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
        private readonly Dictionary<string, Logger> _loggers = new Dictionary<string, Logger>(StringComparer.Ordinal);
        private readonly ILogSinkProvider _logSinkProvider;
        private readonly object _sync = new object();

        public LoggerFactory()
            : this(logSinkProvider: null)
        {
        }

        public LoggerFactory(ILogSinkProvider logSinkProvider)
        {
            _logSinkProvider = logSinkProvider ?? new LogSinkProvider();
        }

        public ILogger CreateLogger(string categoryName)
        {
            Logger logger;
            lock (_sync)
            {
                if (!_loggers.TryGetValue(categoryName, out logger))
                {
                    logger = new Logger(_logSinkProvider, categoryName);
                    _loggers[categoryName] = logger;
                }
            }
            return logger;
        }

        public void AddSink(ILogSink provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            _logSinkProvider.AddSink(provider);
        }

        public void SetFilter(ILogFilter filter)
        {
            _logSinkProvider.Filter = filter;
        }

        public void Dispose()
        {
            _logSinkProvider?.Dispose();
        }
    }
}