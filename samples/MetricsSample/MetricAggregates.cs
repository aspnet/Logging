using System;

namespace MetricsSample
{
    public struct MetricAggregates
    {
        public int Count { get; }
        public double Sum { get; }
        public double SumOfSquares { get; }
        public double? Max { get; }
        public double? Min { get; }

        public MetricAggregates(int count, double sum, double sumOfSquares, double? max, double? min)
        {
            Count = count;
            Sum = sum;
            SumOfSquares = sumOfSquares;
            Max = max;
            Min = min;
        }

        public double GetMean() => Sum / Count;

        // Estimated std-dev via online sum-of-squares method.
        public double GetStdDev() => Math.Sqrt((SumOfSquares - ((Sum * Sum) / Count)) / (Count - 1));
    }
}
