using System;

namespace Microsoft.Extensions.Logging
{
    public class LoggerFilterRule
    {
        public LoggerFilterRule(string loggerType, string categoryName, LogLevel? logLevel, LogMessageFilter filter)
        {
            LoggerType = loggerType;
            CategoryName = categoryName;
            LogLevel = logLevel;
            Filter = filter;
        }

        public string LoggerType { get; }

        public string CategoryName { get; }

        public LogLevel? LogLevel { get; }

        public LogMessageFilter Filter { get; }
    }
}