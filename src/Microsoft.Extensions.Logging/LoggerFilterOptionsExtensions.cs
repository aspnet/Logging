// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up logging services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class LoggerFilterOptionsExtensions
    {
        public static LoggerFilterOptions AddFilter(this LoggerFilterOptions options, string category, LogLevel level)
        {
            options.Rules.Add(new LoggerFilterRule(null, category, level, null));
            return options;
        }

        public static LoggerFilterOptions AddFilter<T>(this LoggerFilterOptions options, string category, LogLevel level)
        {
            options.Rules.Add(new LoggerFilterRule(typeof(T).FullName, category, level, null));
            return options;
        }

        public static LoggerFilterOptions AddFilter(this LoggerFilterOptions options, LogMessageFilter filter)
        {
            options.Rules.Add(new LoggerFilterRule(null, null, null, filter));
            return options;
        }
        public static LoggerFilterOptions AddFilter<T>(this LoggerFilterOptions options, LogMessageFilter filter)
        {
            options.Rules.Add(new LoggerFilterRule(typeof(T).FullName, null, null, filter));
            return options;
        }

        public static LoggerFilterOptions AddFilter(this LoggerFilterOptions options, Func<string, LogLevel, bool> categoryLevelFilter)
        {
            options.Rules.Add(new LoggerFilterRule(null, null, null, (type, name, level) => categoryLevelFilter(name, level)));
            return options;
        }

        public static LoggerFilterOptions AddFilter<T>(this LoggerFilterOptions options, Func<string, LogLevel, bool> categoryLevelFilter)
        {
            options.Rules.Add(new LoggerFilterRule(typeof(T).FullName, null, null, (type, name, level) => categoryLevelFilter(name, level)));
            return options;
        }
        public static LoggerFilterOptions AddFilter(this LoggerFilterOptions options, string category, Func<LogLevel, bool> levelFilter)
        {
            options.Rules.Add(new LoggerFilterRule(null, category, null, (type, name, level) => levelFilter(level)));
            return options;
        }

        public static LoggerFilterOptions AddFilter<T>(this LoggerFilterOptions options, string category, Func<LogLevel, bool> levelFilter)
        {
            options.Rules.Add(new LoggerFilterRule(typeof(T).FullName, category, null, (type, name, level) => levelFilter(level)));
            return options;
        }
    }
}