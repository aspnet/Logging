// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.EventLog;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Extension methods for the <see cref="ILoggerFactory"/> class.
    /// </summary>
    public static class EventLoggerFactoryExtensions
    {
        /// <summary>
        /// Adds an event logger that is enabled for <see cref="LogLevel"/>.Information or higher.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        public static ILoggerFactory AddEventLog(this ILoggerFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return AddEventLog(factory, LogLevel.Information);
        }

        /// <summary>
        /// Adds an event logger that is enabled for <see cref="LogLevel"/>.Information or higher.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="includeScopes">A value which indicates whether log scope information should be displayed
        /// in the output.</param>
        public static ILoggerFactory AddEventLog(this ILoggerFactory factory, bool includeScopes)
        {
            factory.AddEventLog((n, l) => l >= LogLevel.Information, includeScopes);
            return factory;
        }

        /// <summary>
        /// Adds an event logger that is enabled as defined by the filter function.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="filter"></param>
        public static ILoggerFactory AddEventLog(
            this ILoggerFactory factory,
            Func<string, LogLevel, bool> filter)
        {
            factory.AddEventLog(filter, includeScopes: false);
            return factory;
        }

        /// <summary>
        /// Adds an event logger that is enabled as defined by the filter function.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="filter"></param>
        /// <param name="includeScopes">A value which indicates whether log scope information should be displayed
        /// in the output.</param>
        public static ILoggerFactory AddEventLog(
            this ILoggerFactory factory,
            Func<string, LogLevel, bool> filter,
            bool includeScopes)
        {
            factory.AddProvider(new EventLogLoggerProvider(filter, includeScopes));
            return factory;
        }

        /// <summary>
        /// Adds an event logger that is enabled for <see cref="LogLevel"/>s of minLevel or higher.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="minLevel">The minimum <see cref="LogLevel"/> to be logged</param>
        public static ILoggerFactory AddEventLog(this ILoggerFactory factory, LogLevel minLevel)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return AddEventLog(factory, (_, logLevel) => logLevel >= minLevel);
        }

        /// <summary>
        /// Adds an event logger. Use <paramref name="loggerSettings"/> to enable logging for specific <see cref="LogLevel"/>s.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="loggerSettings">The <see cref="EventLogSettings"/>.</param>
        public static ILoggerFactory AddEventLog(
            this ILoggerFactory factory,
            IConfigurableLoggerSettings loggerSettings)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (loggerSettings == null)
            {
                throw new ArgumentNullException(nameof(loggerSettings));
            }

            factory.AddProvider(new EventLogLoggerProvider(loggerSettings));
            return factory;
        }

        /// <summary>
        /// Adds an event logger. Use <paramref name="loggerSettings"/> to enable logging for specific <see cref="LogLevel"/>s.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="loggerSettings">The <see cref="IConfigurableLoggerSettings"/>.</param>
        /// <param name="eventLogSettings">The <see cref="EventLogSettings"/>.</param>
        public static ILoggerFactory AddEventLog(
            this ILoggerFactory factory,
            IConfigurableLoggerSettings loggerSettings,
            EventLogSettings eventLogSettings)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (loggerSettings == null)
            {
                throw new ArgumentNullException(nameof(loggerSettings));
            }

            if (eventLogSettings == null)
            {
                throw new ArgumentNullException(nameof(eventLogSettings));
            }

            factory.AddProvider(new EventLogLoggerProvider(loggerSettings));
            return factory;
        }

        public static ILoggerFactory AddEventLog(this ILoggerFactory factory, IConfiguration configuration)
        {
            var settings = new ConfigurableLoggerSettings(configuration);
            return factory.AddEventLog(settings);
        }

        public static ILoggerFactory AddEventLog(this ILoggerFactory factory, IConfiguration configuration, EventLogSettings eventLogSettings)
        {
            var settings = new ConfigurableLoggerSettings(configuration);
            return factory.AddEventLog(settings, eventLogSettings);
        }
    }
}
