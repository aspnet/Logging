// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.Logging.Test
{
    public class TestLoggerProvider : ILoggerProvider
    {
        private readonly bool _isEnabled;

        public TestLoggerProvider(TestSink testSink, bool isEnabled)
        {
            Sink = testSink;
            _isEnabled = isEnabled;
        }

        public TestSink Sink { get; }

        public bool DisposeCalled { get; private set; }

        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger(categoryName, Sink, _isEnabled);
        }

        public void Dispose()
        {
            DisposeCalled = true;
        }
    }
}
