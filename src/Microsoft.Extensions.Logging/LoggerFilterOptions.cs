using System.Collections.Generic;

namespace Microsoft.Extensions.Logging
{
    public class LoggerFilterOptions
    {
        public LogLevel MinLevel { get; set; } = LogLevel.Information;

        public ICollection<LoggerFilterRule> Rules { get; } = new List<LoggerFilterRule>();
    }
}