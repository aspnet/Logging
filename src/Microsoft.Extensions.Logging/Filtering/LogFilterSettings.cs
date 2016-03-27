// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Logging.Filtering
{
    /// <summary>
    /// In-memory settings for the <see cref="LogFilter"/> class.
    /// </summary>
    public class LogFilterSettings : ILogFilterSettings
    {
        /// <summary>
        /// Gets or sets filters that are used, when no sink-specific filters exist.
        /// </summary>
        public IDictionary<string, LogLevel> DefaultSwitches { get; set; }

        /// <summary>
        /// Gets or sets filters for specific sinks.
        /// </summary>
        public IDictionary<Type, IDictionary<string, LogLevel>> SinkSwitches { get; set; }

        public LogFilterSettings()
        {
            DefaultSwitches = new Dictionary<string, LogLevel>();
            SinkSwitches = new Dictionary<Type, IDictionary<string, LogLevel>>();
        }

        /// <summary>
        /// Adds the given <paramref name="logLevel"/> as the default level for sinks 
        /// that don't have specific switches.
        /// </summary>
        public void AddDefaultSwitch(LogLevel logLevel)
        {
            AddDefaultSwitch(LogFilter.DefaultCategory, logLevel);
        }

        public void AddDefaultSwitch(string categoryName, LogLevel logLevel)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentNullException(nameof(categoryName));
            }

            EnsureSwitches();

            DefaultSwitches.Add(categoryName, logLevel);
        }

        /// <summary>
        /// Adds the given <paramref name="logLevel"/> as the default level for the given <paramref name="sinkType"/>.
        /// </summary>
        public void AddSinkSwitch(Type sinkType, LogLevel logLevel)
        {
            AddSinkSwitch(sinkType, LogFilter.DefaultCategory, logLevel);
        }

        public void AddSinkSwitch(Type sinkType, string categoryName, LogLevel logLevel)
        {
            if (sinkType == null)
            {
                throw new ArgumentNullException(nameof(sinkType));
            }
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentNullException(nameof(categoryName));
            }

            EnsureSwitches();

            IDictionary<string, LogLevel> switches;

            if (!SinkSwitches.TryGetValue(sinkType, out switches))
            {
                switches = new Dictionary<string, LogLevel>();
                SinkSwitches.Add(sinkType, switches);
            }
            
            switches.Add(categoryName, logLevel);
        }

        public bool TryGetSwitch(Type sinkType, string categoryName, out LogLevel logLevel)
        {
            if (sinkType == null)
            {
                throw new ArgumentNullException(nameof(sinkType));
            }
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentNullException(nameof(categoryName));
            }

            EnsureSwitches();

            IDictionary<string, LogLevel> switches;
            if (SinkSwitches.TryGetValue(sinkType, out switches))
            {
                return switches.TryGetValue(categoryName, out logLevel);
            }

            return DefaultSwitches.TryGetValue(categoryName, out logLevel);
        }

        private void EnsureSwitches()
        {
            if (DefaultSwitches == null)
            {
                DefaultSwitches = new Dictionary<string, LogLevel>();
            }

            if (SinkSwitches == null)
            {
                SinkSwitches = new Dictionary<Type, IDictionary<string, LogLevel>>();
            }
        }
    }
}

