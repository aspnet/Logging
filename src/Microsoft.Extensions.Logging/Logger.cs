// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging.Abstractions.Internal;

namespace Microsoft.Extensions.Logging
{
    internal class Logger : ILogger, IMetricLogger
    {
        private Dictionary<string, Metric> _metrics = new Dictionary<string, Metric>();
        private object _sync = new object();

        public LoggerInformation[] Loggers { get; set; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var loggers = Loggers;
            if (loggers == null)
            {
                return;
            }

            List<Exception> exceptions = null;
            foreach (var loggerInfo in loggers)
            {
                if (!loggerInfo.IsEnabled(logLevel))
                {
                    continue;
                }

                try
                {
                    loggerInfo.Logger.Log(logLevel, eventId, state, exception, formatter);
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
            var loggers = Loggers;
            if (loggers == null)
            {
                return false;
            }

            List<Exception> exceptions = null;
            foreach (var loggerInfo in loggers)
            {
                if (!loggerInfo.IsEnabled(logLevel))
                {
                    continue;
                }

                try
                {
                    if (loggerInfo.Logger.IsEnabled(logLevel))
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
            var loggers = Loggers;

            if (loggers == null)
            {
                return NullScope.Instance;
            }

            if (loggers.Length == 1)
            {
                return loggers[0].Logger.BeginScope(state);
            }

            var scope = new Scope(loggers.Length);
            List<Exception> exceptions = null;
            for (var index = 0; index < loggers.Length; index++)
            {
                try
                {
                    var disposable = loggers[index].Logger.BeginScope(state);
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

        public IMetric DefineMetric(string name)
        {
            if (!_metrics.TryGetValue(name, out var metric))
            {
                lock (_sync)
                {
                    if (!_metrics.TryGetValue(name, out metric))
                    {
                        metric = new Metric(this, name);
                        _metrics[name] = metric;
                    }
                }
            }
            return metric;
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
        }

        private class Metric : IMetric
        {
            private readonly string _name;

            private Logger _logger;
            private IMetric[] _metrics;
            private object _sync = new object();

            public Metric(Logger logger, string name)
            {
                _logger = logger;
                _name = name;
            }

            public void RecordValue(double value)
            {
                IMetric[] metrics;
                lock (_sync)
                {
                    if (_metrics == null || _metrics.Length != _logger.Loggers.Length)
                    {
                        UpdateMetrics(_logger.Loggers);
                    }
                    metrics = _metrics;
                }

                for (var i = 0; i < metrics.Length; i += 1)
                {
                    // REVIEW: LogLevel for metrics?
                    // REVIEW: Filtering by Metric Name?
                    if (_logger.Loggers[i].IsEnabled(LogLevel.Critical))
                    {
                        metrics[i].RecordValue(value);
                    }
                }
            }

            public void RecordValue<T>(double value, T properties) where T : IEnumerable<KeyValuePair<string, object>>
            {
                IMetric[] metrics;
                lock (_sync)
                {
                    if (_metrics == null || _metrics.Length != _logger.Loggers.Length)
                    {
                        UpdateMetrics(_logger.Loggers);
                    }
                    metrics = _metrics;
                }

                for (var i = 0; i < metrics.Length; i += 1)
                {
                    if (_logger.Loggers[i].MetricsEnabled)
                    {
                        metrics[i].RecordValue(value, properties);
                    }
                }
            }

            private void UpdateMetrics(LoggerInformation[] loggers)
            {
                lock (_sync)
                {
                    if (_metrics == null)
                    {
                        _metrics = new IMetric[loggers.Length];
                    }
                    else if (_metrics.Length != loggers.Length)
                    {
                        Array.Resize(ref _metrics, loggers.Length);
                    }

                    for (var i = 0; i < loggers.Length; i += 1)
                    {
                        if (_metrics[i] == null)
                        {
                            _metrics[i] = loggers[i].Logger.DefineMetric(_name);
                        }
                    }
                }
            }
        }
    }
}
