// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Framework.Logging.Console
{
    public class ConsoleLogger : ILogger
    {
        private readonly string _name;
        private Func<string, LogLevel, bool> _filter;
        private readonly object _lock = new object();

        public ConsoleLogger(string name, Func<string, LogLevel, bool> filter)
        {
            _name = name;
            _filter = filter ?? ((category, logLevel) => true);
            Console = new LogConsole();
        }

        public IConsole Console { get; set; }

        public void Write(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            var message = string.Empty;
            if (state is ILoggerStructure)
            {
                message = FormatLoggerStructure((ILoggerStructure)state, 1);
            }
            else if (formatter != null)
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
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            lock (_lock)
            {
                var originalForegroundColor = Console.ForegroundColor;  // save current colors
                var originalBackgroundColor = Console.BackgroundColor;
                var severity = logLevel.ToString().ToUpperInvariant();
                SetConsoleColor(logLevel);
                try
                {
                    Console.WriteLine("[{0}:{1}] {2}", severity, _name, message);
                }
                finally
                {
                    Console.ForegroundColor = originalForegroundColor;  // reset initial colors
                    Console.BackgroundColor = originalBackgroundColor;
                }
            }
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

        public IDisposable BeginScope(object state)
        {
            return null;
        }

        private string FormatLoggerStructure(ILoggerStructure structure, int level)
        {
            var values = structure.GetValues();
            if (values == null)
            {
                return string.Empty;
            }
            var builder = new StringBuilder();
            foreach (var kvp in values)
            {
                builder.Append("\r\n");
                for (var i = 0; i < level; i++)
                {
                    builder.Append("\t");
                }
                builder.Append(kvp.Key);
                builder.Append(": ");
                if ((kvp.Value as IEnumerable<ILoggerStructure>) != null)
                {
                    foreach (var value in (IEnumerable<ILoggerStructure>)kvp.Value)
                    {
                        builder.Append(FormatLoggerStructure(value, level + 1));
                        builder.Append("\r\n");
                    }
                }
                else if ((kvp.Value as ILoggerStructure) != null)
                {
                    builder.Append(FormatLoggerStructure((ILoggerStructure)kvp.Value, level + 1));
                }
                else
                {
                    builder.Append(kvp.Value);
                }
            }
            return builder.ToString();
        }
    }
}