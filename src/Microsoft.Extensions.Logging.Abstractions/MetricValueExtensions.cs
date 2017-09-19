using System;

namespace Microsoft.Extensions.Logging
{
    public static class MetricValueExtensions
    {
        public static void RecordValue(this IMetric metric, TimeSpan value) => metric.RecordValue(value.TotalMilliseconds);
    }
}
