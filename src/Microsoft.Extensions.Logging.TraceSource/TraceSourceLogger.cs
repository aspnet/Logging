// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using DiagnosticsTraceSource = System.Diagnostics.TraceSource;

namespace Microsoft.Extensions.Logging.TraceSource
{
    public class TraceSourceLogger
    {
        public void Log<TState>(DiagnosticsTraceSource traceSource, LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(traceSource, logLevel))
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
                traceSource.TraceEvent(GetEventType(logLevel), eventId.Id, message);
            }
        }

        public bool IsEnabled(DiagnosticsTraceSource traceSource, LogLevel logLevel)
        {
            var traceEventType = GetEventType(logLevel);
            return traceSource.Switch.ShouldTrace(traceEventType);
        }

        private static TraceEventType GetEventType(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical: return TraceEventType.Critical;
                case LogLevel.Error: return TraceEventType.Error;
                case LogLevel.Warning: return TraceEventType.Warning;
                case LogLevel.Information: return TraceEventType.Information;
                case LogLevel.Trace:
                default: return TraceEventType.Verbose;
            }
        }

        public IDisposable BeginScopeImpl(object state)
        {
            return new TraceSourceScope(state);
        }
    }
}