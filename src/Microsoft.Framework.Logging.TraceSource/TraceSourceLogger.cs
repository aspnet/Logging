// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using DiagnosticsTraceSource = System.Diagnostics.TraceSource;

namespace Microsoft.Framework.Logging.TraceSource
{
    internal class TraceSourceLogger : ILogger
    {
        private readonly DiagnosticsTraceSource _traceSource;

        public TraceSourceLogger(DiagnosticsTraceSource traceSource)
        {
            _traceSource = traceSource;
        }

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            var message = string.Empty;
            if (formatter != null)
            {
                message = formatter(state, exception);
            }
            else
            {
                if (state != null)
                {
                    message += state;
                }
                if (exception != null)
                {
                    message += Environment.NewLine + exception;
                }
            }
            if (!string.IsNullOrEmpty(message))
            {
                _traceSource.TraceEvent(GetEventType(logLevel), eventId, message);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            var traceEventType = GetEventType(logLevel);
            return _traceSource.Switch.ShouldTrace(traceEventType);
        }

        private static TraceEventType GetEventType(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical: return TraceEventType.Critical;
                case LogLevel.Error: return TraceEventType.Error;
                case LogLevel.Warning: return TraceEventType.Warning;
                case LogLevel.Information: return TraceEventType.Information;
                case LogLevel.Verbose:
                default: return TraceEventType.Verbose;
            }
        }

        public IDisposable BeginScopeImpl(object state)
        {
            return new TraceSourceScope(state);
        }

        public IDisposable BeginTrackedScopeImpl(object state, LogLevel logLevel, string startMessage, string endMessage, bool trackTime)
        {
            Log(logLevel, 0, startMessage, null, null);

            return new TraceSourceTrackedScope(state, this, logLevel, endMessage, trackTime);
        }
    }
}