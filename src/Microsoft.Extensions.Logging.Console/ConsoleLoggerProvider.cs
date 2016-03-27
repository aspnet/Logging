// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Extensions.Logging.Console
{
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ConsoleLogger> _loggers = new ConcurrentDictionary<string, ConsoleLogger>();

        private readonly Func<string, LogLevel, bool> _filter;
        private IConsoleLoggerSettings _settings;

        public ConsoleLoggerProvider(Func<string, LogLevel, bool> filter, bool includeScopes)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            _filter = filter;
            _settings = new ConsoleLoggerSettings()
            {
                IncludeScopes = includeScopes,
            };
        }

        public ConsoleLoggerProvider(IConsoleLoggerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _settings = settings;

            if (_settings.ChangeToken != null)
            {
                _settings.ChangeToken.RegisterChangeCallback(OnConfigurationReload, null);
            }
        }

        private void OnConfigurationReload(object state)
        {
            // The settings object needs to change here, because the old one is probably holding on
            // to an old change token.
            _settings = _settings.Reload();

            foreach (var logger in _loggers.Values)
            {
                logger.Filter = GetFilter(logger.Name, _settings);
                logger.IncludeScopes = _settings.IncludeScopes;
            }

            // The token will change each time it reloads, so we need to register again.
            if (_settings?.ChangeToken != null)
            {
                _settings.ChangeToken.RegisterChangeCallback(OnConfigurationReload, null);
            }
        }

        public void Log<TState>(
            string categoryName, 
            LogLevel logLevel, 
            EventId eventId, 
            TState state, 
            Exception exception, 
            Func<TState, Exception, string> formatter)
        {
            var logger = _loggers.GetOrAdd(categoryName, CreateLoggerImplementation);
            logger.Log(logLevel, eventId, state, exception, formatter);
        }

        public bool IsEnabled(string categoryName, LogLevel logLevel)
        {
            var logger = _loggers.GetOrAdd(categoryName, CreateLoggerImplementation);
            return logger.IsEnabled(logLevel);
        }

        public IDisposable BeginScopeImpl(string categoryName, object state)
        {
            var logger = _loggers.GetOrAdd(categoryName, CreateLoggerImplementation);
            return logger.BeginScopeImpl(state);
        }

        private ConsoleLogger CreateLoggerImplementation(string name)
        {
            return new ConsoleLogger(name, GetFilter(name, _settings), _settings.IncludeScopes);
        }

        private Func<string, LogLevel, bool> GetFilter(string name, IConsoleLoggerSettings settings)
        {
            if (_filter != null)
            {
                return _filter;
            }

            if (settings != null)
            {
                foreach (var prefix in GetKeyPrefixes(name))
                {
                    LogLevel level;
                    if (settings.TryGetSwitch(prefix, out level))
                    {
                        return (n, l) => l >= level;
                    }
                }
            }

            return (n, l) => false;
        }

        private IEnumerable<string> GetKeyPrefixes(string name)
        {
            while (!string.IsNullOrEmpty(name))
            {
                yield return name;
                var lastIndexOfDot = name.LastIndexOf('.');
                if (lastIndexOfDot == -1)
                {
                    yield return "Default";
                    break;
                }
                name = name.Substring(0, lastIndexOfDot);
            }
        }

        public void Dispose()
        {
        }
    }
}
