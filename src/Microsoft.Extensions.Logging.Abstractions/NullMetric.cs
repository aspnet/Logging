using System.Collections.Generic;

namespace Microsoft.Extensions.Logging
{
    public class NullMetric : IMetric
    {
        public static NullMetric Instance { get; } = new NullMetric();

        private NullMetric()
        {

        }

        public void RecordValue(double value)
        {
        }

        public void RecordValue<T>(double value, T properties) where T : IEnumerable<KeyValuePair<string, object>>
        {
        }
    }
}
