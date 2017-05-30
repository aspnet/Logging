// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up logging services in an <see cref="ILoggerBuilder" />.
    /// </summary>
    public static class LoggerBuilderExtensions
    {
        public static ILoggerBuilder AddConfiguration(this ILoggerBuilder builder, IConfiguration configuration)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<LoggerFilterOptions>>(new ConfigurationLoggerFilterConfigureOptions(configuration)));
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsChangeTokenSource<LoggerFilterOptions>>(new ConfigurationChangeTokenSource<LoggerFilterOptions>(configuration)));

            return builder;
        }

        public static ILoggerBuilder SetMinimumLevel(this ILoggerBuilder builder, LogLevel level)
        {
            builder.Services.Add(ServiceDescriptor.Singleton<IConfigureOptions<LoggerFilterOptions>>(
                new DefaultLoggerLevelConfigureOptions(level)));
            return builder;
        }

        public static ILoggerBuilder AddProvider(this ILoggerBuilder builder, ILoggerProvider provider)
        {
            builder.Services.AddSingleton(provider);
            return builder;
        }
    }
}