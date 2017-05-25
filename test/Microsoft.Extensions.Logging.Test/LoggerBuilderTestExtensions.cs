// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Logging.Test
{
    public static class TestLoggerBuilder
    {
        public static ILoggerBuilder Create(IConfiguration configuration = null)
        {
            var builder = new ServiceCollection().AddLogging();
            // Most test setup their own filtering or for all events to pass through
            builder.Services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Trace);
            if (configuration != null)
            {
                builder.AddConfiguration(configuration);
            }
            return builder;
        }
    }

    public static class LoggerBuilderTestExtensions
    {
        public static ILoggerFactory Build(this ILoggerBuilder builder)
        {
            return builder.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        }
    }
}
