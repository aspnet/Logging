// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging.Testing;

namespace Microsoft.Extensions.Logging.Test
{
    // Just wraps a TestLogger but does so in a type that doesn't implement IMetricsLogger
    // Didn't put this in Microsoft.Extensions.Logging.Testing because it's not really necessary for general testing of logging,
    // just for testing the logging infrastructure itself.
    public class TestLoggerWithoutMetrics : ILogger
    {
        private readonly TestLogger _innerLogger;

        public TestLoggerWithoutMetrics(string name, ITestSink sink, bool enabled)
        {
            _innerLogger = new TestLogger(name, sink, enabled);
        }

        public TestLoggerWithoutMetrics(string name, ITestSink sink, Func<LogLevel, bool> filter)
        {
            _innerLogger = new TestLogger(name, sink, filter);
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return ((ILogger)_innerLogger).BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _innerLogger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            ((ILogger)_innerLogger).Log(logLevel, eventId, state, exception, formatter);
        }
    }
}
