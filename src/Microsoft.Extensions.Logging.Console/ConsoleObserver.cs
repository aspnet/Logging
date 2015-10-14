// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.Logging.Console
{
    public class ConsoleObserver : IObserver<KeyValuePair<string, object>>, IDisposable
    {
        public ConsoleObserver(String name, Func<string, LogLevel, bool> filter = null)
        {
            _name = name;
            _filter = filter;
        }
        public void OnCompleted()
        {
            //TODO
        }
        public void OnError(Exception error)
        {
            // TODO
        }
        public void OnNext(KeyValuePair<string, object> value)
        {
            var loggerArguments = value.Value as LoggerArguments;
            if (loggerArguments != null)
            {
                LogLevel logLevel = loggerArguments.Level;
                if (!IsEnabled(logLevel))
                {
                    return;
                }
                string loggerName = loggerArguments.LoggerName;
                string logItemName = value.Key;
                
                string payloadStr = loggerArguments.Arguments.PrintData();

                if (string.IsNullOrEmpty(payloadStr))
                {
                    return;
                }
                lock (_lock)
                {
                    SetConsoleColor(logLevel);
                    try
                    {
                        System.Console.WriteLine(FormatMessage(logLevel, loggerName, payloadStr));
                    }
                    finally
                    {
                        System.Console.ResetColor();
                    }
                }
            }
        }

        //TODO: add indentation & bullets

        public virtual string FormatMessage(LogLevel logLevel, string logName, string message)
        {
            var logLevelString = GetRightPaddedLogLevelString(logLevel);
            return $"{logLevelString}: [{logName}] {message}";
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (_filter != null)
                return _filter(Name, logLevel);
            else
                return true;
        }

        public IDisposable BeginScopeImpl(object state)
        {
            return new NoopDisposable();
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }


        // sets the console text color to reflect the given LogLevel
        private void SetConsoleColor(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    System.Console.BackgroundColor = ConsoleColor.Red;
                    System.Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Error:
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Warning:
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Informational:
                    System.Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Verbose:
                default:
                    System.Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }
        }

        private static string GetRightPaddedLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.LogAlways:
                    return "LogAlways   ";
                case LogLevel.Verbose:
                    return "verbose ";
                case LogLevel.Informational:
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

        private readonly string _name;
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly object _lock = new object();

        protected string Name { get { return _name; } }

        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }

    }
}