// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Microsoft.Extensions.Logging.Debug
{
    /// <summary>
    /// A logger that writes messages in the debug output window only when a debugger is attached.
    /// </summary>
    public partial class DebugSink : ILogSink
    {
        public IDisposable BeginScope(string categoryName, object state)
        {
            return new NoopDisposable();
        }

        /// <inheritdoc />
        public bool IsEnabled(string categoryName, LogLevel logLevel)
        {
            return Debugger.IsAttached;
        }

        /// <inheritdoc />
        public void Log<TState>(
            string categoryName,
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(categoryName, logLevel))
            {
                return;
            }

            if(formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $"{ logLevel }: {message}";
            DebugWriteLine(message, categoryName);
        }

        public void Dispose()
        {
        }

        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
