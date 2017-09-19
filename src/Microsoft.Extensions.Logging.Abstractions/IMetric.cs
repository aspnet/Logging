namespace Microsoft.Extensions.Logging
{
    public interface IMetric
    {
        void RecordValue(double value);
    }
}
