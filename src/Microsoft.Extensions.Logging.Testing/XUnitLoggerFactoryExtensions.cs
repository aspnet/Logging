// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging.Testing;
using Xunit.Abstractions;

namespace Microsoft.Extensions.Logging
{
    public static class XUnitLoggerFactoryExtensions
    {
        public static void AddXUnit(this ILoggerFactory loggerFactory, ITestOutputHelper output)
        {
            loggerFactory.AddProvider(new XUnitLoggerProvider(output));
        }

        public static void AddXUnit(this ILoggerFactory loggerFactory, ITestOutputHelper output, LogLevel minLevel)
        {
            loggerFactory.AddProvider(new XUnitLoggerProvider(output, minLevel));
        }
    }
}
