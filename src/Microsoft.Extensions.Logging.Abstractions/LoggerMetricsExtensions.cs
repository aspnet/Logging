namespace Microsoft.Extensions.Logging
{
    public static class LoggerMetricsExtensions
    {
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
