// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration;
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

            return AddEventLog(factory, new EventLogSettings()
            {
                Filter = (_, logLevel) => logLevel >= minLevel
            });
        }

        /// <summary>
        /// Adds an event logger. Use <paramref name="settings"/> to enable logging for specific <see cref="LogLevel"/>s.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="settings">The <see cref="EventLogSettings"/>.</param>
        public static ILoggerFactory AddEventLog(
            this ILoggerFactory factory,
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

            factory.AddProvider(new EventLogLoggerProvider(settings));
            return factory;
        }

        public static ILoggerFactory AddEventLog(this ILoggerFactory factory, IConfiguration configuration)
        {
            var settings = new EventLogSettings(configuration);
            return factory.AddEventLog(settings);
        }
    }
}
