using System.Collections.Generic;

namespace Microsoft.Extensions.Logging.Testing
{
    public class MetricContext
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public IEnumerable<KeyValuePair<string, object>> Properties { get; set; }
    }
}
