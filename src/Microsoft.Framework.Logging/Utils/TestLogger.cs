﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Framework.Logging
{
    public class TestLogger : ILogger
    {
        private object _scope;
        private readonly TestSink _sink;
        private readonly string _name;
        private readonly bool _enabled;

        public TestLogger(string name, TestSink sink, bool enabled)
        {
            _sink = sink;
            _name = name;
            _enabled = enabled;
        }

        public string Name { get; set; }

        public IDisposable BeginScope(object state)
        {
            _scope = state;

            _sink.Begin(new BeginScopeContext()
            {
                LoggerName = _name,
                Scope = state,
            });

            return NullDisposable.Instance;
        }

        public void Write(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            _sink.Write(new WriteContext()
            {
                LogLevel = logLevel,
                EventId = eventId,
                State = state,
                Exception = exception,
                Formatter = formatter,
                LoggerName = _name,
                Scope = _scope,
#if ASPNET50 || ASPNETCORE50
                RequestId = LoggingContext.Current?.RequestId ?? Guid.Empty
#endif
            });
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _enabled;
        }
    }
}