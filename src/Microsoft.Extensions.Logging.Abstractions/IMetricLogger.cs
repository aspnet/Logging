using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Logging
{
    public interface IMetricLogger
    {
        IMetric DefineMetric(string name);
    }
}
