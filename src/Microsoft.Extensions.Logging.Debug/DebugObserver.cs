// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.Logging.Debug
{
    public class DebugObserver : IObserver<KeyValuePair<string, object>>, IDisposable
    {
        public DebugObserver(String name, Func<string, LogLevel, bool> filter = null)
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
                string payloadStr = loggerArguments.PrintData();
                if (string.IsNullOrEmpty(payloadStr))
                {
                    return;
                }

                var message = $"{ logLevel }: {payloadStr}";
                System.Diagnostics.Debug.WriteLine(message, _name);
            }
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