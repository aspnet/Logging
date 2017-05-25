// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Logging
{
    public class LoggerFilterRule
    {
        public LoggerFilterRule(string loggerType, string categoryName, LogLevel? logLevel, Func<string, string, LogLevel, bool> filter)
        {
            LoggerType = loggerType;
            CategoryName = categoryName;
            LogLevel = logLevel;
            Filter = filter;
        }

        public string LoggerType { get; }

        public string CategoryName { get; }

        public LogLevel? LogLevel { get; }

        public Func<string, string, LogLevel, bool> Filter { get; }
    }
}