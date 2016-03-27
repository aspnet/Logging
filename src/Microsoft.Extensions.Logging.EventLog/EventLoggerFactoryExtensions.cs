// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging.EventLog;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Extension methods for the <see cref="LoggerFactory"/> class.
    /// </summary>
    public static class EventLoggerFactoryExtensions
    {
        /// <summary>
        /// Adds an event logger that is enabled for <see cref="LogLevel"/>.Information or higher.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        public static LoggerFactory AddEventLog(this LoggerFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return AddEventLog(factory, new EventLogSettings());
        }

        /// <summary>
        /// Adds an event logger. Use <paramref name="settings"/> to enable logging for specific <see cref="LogLevel"/>s.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="settings">The <see cref="EventLogSettings"/>.</param>
        public static LoggerFactory AddEventLog(
            this LoggerFactory factory,
            EventLogSettings settings)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            factory.AddSink(new EventLogSink(settings));
            return factory;
        }
    }
}
