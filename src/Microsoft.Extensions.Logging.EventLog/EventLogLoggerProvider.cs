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
        private readonly EventLogSettings _settings;

        public EventLogLoggerProvider(Func<string, LogLevel, bool> filter, bool includeScopes)
            : base(filter, includeScopes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogLoggerProvider"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="EventLogSettings"/>.</param>
        public EventLogLoggerProvider(EventLogSettings settings)
            : base(settings)
        {
            _settings = settings;
        }

        /// <inheritdoc />
        protected override EventLogLogger CreateLoggerImplementation(string name, Func<string, LogLevel, bool> filter, bool includeScopes)
        {
            return new EventLogLogger(name,
                logName: _settings?.LogName ?? "Application",
                sourceName: _settings?.SourceName ?? "Application",
                machineName: _settings?.MachineName ?? ".",
                filter: filter,
                includeScopes: includeScopes);
        }
    }
}
