namespace Microsoft.Extensions.Logging
{
    public static class LoggerMetricsExtensions
    {
        public static void RecordMetric(this ILogger logger, string name, double value) => logger.RecordMetric(new Metric(name, value));

        public static void RecordMetric(this ILogger logger, Metric metric)
        {
            if(logger is IMetricLogger metricLogger)
            {
                metricLogger.RecordMetric(metric);
            }
            // Not a metric logger? Drop the metric on the floor!
        }
    }
}
