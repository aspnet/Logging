using System;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging
{
    internal class StaticFilterOptionsMonitor : IOptionsMonitor<LoggerFilterOptions>
    {
        public StaticFilterOptionsMonitor(LoggerFilterOptions currentValue)
        {
            CurrentValue = currentValue;
        }

        public IDisposable OnChange(Action<LoggerFilterOptions> listener)
        {
            return null;
        }

        public LoggerFilterOptions CurrentValue { get; }
    }
}