﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.Logging.Console;

namespace Microsoft.Framework.Logging
{
    public static class ConsoleLoggerExtensions
    {
        /// <summary>
        /// Adds a console logger that is enabled for <see cref="LogLevel"/>.Information or higher.
        /// </summary>
        public static ILoggerFactory AddConsole(this ILoggerFactory factory)
        {
            factory.AddProvider(new ConsoleLoggerProvider((category, logLevel) => logLevel >= LogLevel.Information));
            return factory;
        }

        /// <summary>
        /// Adds a console logger that is enabled as defined by the filter function.
        /// </summary>
        public static ILoggerFactory AddConsole(this ILoggerFactory factory, Func<string, LogLevel, bool> filter)
        {
            factory.AddProvider(new ConsoleLoggerProvider(filter));
            return factory;
        }

        /// <summary>
        /// Adds a console logger that is enabled for <see cref="LogLevel"/>s of minLevel or higher.
        /// </summary>
        /// <param name="minLevel">The minimum <see cref="LogLevel"/> to be logged</param>
        public static ILoggerFactory AddConsole(this ILoggerFactory factory, LogLevel minLevel)
        {
            factory.AddProvider(new ConsoleLoggerProvider((category, logLevel) => logLevel >= minLevel));
            return factory;
        }

        /// <summary>
        ///     Adds a console logger that is enabled as defined by the IConfiguration object
        /// </summary>
        /// <param name="configuration">IConfiguration object with filter attributes</param>
        /// <returns></returns>
        public static ILoggerFactory AddConsole(this ILoggerFactory factory, IConfiguration configuration)
        {
            factory.AddProvider(new ConsoleLoggerProvider((category, logLevel) =>
            {
                LogLevel minLevel;

                return Enum.TryParse(configuration?.GetSubKey("ConsoleLogger")?.Get(category), out minLevel) &&
                       logLevel >= minLevel;
            }));

            return factory;
        }
    }
}