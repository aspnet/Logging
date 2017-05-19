using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging
{
    public class LoggerFactory : ILoggerFactory
    {
        private static readonly LoggerRuleSelector RuleSelector = new LoggerRuleSelector();

        private readonly Dictionary<string, Logger> _loggers = new Dictionary<string, Logger>(StringComparer.Ordinal);

        private readonly List<ILoggerProvider> _providers;
        private readonly object _sync = new object();
        private volatile bool _disposed;
        private IDisposable _changeTokenRegistration;
        private LoggerFilterOptions _filterOptions;

        public LoggerFactory() : this(new LoggerFilterOptions())
        {
        }

        public LoggerFactory(LoggerFilterOptions filterOptions) : this(Enumerable.Empty<ILoggerProvider>(), new StaticFilterOptionsMonitor(filterOptions))
        {
        }

        public LoggerFactory(IEnumerable<ILoggerProvider> providers, IOptionsMonitor<LoggerFilterOptions> filterOption)
        {
            _providers = providers.ToList();
            _changeTokenRegistration = filterOption.OnChange(RefreshFilters);
            RefreshFilters(filterOption.CurrentValue);
        }

        private void RefreshFilters(LoggerFilterOptions filterOptions)
        {
            lock (_sync)
            {
                _filterOptions = filterOptions;
                foreach (var logger in _loggers)
                {
                    var loggerInformation = logger.Value.Loggers;
                    var categoryName = logger.Key;

                    ApplyRules(loggerInformation, categoryName, 0, loggerInformation.Length);
                }
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            lock (_sync)
            {
                Logger logger;

                if (!_loggers.TryGetValue(categoryName, out logger))
                {

                    logger = new Logger()
                    {
                        Loggers = CreateLoggers(categoryName)
                    };
                    _loggers[categoryName] = logger;
                }

                return logger;
            }
        }

        void ILoggerFactory.AddProvider(ILoggerProvider provider)
        {
            lock (_sync)
            {
                _providers.Add(provider);
                foreach (var logger in _loggers)
                {
                    var loggerInformation = logger.Value.Loggers;
                    var categoryName = logger.Key;

                    Array.Resize(ref loggerInformation, loggerInformation.Length + 1);
                    var newLoggerIndex = loggerInformation.Length - 1;
                    loggerInformation[newLoggerIndex].Logger = provider.CreateLogger(categoryName);

                    ApplyRules(loggerInformation, categoryName, newLoggerIndex, 1);
                }
            }
        }

        private Logger.LoggerInformation[] CreateLoggers(string categoryName)
        {
            Logger.LoggerInformation[] loggers = new Logger.LoggerInformation[_providers.Count];
            for (int i = 0; i < _providers.Count; i++)
            {
                loggers[i].Logger = _providers[i].CreateLogger(categoryName);
            }

            ApplyRules(loggers, categoryName, 0, loggers.Length);
            return loggers;
        }

        private void ApplyRules(Logger.LoggerInformation[] loggers, string categoryName, int start, int count)
        {
            for (var index = start; index < start + count; index++)
            {
                ref var loggerInformation = ref loggers[index];

                RuleSelector.Select(_filterOptions,
                    loggerInformation.Logger.GetType().FullName,
                    categoryName,
                    out var minLevel,
                    out var filter);

                loggerInformation.MinLevel = minLevel;
                loggerInformation.Filter = filter;
            }
        }

        /// <summary>
        /// Check if the factory has been disposed.
        /// </summary>
        /// <returns>True when <see cref="Dispose()"/> as been called</returns>
        protected virtual bool CheckDisposed() => _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                _changeTokenRegistration?.Dispose();

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
            }
        }
    }
}