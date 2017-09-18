using Microsoft.Extensions.Logging;

namespace MetricsSample
{
    internal class SampleConsoleMetric : IMetric
    {
        private readonly SampleConsoleMetricsLoggerProvider _loggerProvider;

        private object _lock = new object();

        private int _count;
        private double _sum;
        private double _sumOfSquares;
        private double? _max;
        private double? _min;

        public string CategoryName { get; }
        public string Name { get; }

        public SampleConsoleMetric(string categoryName, string name, SampleConsoleMetricsLoggerProvider loggerProvider)
        {
            _count = 0;
            _sum = 0;
            _sumOfSquares = 0;
            CategoryName = categoryName;
            Name = name;
            _loggerProvider = loggerProvider;
        }

        public void RecordValue(double value)
        {
            lock (_lock)
            {
                if (_max == null || _max.Value < value)
                {
                    _max = value;
                }
                if (_min == null || _min.Value > value)
                {
                    _min = value;
                }
                _count += 1;
                _sum += value;
                _sumOfSquares += (value * value);
            }
        }

        public MetricAggregates GetAggregates()
        {
            lock (_lock)
            {
                return new MetricAggregates(_count, _sum, _sumOfSquares, _max, _min);
            }
        }
    }
}
