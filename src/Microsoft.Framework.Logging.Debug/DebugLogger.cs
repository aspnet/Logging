// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Microsoft.Framework.Logging.Debug
{
    /// <summary>
    /// A logger that writes messages in the debug output window only when a debugger is attached.
    /// </summary>
    public partial class DebugLogger : ILogger
    {
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLogger"/> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        public DebugLogger(string name)
            : this(name, filter: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLogger"/> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="filter">The function used to filter events based on the log level.</param>
        public DebugLogger(string name, Func<string, LogLevel, bool> filter)
        {
            _name = string.IsNullOrEmpty(name) ? nameof(DebugLogger) : name;
            _filter = filter;
        }


        /// <inheritdoc />
        public IDisposable BeginScopeImpl(object state)
        {
            return new NoopDisposable();
        }

        public IDisposable BeginTrackedScopeImpl(object state, LogLevel logLevel, string startMessage, string endMessage, bool trackTime)
        {
            Log(logLevel, 0, startMessage, null, null);

            return new TrackedDisposable(this, logLevel, endMessage, trackTime);
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            // If the filter is null, everything is enabled
            // unless the debugger is not attached
            return Debugger.IsAttached &&
                (_filter == null || _filter(_name, logLevel));
        }

        /// <inheritdoc />
        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string message;
            var values = state as ILogValues;
            if (formatter != null)
            {
                message = formatter(state, exception);
            }
            else if (values != null)
            {
                message = LogFormatter.FormatLogValues(values);
                if (exception != null)
                {
                    message += Environment.NewLine + exception;
                }
            }
            else
            {
                message = LogFormatter.Formatter(state, exception);
            }

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $"{ logLevel }: {message}";
            DebugWriteLine(message, _name);
        }

        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }

        private class TrackedDisposable : IDisposable
        {
            private readonly ILogger _logger;
            private readonly string _endMessage;
            private readonly Stopwatch _stopwatch;
            private readonly bool _trackTime;
            private readonly LogLevel _logLevel;
            private bool _disposed;

            public TrackedDisposable(ILogger logger, LogLevel logLevel, string endMessage, bool trackTime)
            {
                _endMessage = endMessage;
                _logger = logger;
                _logLevel = logLevel;
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
            }

            public void Dispose()
            {
                Dispose(true);
            }
        }
    }
}
