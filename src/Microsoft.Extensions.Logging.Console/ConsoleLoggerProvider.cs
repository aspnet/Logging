// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Logging.Console
{
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        public ConsoleLoggerProvider(Func<string, LogLevel, bool> filter)
        {
            Filter = filter;
        }

        // to enable unit testing
        internal Func<string, LogLevel, bool> Filter { get; }

        public ILogger CreateLogger(string name)
        {
            return new ConsoleLogger(name, Filter);
        }

        public void Dispose()
        {
        }
    }
}
