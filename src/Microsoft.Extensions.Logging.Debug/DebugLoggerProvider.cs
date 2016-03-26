// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Logging.Debug
{
    /// <summary>
    /// The provider for the <see cref="DebugLogger"/>.
    /// </summary>
    public class DebugLoggerProvider : ILoggerProvider
    {
        private readonly DebugLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLoggerProvider"/> class.
        /// </summary>
        /// <param name="filter">The function used to filter events based on the log level.</param>
        public DebugLoggerProvider(Func<string, LogLevel, bool> filter)
        {
            _logger = new DebugLogger(filter);
        }

        public void Log<TState>(
            string categoryName,
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            _logger.Log(categoryName, logLevel, eventId, state, exception, formatter);
        }

        public bool IsEnabled(string categoryName, LogLevel logLevel)
        {
            return _logger.IsEnabled(categoryName, logLevel);
        }

        public IDisposable BeginScopeImpl(string categoryName, object state)
        {
            return _logger.BeginScopeImpl(state);
        }

        public void Dispose()
        {
        }
    }
}
