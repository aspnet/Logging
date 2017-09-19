namespace Microsoft.Extensions.Logging
{
    public static class LoggerMetricsExtensions
    {
        /// <summary>
        /// Define a new metric with the provided name and return an <see cref="IMetric"/> that can be used to report values for that metric.
        /// </summary>
        /// <remarks>
        /// If none of the registered logger providers support metrics, values recorded by this metric will be lost.
        /// </remarks>
        /// <param name="logger">The logger on which to define the metric</param>
        /// <param name="name">The name of the metric to define</param>
        /// <returns>An <see cref="IMetric"/> that can be used to report values for the metric</returns>
        public static IMetric DefineMetric(this ILogger logger, string name)
        {
            if(logger is IMetricLogger metricLogger)
            {
                return metricLogger.DefineMetric(name);
            }
            return NullMetric.Instance;
        }
    }
}
