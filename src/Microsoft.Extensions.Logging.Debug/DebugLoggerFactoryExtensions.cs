// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Debug;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Extension methods for the <see cref="ILoggerFactory"/> class.
    /// </summary>
    public static class DebugLoggerFactoryExtensions
    {
        /// <summary>
        /// Adds a debug logger that is enabled for <see cref="LogLevel"/>.Information or higher.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        public static ILoggerFactory AddDebug(this ILoggerFactory factory)
        {
            return AddDebug(factory, LogLevel.Information);
        }

        /// <summary>
        /// Adds a debug logger that is enabled as defined by the filter function.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="includeScopes">A value which indicates whether log scope information should be displayed
        /// in the output.</param>
        public static ILoggerFactory AddDebug(this ILoggerFactory factory, bool includeScopes)
        {
            factory.AddDebug((n, l) => l >= LogLevel.Information, false);
            return factory;
        }

        /// <summary>
        /// Adds a debug logger that is enabled as defined by the filter function.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="filter">The function used to filter events based on the log level.</param>
        /// <param name="includeScopes">A value which indicates whether log scope information should be displayed
        /// in the output.</param>
        public static ILoggerFactory AddDebug(this ILoggerFactory factory, Func<string, LogLevel, bool> filter, bool includeScopes)
        {
            factory.AddProvider(new DebugLoggerProvider(filter, false));
            return factory;
        }

        /// <summary>
        /// Adds a debug logger that is enabled for <see cref="LogLevel"/>s of minLevel or higher.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="minLevel">The minimum <see cref="LogLevel"/> to be logged</param>
        public static ILoggerFactory AddDebug(this ILoggerFactory factory, LogLevel minLevel)
        {
            return factory.AddDebug((_, logLevel) => logLevel >= minLevel, false);
        }

        /// <summary>
        /// Adds a debug logger that is enabled for <see cref="LogLevel"/>s of minLevel or higher.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="minLevel">The minimum <see cref="LogLevel"/> to be logged</param>
        /// <param name="includeScopes">A value which indicates whether log scope information should be displayed
        /// in the output.</param>
        public static ILoggerFactory AddDebug(this ILoggerFactory factory, LogLevel minLevel, bool includeScopes)
        {
            return factory.AddDebug((_, logLevel) => logLevel >= minLevel, includeScopes);
        }

        public static ILoggerFactory AddDebug(
            this ILoggerFactory factory,
            IConfigurableLoggerSettings settings)
        {
            factory.AddProvider(new DebugLoggerProvider(settings));
            return factory;
        }

        public static ILoggerFactory AddDebug(this ILoggerFactory factory, IConfiguration configuration)
        {
            var settings = new ConfigurableLoggerSettings(configuration);
            return factory.AddDebug(settings);
        }
    }
}