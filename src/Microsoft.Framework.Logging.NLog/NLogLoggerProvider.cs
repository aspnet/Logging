﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using NLog;

namespace Microsoft.Framework.Logging.NLog
{
    public class NLogLoggerProvider : ILoggerProvider
    {
        private readonly LogFactory _logFactory;

        public NLogLoggerProvider(LogFactory logFactory)
        {
            _logFactory = logFactory;
        }

        public ILogger Create(string name)
        {
            return new Logger(_logFactory.GetLogger(name));
        }

        private class Logger : ILogger
        {
            private readonly global::NLog.Logger _logger;

            public Logger(global::NLog.Logger logger)
            {
                _logger = logger;
            }

            public void Write(
                TraceType eventType,
                int eventId,
                object state,
                Exception exception,
                Func<object, Exception, string> formatter)
            {
                var logLevel = GetLogLevel(eventType);
                if (formatter != null)
                {
                    var message = formatter(state, exception);
                    var eventInfo = LogEventInfo.Create(logLevel, _logger.Name, message, exception);
                    eventInfo.Properties["EventId"] = eventId;
                    _logger.Log(eventInfo);
                }
            }

            public bool IsEnabled(TraceType eventType)
            {
                return _logger.IsEnabled(GetLogLevel(eventType));
            }

            private LogLevel GetLogLevel(TraceType eventType)
            {
                switch (eventType)
                {
                    case TraceType.Verbose: return LogLevel.Debug;
                    case TraceType.Information: return LogLevel.Info;
                    case TraceType.Warning: return LogLevel.Warn;
                    case TraceType.Error: return LogLevel.Error;
                    case TraceType.Critical: return LogLevel.Fatal;
                }
                return LogLevel.Debug;
            }

            public IDisposable BeginScope(object state)
            {
                return NestedDiagnosticsContext.Push(state.ToString());
            }
        }
    }
}
