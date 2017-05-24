// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up logging services in an <see cref="ILoggerBuilder" />.
    /// </summary>
    public static class LoggerBuilderExtensions
    {
        public static ILoggerBuilder AddProvider(this ILoggerBuilder builder, ILoggerProvider provider)
        {
            builder.Services.AddSingleton(provider);
            return builder;
        }
    }
}