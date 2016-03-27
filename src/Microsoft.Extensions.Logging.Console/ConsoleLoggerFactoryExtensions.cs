// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Logging
{
    public static class ConsoleLoggerExtensions
    {
        /// <summary>
        /// Adds a console sink.
        /// </summary>
        public static LoggerFactory AddConsole(this LoggerFactory factory)
        {
            return factory.AddConsole(includeScopes: false);
        }

        /// <summary>
        /// Adds a console sink.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="includeScopes">A value which indicates whether log scope information should be displayed
        /// in the output.</param>
        public static LoggerFactory AddConsole(this LoggerFactory factory, bool includeScopes)
        {
            factory.AddConsole(new ConsoleLoggerSettings() { IncludeScopes = includeScopes });
            return factory;
        }

        public static LoggerFactory AddConsole(this LoggerFactory factory, IConfiguration configuration)
        {
            var settings = new ConfigurationConsoleLoggerSettings(configuration);
            return factory.AddConsole(settings);
        }

        public static LoggerFactory AddConsole(
            this LoggerFactory factory,
            IConsoleLoggerSettings settings)
        {
            factory.AddSink(new ConsoleSink(settings));
            return factory;
        }
    }
}