// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

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

        public void RecordValue<T>(double value, T properties) where T : IEnumerable<KeyValuePair<string, object>>
        {
            _sink.Metrics.Add(new MetricContext() { Name = _name, Value = value, Properties = properties });
        }
    }
}
