// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Console;

namespace Microsoft.Extensions.Logging
{
    public static class ConsoleLoggerExtensions
    {
        /// <summary>
        /// Adds a console logger.
        /// </summary>
        public static LoggerFactory AddConsole(this LoggerFactory factory)
        {
            factory.AddProvider("Console", new ConsoleLoggerProvider(factory.Configuration));
            return factory;
        }

        /// <summary>
        /// Adds a console logger that is enabled for <see cref="LogLevel"/>.Information or higher.
        /// </summary>
        public static ILoggerFactory AddConsole(this ILoggerFactory factory)
        {
            return factory.AddConsole(includeScopes: false);
        }

        /// <summary>
        /// Adds a console logger that is enabled for <see cref="LogLevel"/>.Information or higher.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="includeScopes">A value which indicates whether log scope information should be displayed
        /// in the output.</param>
        public static ILoggerFactory AddConsole(this ILoggerFactory factory, bool includeScopes)
        {
            factory.AddConsole((n, l) => l >= LogLevel.Information, includeScopes);
            return factory;
        }

        /// <summary>
        /// Adds a console logger that is enabled for <see cref="LogLevel"/>s of minLevel or higher.
        /// </summary>
        /// <param name="factory">The <see cref="ILoggerFactory"/> to use.</param>
        /// <param name="minLevel">The minimum <see cref="LogLevel"/> to be logged</param>
        public static ILoggerFactory AddConsole(this ILoggerFactory factory, LogLevel minLevel)
        {
            factory.AddConsole(minLevel, includeScopes: false);
            return factory;
        }

        /// <summary>
        /// Adds a console logger that is enabled for <see cref="LogLevel"/>s of minLevel or higher.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="minLevel">The minimum <see cref="LogLevel"/> to be logged</param>
        /// <param name="includeScopes">A value which indicates whether log scope information should be displayed
        /// in the output.</param>
        public static ILoggerFactory AddConsole(
            this ILoggerFactory factory,
            LogLevel minLevel,
            bool includeScopes)
        {
            factory.AddConsole((category, logLevel) => logLevel >= minLevel, includeScopes);
            return factory;
        }

        /// <summary>
        /// Adds a console logger that is enabled as defined by the filter function.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="filter"></param>
        public static ILoggerFactory AddConsole(
            this ILoggerFactory factory,
            Func<string, LogLevel, bool> filter)
        {
            factory.AddConsole(filter, includeScopes: false);
            return factory;
        }

        /// <summary>
        /// Adds a console logger that is enabled as defined by the filter function.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="filter"></param>
        /// <param name="includeScopes">A value which indicates whether log scope information should be displayed
        /// in the output.</param>
        public static ILoggerFactory AddConsole(
            this ILoggerFactory factory,
            Func<string, LogLevel, bool> filter,
            bool includeScopes)
        {
            factory.AddProvider(new ConsoleLoggerProvider(filter, includeScopes));
            return factory;
        }

        /// <summary>
        /// Add a <see cref="ConsoleLoggerProvider"/> configured with the provided <see cref="IConsoleLoggerSettings"/>. 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="settings">The console logging settings to use.</param>
        /// <remarks>
        /// <para>
        /// When <see cref="IConsoleLoggerSettings"/> are defined, then only the categories 
        /// they return a result from <c>TryGetSwitch</c>, either for themselves or for a parent, 
        /// are logged.
        /// </para>
        /// <para>
        /// If a category is not configured then the value of the special category 'Default' (case sensitive) 
        /// is used, or 'Default' is not specified then they are not logged at all. 
        /// </para>
        /// </remarks>
        /// <example>
        /// Configures logging with a default level of <c>Warning</c>, and specific levels
        /// for particular namespaces (inherited by all child loggers) and classes.
        /// <code>
        /// var consoleSettings = new ConsoleLoggerSettings();
        /// consoleSettings.Switches = new Dictionary&lt;string, LogLevel&gt;() {
        ///     { "Default", LogLevel.Warning },
        ///     { "CompanyA.Namespace1.ClassB", LogLevel.Debug },
        ///     { "CompanyB", LogLevel.Information },
        /// };
        /// factory.AddConsole(consoleSettings);
        /// </code>
        /// </example>
        public static ILoggerFactory AddConsole(
            this ILoggerFactory factory,
            IConsoleLoggerSettings settings)
        {
            factory.AddProvider(new ConsoleLoggerProvider(settings));
            return factory;
        }

        /// <summary>
        /// Add a <see cref="ConsoleLoggerProvider"/> configured with <see cref="ConfigurationConsoleLoggerSettings"/> 
        /// taken from the provided <see cref="IConfiguration"/>, e.g. from a settings file. 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="configuration">The configuration to use to get the console logging settings from.</param>
        /// <remarks>
        /// <para>
        /// When <see cref="IConsoleLoggerSettings"/> are defined, then only the categories 
        /// they return a result from <c>TryGetSwitch</c>, either for themselves or for a parent, 
        /// are logged.
        /// </para>
        /// <para>
        /// If a category is not configured then the value of the special category 'Default' (case sensitive) 
        /// is used, or 'Default' is not specified then they are not logged at all. 
        /// </para>
        /// </remarks>
        public static ILoggerFactory AddConsole(this ILoggerFactory factory, IConfiguration configuration)
        {
            var settings = new ConfigurationConsoleLoggerSettings(configuration);
            return factory.AddConsole(settings);
        }
    }
}