// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Framework.Logging
{
    public class LogicalOperation : IDisposable
    {
        private bool _disposedValue; // To detect redundant calls

        public LogicalOperation(ILogger logger, TraceType eventType, string message)
        {
            Logger = logger;
            EventType = eventType;
            Message = message;
        }

        public ILogger Logger { get; private set; }

        public TraceType EventType { get; private set; }

        public string Message { get; private set; }

        public void Dispose()
        {
            if (!_disposedValue)
            {
                Logger.WriteEnd(EventType, Message);
                _disposedValue = true;
            }
        }
    }
}