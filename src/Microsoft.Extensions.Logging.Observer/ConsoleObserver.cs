// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.Logging.Observer
{
    public class ConsoleObserver : IObserver<KeyValuePair<string, object>>, IDisposable
    {
        public ConsoleObserver(Func<string, LogLevel, bool> filter = null)
        {
            _filter = filter;
        }
        public void OnCompleted()
        {
        }
        public void OnError(Exception error)
        {
            // System.Console.WriteLine("ERROR in logging stream: {0}", error.Message);
        }
        public void OnNext(KeyValuePair<string, object> value)
        {
            var loggerArguments = value.Value as LoggerArguments;
            if (loggerArguments != null)
            {
                LogLevel logLevel = loggerArguments.Level;
                string loggerName = loggerArguments.LoggerName;
                if (!IsEnabled(loggerName, logLevel))
                {
                    return;
                }

                string logItemName = value.Key;
                string payloadStr = GetPayload(loggerArguments.Arguments);

                if (string.IsNullOrEmpty(payloadStr))
                {
                    return;
                }
                lock (_lock)
                {
                    SetConsoleColor(logLevel);
                    try
                    {
                        Console.WriteLine(FormatMessage(logLevel, loggerName, payloadStr));
                    }
                    finally
                    {
                        Console.ResetColor();
                    }
                }
            }
        }

        public Func<string, LogLevel, bool> Filter
        {
            get
            {
                return _filter;
            }
        }


        internal static string GetPayload(object data)
        {
            if (data == null)
                return null;
            if(data.GetType().GetTypeInfo().IsPrimitive || data is String || data is Exception)
            {
                return data.ToString();
            }
            var builder = new StringBuilder();
            if (data != null)
            {
                Type t = data.GetType();
                // Get a list of the properties
                IEnumerable<PropertyInfo> pList = t.GetTypeInfo().DeclaredProperties;
                // Loop through the properties in the list
                foreach (PropertyInfo pi in pList)
                {
                    // Get the value of the property
                    object o = pi.GetValue(data, null);
                    // Write out the property information
                    builder.Append(pi.Name);
                    builder.Append(" = ");
                    builder.Append(o.ToString());
                    builder.Append(", ");
                }
                builder.Remove(builder.Length - 2, 2);
            }
            return builder.ToString();
        }

        public virtual string FormatMessage(LogLevel logLevel, string logName, string message)
        {
            var logLevelString = GetRightPaddedLogLevelString(logLevel);
            return $"{logLevelString}: [{logName}] {message}";
        }

        public bool IsEnabled(string loggerName, LogLevel logLevel)
        {
            if (Filter != null)
                return Filter(loggerName, logLevel);
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
        private void SetConsoleColor(System.Diagnostics.Tracing.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case System.Diagnostics.Tracing.LogLevel.Critical:
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case System.Diagnostics.Tracing.LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case System.Diagnostics.Tracing.LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case System.Diagnostics.Tracing.LogLevel.Informational:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case System.Diagnostics.Tracing.LogLevel.Verbose:
                default:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }
        }

        private static string GetRightPaddedLogLevelString(System.Diagnostics.Tracing.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case System.Diagnostics.Tracing.LogLevel.LogAlways:
                    return "LogAlways   ";
                case System.Diagnostics.Tracing.LogLevel.Verbose:
                    return "verbose ";
                case System.Diagnostics.Tracing.LogLevel.Informational:
                    return "info    ";
                case System.Diagnostics.Tracing.LogLevel.Warning:
                    return "warning ";
                case System.Diagnostics.Tracing.LogLevel.Error:
                    return "error   ";
                case System.Diagnostics.Tracing.LogLevel.Critical:
                    return "critical";
                default:
                    return "unknown ";
            }
        }

        private const int _indentation = 2;
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