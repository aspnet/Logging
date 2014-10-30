using System;
using System.Collections.Generic;

namespace Microsoft.Framework.Logging.Serilog
{
    public interface ILoggerStructure
    {
        IEnumerable<KeyValuePair<string, object>> GetValues();
    }
}