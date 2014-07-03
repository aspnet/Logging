// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Framework.Logging.Infrastructure
{
    /// <summary>
    /// Writes a stop to the log on disposal by calling <see cref="LoggerExtensions.WriteStop(ILogger, int, object, Exception, Func{object, Exception, string})"/>.
    /// </summary>
    public class LogicalOperation : IDisposable
    {
        private bool _disposedValue; // To detect redundant calls

        public LogicalOperation(ILogger logger, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            Logger = logger;
            EventId = eventId;
            State = state;
            Exception = exception;
            Formatter = formatter;
        }

        public ILogger Logger { get; private set; }

        public int EventId { get; private set; }

        public object State { get; private set; }

        public Exception Exception { get; private set; }

        public Func<object, Exception, string> Formatter { get; private set; }

        public void Dispose()
        {
            if (!_disposedValue)
            {
                Logger.WriteStop(EventId, State, Exception, Formatter);
                _disposedValue = true;
            }
        }
    }
}