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
    public class ConsoleObserver : IObserver<KeyValuePair<string, object>>
    {
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
                System.Diagnostics.Tracing.LogLevel logLevel = loggerArguments.Level;
                if (!IsEnabled(logLevel))
                {
                    return;
                }

                string logItemName = value.Key;

                string loggerName = loggerArguments.LoggerName;
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

        public virtual string FormatMessage(System.Diagnostics.Tracing.LogLevel logLevel, string logName, string message)
        {
            var logLevelString = GetRightPaddedLogLevelString(logLevel);
            return $"{logLevelString}: [{logName}] {message}";
        }

        public bool IsEnabled(System.Diagnostics.Tracing.LogLevel logLevel)
        {
            return true;
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

        public IDisposable BeginScopeImpl(object state)
        {
            return new NoopDisposable();
        }

        /*
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
        */

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