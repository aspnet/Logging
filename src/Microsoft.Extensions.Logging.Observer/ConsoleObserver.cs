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
                string loggerName = loggerArguments.LoggerName;
                if (!IsEnabled(loggerName, logLevel))
                {
                    return;
                }

                string logItemName = value.Key;
                var builder = new StringBuilder();
                GetPayload(loggerArguments.Arguments, builder);
                string payloadStr = builder.ToString();

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

        //TODO: add indentation & bullets
        internal static void GetPayload(object data, StringBuilder builder)
        {
            if (data == null)
                return;
            if(data is IEnumerable<KeyValuePair<string, object>>)
            {
                var first = true;
                foreach (var kvp in (IEnumerable<KeyValuePair<string, object>>)data)
                {
                    if (!first) { builder.Append(" "); first = false; }
                    builder.Append(kvp.Key);
                    builder.Append(": ");
                    GetPayload(kvp.Value, builder);
                }
            }
            else if(data is IEnumerable && !(data is String)) // String also implements IEnumerable
            {
                var list = data as IEnumerable;
                if (list != null)
                {
                    var first = true;
                    foreach (var elem in list)
                    {
                        if (!first) { builder.Append(", "); first = false; }
                        GetPayload(elem, builder);
                    }
                }
            }
            else if(data is KeyValuePair<string, object>)
            {
                var kvp = (KeyValuePair<string, object>)data;
                builder.Append(kvp.Key);
                builder.Append(": ");
                GetPayload(kvp.Value, builder);
            }
            else if(data.GetType().IsAnonymousType())
            {
                Type t = data.GetType();

                // Get a list of the properties
                IEnumerable<PropertyInfo> pList = t.GetTypeInfo().DeclaredProperties;

                var first = true;
                // Loop through the properties in the list
                foreach (PropertyInfo pi in pList)
                {
                    if(!first) { builder.Append(" "); first = false; }

                    // Get the value of the property
                    object o = pi.GetValue(data, null);
                    // Write out the property information
                    builder.Append(pi.Name);
                    builder.Append(": ");
                    GetPayload(o, builder);
                }
            }
            else
            {
                builder.Append(data.ToString());
            }
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
    public static class TypeExtension
    {

        public static Boolean IsAnonymousType(this Type type)
        {
            //TODO: Fix this method...is this correct?
            //Boolean hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Count() > 0;
            Boolean nameContainsAnonymousType = type.FullName.Contains("AnonymousType");

            return nameContainsAnonymousType;
        }
    }
}