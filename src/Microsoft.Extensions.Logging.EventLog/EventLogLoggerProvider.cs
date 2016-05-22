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
        private readonly EventLogSettings _eventLogSettings;

        public EventLogLoggerProvider(Func<string, LogLevel, bool> filter, bool includeScopes)
            : base(filter, includeScopes)
        {
            _eventLogSettings = new EventLogSettings();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogLoggerProvider"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="IConfigurableLoggerSettings"/>.</param>
        public EventLogLoggerProvider(IConfigurableLoggerSettings settings)
            : base(settings)
        {
            _eventLogSettings = new EventLogSettings();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogLoggerProvider"/> class.
        /// </summary>
        /// <param name="loggerSettings">The <see cref="IConfigurableLoggerSettings"/>.</param>
        /// <param name="eventLogSettings">The <see cref="EventLogSettings"/>.</param>
        public EventLogLoggerProvider(IConfigurableLoggerSettings loggerSettings, EventLogSettings eventLogSettings)
            : base(loggerSettings)
        {
            if (eventLogSettings == null)
            {
                throw new ArgumentNullException(nameof(eventLogSettings));
            }
        }

        /// <inheritdoc />
        protected override EventLogLogger CreateLoggerImplementation(string name, Func<string, LogLevel, bool> filter, bool includeScopes)
        {
            return new EventLogLogger(name,
                filter: filter,
                includeScopes: includeScopes,
                eventLogSettings: _eventLogSettings);
        }
    }
}
