// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.Framework.Logging.Internal;

namespace Microsoft.Framework.Logging.TraceSource
{
    /// <summary>
    /// Provides an IDisposable that represents a logical operation scope based on System.Diagnostics LogicalOperationStack
    /// </summary>
    internal class TraceSourceTrackedScope : IDisposable
    {
        // To detect redundant calls
        private bool _isDisposed;
        private readonly ILogger _logger;
        private readonly string _endMessage;
        private readonly Stopwatch _stopwatch;
        private readonly bool _trackTime;
        private readonly LogLevel _logLevel;


        /// <summary>
        /// Pushes state onto the LogicalOperationStack by calling 
        /// <see cref="Trace.CorrelationManager.StartLogicalOperation(object operationId)"/>
        /// </summary>
        /// <param name="state">The state.</param>
        public TraceSourceTrackedScope(object state, ILogger logger, LogLevel logLevel, string endMessage, bool trackTime)
        {
            _endMessage = endMessage;
            _logger = logger;
            _logLevel = logLevel;
            _trackTime = trackTime;

            if (trackTime)
            {
                _stopwatch = new Stopwatch();
                _stopwatch.Start();
            }

#if NET45 || DNX451
            Trace.CorrelationManager.StartLogicalOperation(state);
#endif
        }

        /// <summary>
        /// Pops a state off the LogicalOperationStack by calling
        /// <see cref="Trace.CorrelationManager.StopLogicalOperation()"/>
        /// </summary>
        /// <param name="state">The state.</param>
        public void Dispose()
        {
            if (!_isDisposed)
            {
#if NET45 || DNX451
                Trace.CorrelationManager.StopLogicalOperation();
#endif
                if (_endMessage != null && _logger.IsEnabled(_logLevel))
                {
                    _logger.Log(_logLevel, 0, new FormattedLogValues(_endMessage), null, null);
                    if (_trackTime)
                    {
                        _logger.Log(_logLevel, 0, new FormattedLogValues("{0}", _stopwatch.Elapsed), null, null);
                    }
                }

                _isDisposed = true;
            }
        }
    }
}
