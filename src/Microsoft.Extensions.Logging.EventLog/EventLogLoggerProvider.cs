// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.Extensions.Logging.EventLog
{
    /// <summary>
    /// The provider for the <see cref="EventLogLogger"/>.
    /// </summary>
    public class EventLogLoggerProvider : ConfigurableLoggerProvider<EventLogLogger>
    {
        private readonly string _logName;
        private readonly string _sourceName;
        private readonly string _machineName;

        public EventLogLoggerProvider(Func<string, LogLevel, bool> filter, bool includeScopes)
            : base(filter, includeScopes)
        {
            _logName = "Application";
            _sourceName = "Application";
            _machineName = ".";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogLoggerProvider"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="EventLogSettings"/>.</param>
        public EventLogLoggerProvider(EventLogSettings settings)
            : base(settings)
        {
            _logName = settings.LogName ?? "Application";
            _sourceName = settings.SourceName ?? "Application";
            _machineName = settings.MachineName ?? ".";
        }

        /// <inheritdoc />
        protected override EventLogLogger CreateLoggerImplementation(string name, Func<string, LogLevel, bool> filter, bool includeScopes)
        {
            return new EventLogLogger(name,
                logName: _logName,
                sourceName: _sourceName,
                machineName: _machineName,
                filter: filter,
                includeScopes: includeScopes);
        }
    }
}
