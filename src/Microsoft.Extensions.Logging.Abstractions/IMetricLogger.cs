namespace Microsoft.Extensions.Logging
{
    public interface IMetricLogger
    {
        /// <summary>
        /// Define a new metric with the provided name and return an <see cref="IMetric"/> that can be used to report values for that metric.
        /// </summary>
        /// <param name="name">The name of the metric to define</param>
        /// <returns>An <see cref="IMetric"/> that can be used to report values for the metric</returns>
        IMetric DefineMetric(string name);
    }
}
