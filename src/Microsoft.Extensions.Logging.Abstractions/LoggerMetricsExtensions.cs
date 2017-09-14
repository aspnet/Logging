namespace Microsoft.Extensions.Logging
{
    public static class LoggerMetricsExtensions
    {
        public static void LogMetric(this ILogger logger, LogLevel level, EventId metricId, double value)
        {
            logger.Log(level, metricId, new Metric(value), exception: null, formatter: (metric, _) => $"Metric recorded: {metricId.Name ?? metricId.Id.ToString()}");
        }
    }
}
