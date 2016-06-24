// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.Extensions.Logging.Console
{
    public class ConsoleLoggerProvider : ConfigurableLoggerProvider<ConsoleLogger>
    {
        public ConsoleLoggerProvider(Func<string, LogLevel, bool> filter, bool includeScopes)
            : base(filter, includeScopes)
        {
        }

        public ConsoleLoggerProvider(IConfigurableLoggerSettings settings)
            : base(settings)
        {
        }

        protected override ConsoleLogger CreateLoggerImplementation(string name, Func<string, LogLevel, bool> filter, bool includeScopes)
        {
            return new ConsoleLogger(name, filter, includeScopes);
        }
    }
}
