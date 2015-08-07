// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.Framework.Internal;
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

        public ILogger CreateLogger(string name)
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

            public void Log(
                LogLevel logLevel,
                int eventId,
                object state,
                Exception exception,
                Func<object, Exception, string> formatter)
            {
                var nLogLogLevel = GetLogLevel(logLevel);
                var message = string.Empty;
                if (formatter != null)
                {
                    message = formatter(state, exception);
                }
                else
                {
                    message = LogFormatter.Formatter(state, exception);
                }
                if (!string.IsNullOrEmpty(message))
                {
                    var eventInfo = LogEventInfo.Create(nLogLogLevel, _logger.Name, message, exception);
                    eventInfo.Properties["EventId"] = eventId;
                    _logger.Log(eventInfo);
                }
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return _logger.IsEnabled(GetLogLevel(logLevel));
            }

            private global::NLog.LogLevel GetLogLevel(LogLevel logLevel)
            {
                switch (logLevel)
                {
                    case LogLevel.Verbose: return global::NLog.LogLevel.Trace;
                    case LogLevel.Debug: return global::NLog.LogLevel.Debug;
                    case LogLevel.Information: return global::NLog.LogLevel.Info;
                    case LogLevel.Warning: return global::NLog.LogLevel.Warn;
                    case LogLevel.Error: return global::NLog.LogLevel.Error;
                    case LogLevel.Critical: return global::NLog.LogLevel.Fatal;
                }
                return global::NLog.LogLevel.Debug;
            }

            public IDisposable BeginScopeImpl([NotNull] object state)
            {
                return NestedDiagnosticsContext.Push(state.ToString());
            }

            public IDisposable BeginTrackedScopeImpl(object state, LogLevel logLevel, string startMessage, string endMessage, bool trackTime)
            {
                var nlogScope = NestedDiagnosticsContext.Push(state.ToString());
                Log(logLevel, 0, startMessage, null, null);

                return new TrackedDisposable(this, logLevel, endMessage, trackTime, nlogScope);
            }

            private class TrackedDisposable : IDisposable
            {
                private readonly ILogger _logger;
                private readonly string _endMessage;
                private readonly Stopwatch _stopwatch;
                private readonly bool _trackTime;
                private readonly LogLevel _logLevel;
                private readonly IDisposable _nlogScope;
                private bool _disposed;

                public TrackedDisposable(ILogger logger, LogLevel logLevel, string endMessage, bool trackTime, IDisposable nlogScope)
                {
                    _endMessage = endMessage;
                    _logger = logger;
                    _logLevel = logLevel;
                    _nlogScope = nlogScope;
                    _trackTime = trackTime;

                    if (trackTime)
                    {
                        _stopwatch = new Stopwatch();
                        _stopwatch.Start();
                    }
                }

                protected virtual void Dispose(bool disposing)
                {
                    if (!_disposed)
                    {
                        if (disposing)
                        {
                            if (_endMessage != null && _logger.IsEnabled(_logLevel))
                            {
                                _logger.Log(_logLevel, 0, _endMessage, null, null);
                                if (_trackTime)
                                {
                                    _logger.Log(_logLevel, 0, $"Elapsed: {_stopwatch.Elapsed}", null, null);
                                }
                            }
                        }

                        _disposed = true;
                    }

                    _nlogScope.Dispose();
                }

                public void Dispose()
                {
                    Dispose(true);
                }
            }
        }
    }
}
