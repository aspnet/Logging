namespace Microsoft.Extensions.Logging
{
    public struct Metric
    {
        public string Name { get; }
        public double Value { get; }

        public Metric(string name, double value)
        {
            Name = name;
            Value = value;
        }
    }
}
