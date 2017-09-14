namespace Microsoft.Extensions.Logging
{
    public struct Metric
    {
        public double Value { get; }

        public Metric(double value)
        {
            Value = value;
        }
    }
}
