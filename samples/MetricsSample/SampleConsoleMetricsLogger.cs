using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace MetricsSample
{
    internal class SampleConsoleMetricsLogger : ILogger, IMetricLogger
    {
        private string _categoryName;
        private readonly SampleConsoleMetricsLoggerProvider _loggerProvider;

        public SampleConsoleMetricsLogger(string categoryName, SampleConsoleMetricsLoggerProvider loggerProvider)
        {
            _categoryName = categoryName;
            _loggerProvider = loggerProvider;
        }

        public IMetric DefineMetric(string name)
        {
            // Use the logger-provider's root list of metrics.
            return _loggerProvider.DefineMetric(_categoryName, name);
        }

        // No-ops for non-metrics.
        public IDisposable BeginScope<TState>(TState state)
        {
            // REVIEW: Is this allowed? It works because Logger handles it, but...
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }
    }
}
