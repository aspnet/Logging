// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Logging.EventLog
{
    /// <summary>
    /// The provider for the <see cref="EventLogLogger"/>.
    /// </summary>
    public class EventLogLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogLoggerProvider"/> class.
        /// </summary>
        public EventLogLoggerProvider()
            : this(settings: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogLoggerProvider"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="EventLogSettings"/>.</param>
        public EventLogLoggerProvider(EventLogSettings settings)
        {
            Settings = settings;
        }

        // to enable unit testing
        internal EventLogSettings Settings { get; }

        /// <inheritdoc />
        public ILogger CreateLogger(string name)
        {
            return new EventLogLogger(name, Settings ?? new EventLogSettings());
        }

        public void Dispose()
        {
        }
    }
}
