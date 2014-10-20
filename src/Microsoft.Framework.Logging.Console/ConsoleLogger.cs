// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;

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
#if !ASPNETCORE50
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => OnProcessExit(sender, e, Console, this);
#endif
            // store original console colors
            OriginalForegroundColor = Console.ForegroundColor;
            OriginalBackgroundColor = Console.BackgroundColor;
        }

        public ConsoleColor OriginalForegroundColor { get; private set; }
        public ConsoleColor OriginalBackgroundColor { get; private set; }

        public IConsole Console { get; set; }

        public void Write(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
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
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            lock (_lock)
            {
                var severity = logLevel.ToString().ToUpperInvariant();
                SetConsoleColor(logLevel);
                try
                {
                    Console.WriteLine("[{0}:{1}] {2}", severity, _name, message);
                }
                finally
                {
                    Console.ForegroundColor = OriginalForegroundColor;  // reset initial colors
                    Console.BackgroundColor = OriginalBackgroundColor;
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

        // delay ending this process until console colors have been reset to their original state
        public static void OnProcessExit(object sender, EventArgs e, IConsole Console, ConsoleLogger logger)
        {
            // if it takes more than a second, let it end
            int i = 0;
            while ((System.Console.ForegroundColor != logger.OriginalForegroundColor || 
                   System.Console.BackgroundColor != logger.OriginalBackgroundColor) &&
                   i < 10)
            {
                i++;
#if !ASPNETCORE50
                Thread.Sleep(100);
#endif
            }
        }
    }
}