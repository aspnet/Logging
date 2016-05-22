// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace Microsoft.Extensions.Logging.Abstractions
{
    public abstract class ConfigurableLoggerProvider<TLogger> : ILoggerProvider
        where TLogger : IConfigurableLogger
    {
        private readonly ConcurrentDictionary<string, TLogger> _loggers = new ConcurrentDictionary<string, TLogger>();
        private readonly Func<string, LogLevel, bool> _filter;
        private IConfigurableLoggerSettings _settings;

        public ConfigurableLoggerProvider(Func<string, LogLevel, bool> filter, bool includeScopes)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            _filter = filter;
            _settings = new ConfigurableLoggerSettings(new ConfigurationBuilder()
                .Add(new MemoryConfigurationSource
                {
                    InitialData = new Dictionary<string, string>
                    {
                        ["IncludeScopes"] = "false"
                    }
                })
                .Build());
        }

        public ConfigurableLoggerProvider(IConfigurableLoggerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _settings = settings;
            _settings.ChangeToken?.RegisterChangeCallback(OnConfigurationReload, null);
        }

        private void OnConfigurationReload(object state)
        {
            if (_settings == null)
            {
                return;
            }

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

        public ILogger CreateLogger(string name)
        {
            return _loggers.GetOrAdd(name, (key) => CreateLoggerImplementation(key, GetFilter(key, _settings), _settings?.IncludeScopes ?? false));
        }

        protected abstract TLogger CreateLoggerImplementation(string name, Func<string, LogLevel, bool> filter, bool includeScopes);

        private Func<string, LogLevel, bool> GetFilter(string name, IConfigurableLoggerSettings settings)
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

        public virtual void Dispose()
        {
        }
    }
}
