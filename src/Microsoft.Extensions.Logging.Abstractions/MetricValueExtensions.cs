using System;

namespace Microsoft.Extensions.Logging
{
    public static class MetricValueExtensions
    {
        /// <summary>
        /// Record a new value for this metric.
        /// </summary>
        /// <remarks>
        /// This is a convenience method that will convert the <see cref="TimeSpan"/> to a <see cref="double"/> via
        /// the <see cref="TimeSpan.TotalMilliseconds"/> property.
        /// </remarks>
        /// <param name="metric">The metric to record the value on.</param>
        /// <param name="value">The value to record for this metric.</param>
        public static void RecordValue(this IMetric metric, TimeSpan value) => metric.RecordValue(value.TotalMilliseconds);
    }
}
