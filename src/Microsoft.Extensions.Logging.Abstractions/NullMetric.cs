namespace Microsoft.Extensions.Logging
{
    public class NullMetric : IMetric
    {
        public static NullMetric Instance { get; } = new NullMetric();

        private NullMetric()
        {

        }

        public void RecordValue(double value)
        {
        }
    }
}
