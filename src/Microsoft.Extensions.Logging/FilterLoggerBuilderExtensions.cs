// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up logging services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class FilterLoggerBuilderExtensions
    {
        public static ILoggerBuilder AddFilter(this ILoggerBuilder builder, string category, LogLevel level)
        {
            return AddRule(builder, new LoggerFilterRule(null, category, level, null));
        }

        public static ILoggerBuilder AddFilter<T>(this ILoggerBuilder builder, string category, LogLevel level)
        {
            return AddRule(builder, new LoggerFilterRule(typeof(T).FullName, category, level, null));
        }

        public static ILoggerBuilder AddFilter(this ILoggerBuilder builder, LogMessageFilter filter)
        {
            return AddRule(builder, new LoggerFilterRule(null, null, null, filter));
        }
        public static ILoggerBuilder AddFilter<T>(this ILoggerBuilder builder, LogMessageFilter filter)
        {
            return AddRule(builder, new LoggerFilterRule(typeof(T).FullName, null, null, filter));
        }

        public static ILoggerBuilder AddFilter(this ILoggerBuilder builder, Func<string, LogLevel, bool> categoryLevelFilter)
        {
            return AddRule(builder, new LoggerFilterRule(null, null, null, (type, name, level) => categoryLevelFilter(name, level)));
        }

        public static ILoggerBuilder AddFilter<T>(this ILoggerBuilder builder, Func<string, LogLevel, bool> categoryLevelFilter)
        {
            return AddRule(builder, new LoggerFilterRule(typeof(T).FullName, null, null, (type, name, level) => categoryLevelFilter(name, level)));
        }

        public static ILoggerBuilder AddFilter(this ILoggerBuilder builder, string category, Func<LogLevel, bool> levelFilter)
        {
            return AddRule(builder, new LoggerFilterRule(null, category, null, (type, name, level) => levelFilter(level)));
        }

        public static ILoggerBuilder AddFilter<T>(this ILoggerBuilder builder, string category, Func<LogLevel, bool> levelFilter)
        {
            return AddRule(builder, new LoggerFilterRule(typeof(T).FullName, category, null, (type, name, level) => levelFilter(level)));
        }

        private static ILoggerBuilder AddRule(ILoggerBuilder builder, LoggerFilterRule loggerFilterRule)
        {
            builder.Services.Configure<LoggerFilterOptions>(options => options.Rules.Add(loggerFilterRule));
            return builder;
        }
    }
}