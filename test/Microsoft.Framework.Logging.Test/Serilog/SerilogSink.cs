using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace Microsoft.Framework.Logging.Test.Serilog
{
    public class SerilogSink : ILogEventSink
    {
        public List<LogEvent> Writes { get; set; } = new List<LogEvent>();

        public void Emit(LogEvent logEvent)
        {
            Writes.Add(logEvent);
        }
    }
}