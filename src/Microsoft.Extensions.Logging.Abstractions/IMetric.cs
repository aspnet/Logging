using System.Collections.Generic;

namespace Microsoft.Extensions.Logging
{
    public interface IMetric
    {
        /// <summary>
        /// Record a new value for this metric.
        /// </summary>
        /// <param name="value">The value to record for this metric</param>
        void RecordValue(double value);

        /// <summary>
        /// Record a new value for this metric, including associated properties.
        /// </summary>
        /// <typeparam name="T">The type of the object containing the properties</typeparam>
        /// <param name="value">The value to record for this metric</param>
        /// <param name="properties">Arbitrary properties to associate with the metric</param>
        void RecordValue<T>(double value, T properties) where T: IEnumerable<KeyValuePair<string, object>>;
    }
}
