using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Logging
{
    public interface IMetricLogger
    {
        void RecordMetric(Metric metric);
    }
}
