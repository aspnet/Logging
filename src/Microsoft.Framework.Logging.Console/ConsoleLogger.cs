// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Microsoft.Framework.Logging.Console.Internal;

namespace Microsoft.Framework.Logging.Console
{
    public class ConsoleLogger : ILogger
    {
        private const int _indentation = 2;
        private readonly string _name;
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly object _lock = new object();

        public ConsoleLogger(string name, Func<string, LogLevel, bool> filter)
        {
            _name = name;
            _filter = filter ?? ((category, logLevel) => true);
            Console = new LogConsole();
        }

        public IConsole Console { get; set; }
        protected string Name { get { return _name; } }

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            var message = string.Empty;
            var values = state as ILogValues;
            if (formatter != null)
            {
                message = formatter(state, exception);
            }
            else if (values != null)
            {
                var builder = new StringBuilder();
                FormatLogValues(
                    builder,
                    values,
                    level: 1,
                    bullet: false);
                message = builder.ToString();
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
            lock (_lock)
            {
                SetConsoleColor(logLevel);
                try
                {
                    Console.WriteLine(FormatMessage(logLevel, message));
                }
                finally
                {
                    Console.ResetColor();
                }
            }
        }

        private string FormatMessage(LogLevel logLevel, string message)
        {
            var logLevelString = GetRightPaddedLogLevelString(logLevel);
            return $"{logLevelString}: [{_name}] {message}";
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _filter(_name, logLevel);
        }

        // sets the console text color to reflect the given LogLevel
        private void SetConsoleColor(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Information:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Verbose:
                default:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }
        }

        public IDisposable BeginScopeImpl(object state)
        {
            return new NoopDisposable();
        }

        public IDisposable BeginTrackedScopeImpl(object state, LogLevel logLevel, string startMessage, string endMessage, bool trackTime)
        {
            Log(logLevel, 0, startMessage, null, null);

            return new TrackedDisposable(this, logLevel, endMessage, trackTime);
        }

        private void FormatLogValues(StringBuilder builder, ILogValues logValues, int level, bool bullet)
        {
            var values = logValues.GetValues();
            if (values == null)
            {
                return;
            }
            var isFirst = true;
            foreach (var kvp in values)
            {
                builder.AppendLine();
                if (bullet && isFirst)
                {
                    builder.Append(' ', level * _indentation - 1)
                           .Append('-');
                }
                else
                {
                    builder.Append(' ', level * _indentation);
                }
                builder.Append(kvp.Key)
                       .Append(": ");
                if (kvp.Value is IEnumerable && !(kvp.Value is string))
                {
                    foreach (var value in (IEnumerable)kvp.Value)
                    {
                        if (value is ILogValues)
                        {
                            FormatLogValues(
                                builder,
                                (ILogValues)value,
                                level + 1,
                                bullet: true);
                        }
                        else
                        {
                            builder.AppendLine()
                                   .Append(' ', (level + 1) * _indentation)
                                   .Append(value);
                        }
                    }
                }
                else if (kvp.Value is ILogValues)
                {
                    FormatLogValues(
                        builder,
                        (ILogValues)kvp.Value,
                        level + 1,
                        bullet: false);
                }
                else
                {
                    builder.Append(kvp.Value);
                }
                isFirst = false;
            }
        }

        private static string GetRightPaddedLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    return "debug   ";
                case LogLevel.Verbose:
                    return "verbose ";
                case LogLevel.Information:
                    return "info    ";
                case LogLevel.Warning:
                    return "warning ";
                case LogLevel.Error:
                    return "error   ";
                case LogLevel.Critical:
                    return "critical";
                default:
                    return "unknown ";
            }
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