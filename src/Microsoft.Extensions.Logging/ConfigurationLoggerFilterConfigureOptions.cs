// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging
{
    public class ConfigurationLoggerFilterConfigureOptions : IConfigureOptions<LoggerFilterOptions>
    {
        private readonly IConfiguration _configuration;
        private readonly bool _replace;

        public ConfigurationLoggerFilterConfigureOptions(IConfiguration configuration, bool replace)
        {
            _configuration = configuration;
            _replace = replace;
        }

        public void Configure(LoggerFilterOptions options)
        {
            LoadDefaultConfigValues(options);
        }

        private void LoadDefaultConfigValues(LoggerFilterOptions options)
        {
            if (_configuration == null)
            {
                return;
            }

            foreach (var configurationSection in _configuration.GetChildren())
            {
                if (configurationSection.Key == "LogLevel")
                {
                    // Load global category defaults
                    LoadRules(options, configurationSection, null);
                }
                else
                {
                    var logLevelSection = configurationSection.GetSection("LogLevel");
                    if (logLevelSection != null)
                    {
                        // Load logger specific rules
                        var logger = ExpandLoggerAlias(configurationSection.Key);
                        LoadRules(options, logLevelSection, logger);
                    }
                }
            }
        }

        private void LoadRules(LoggerFilterOptions options, IConfigurationSection configurationSection, string logger)
        {
            foreach (var section in configurationSection.AsEnumerable(true))
            {
                if (TryGetSwitch(section.Value, out var level))
                {
                    var newRule = new LoggerFilterRule(logger, section.Key, level, null);
                    options.Rules.Add(newRule);

                    // If we are in replace mode we need to find other matching rules and remove them
                    if (_replace)
                    {
                        foreach (var rule in LoggerRuleSelector.GetMatchingRules(options, logger, section.Key))
                        {
                            if (rule != newRule)
                            {
                                options.Rules.Remove(rule);
                            }
                        }
                    }
                }
            }
        }

        public string ExpandLoggerAlias(string name)
        {
            switch (name)
            {
                case "Console":
                    name = "Microsoft.Extensions.Logging.ConsoleLoggerProvider";
                    break;
                case "Debug":
                    name = "Microsoft.Extensions.Logging.DebugLoggerProvider";
                    break;
                case "AzureAppServices":
                    name = "Microsoft.Extensions.Logging.AzureAppServices.Internal.AzureAppServicesDiagnosticsLoggerProvider";
                    break;
                case "EventLog":
                    name = "Microsoft.Extensions.Logging.EventLog.EventLogLoggerProvider";
                    break;
                case "TraceSource":
                    name = "Microsoft.Extensions.Logging.TraceSource.TraceSourceLoggerProvider";
                    break;
                case "EventSource":
                    name = "Microsoft.Extensions.Logging.EventSource.EventSourceLoggerProvider";
                    break;
            }

            return name;
        }

        private static bool TryGetSwitch(string value, out LogLevel level)
        {
            if (string.IsNullOrEmpty(value))
            {
                level = LogLevel.None;
                return false;
            }
            else if (Enum.TryParse(value, true, out level))
            {
                return true;
            }
            else
            {
                throw new InvalidOperationException($"Configuration value '{value}' is not supported.");
            }
        }
    }
}