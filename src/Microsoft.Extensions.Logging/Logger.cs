// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Logging
{
    internal class Logger : ILogger
    {
        private readonly LoggerObservable _loggerObservable;
        private readonly string _name;

        public Logger(LoggerObservable loggerObservable, string name)
        {
            _loggerObservable = loggerObservable;
            _name = name;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _loggerObservable.Log(_name, logLevel, eventId, state, exception, formatter);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _loggerObservable.IsEnabled(_name, logLevel);
        }

        public IDisposable BeginScopeImpl(object state)
        {
            return _loggerObservable.BeginScopeImpl(_name, state);
        }
    }
}