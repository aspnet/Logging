// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Logging.Console
{
    public class ConsoleLoggerSettings : IConsoleLoggerSettings
    {

        public bool IncludeScopes { get; set; }

        public IChangeMonitor<IConsoleLoggerSettings> Monitor { get; set; }

        public IDictionary<string, LogLevel> Switches { get; set; } = new Dictionary<string, LogLevel>();

        public bool TryGetSwitch(string name, out LogLevel level)
        {
            return Switches.TryGetValue(name, out level);
        }
    }
}
