// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Framework.Logging
{
    public class TestLoggerProvider : ILoggerProvider
    {
        private readonly TestSink _sink;

        public TestLoggerProvider(TestSink sink)
        {
            _sink = sink;
        }

        public ILogger Create(string name)
        {
            return new TestLogger(name, _sink, true);
        }
    }
}