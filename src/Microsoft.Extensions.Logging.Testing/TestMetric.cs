// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.Logging.Testing
{
    public class TestMetric : IMetric
    {
        private ITestSink _sink;
        private readonly string _name;

        public TestMetric(ITestSink sink, string name)
        {
            _sink = sink;
            _name = name;
        }

        public void RecordValue(double value)
        {
            _sink.Metrics.Add(new MetricContext() { Name = _name, Value = value });
        }
    }
}
