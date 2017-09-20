using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace MetricsSample
{
    public class SampleConsoleMetricsLoggerProvider : ILoggerProvider
    {
        private ImmutableDictionary<ValueTuple<string, string>, SampleConsoleMetric> _metrics = ImmutableDictionary.Create<ValueTuple<string, string>, SampleConsoleMetric>();
        private readonly Timer _worker;

        public SampleConsoleMetricsLoggerProvider()
        {
            // Create a timer to flush metrics
            _worker = new Timer(self => ((SampleConsoleMetricsLoggerProvider)self).Worker(), this, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        private void Worker()
        {
            foreach(var metric in _metrics.Values)
            {
                ReportMetric(metric);
            }
        }

        private void ReportMetric(SampleConsoleMetric metric)
        {
            var aggregates = metric.GetAggregates();
            if (aggregates.Count > 0)
            {
                Console.WriteLine($"Metric({metric.Name}): N={aggregates.Count}, Σ={aggregates.Sum}, σ={aggregates.GetStdDev()}, max={aggregates.Max}, min={aggregates.Min}");
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new SampleConsoleMetricsLogger(categoryName, this);
        }

        public void Dispose()
        {
            _worker.Dispose();
        }

        internal IMetric DefineMetric(string categoryName, string name)
        {
            var key = (categoryName, name);
            return ImmutableInterlocked.GetOrAdd(ref _metrics, key, k => new SampleConsoleMetric(k.Item1, k.Item2, this));
        }
    }
}
