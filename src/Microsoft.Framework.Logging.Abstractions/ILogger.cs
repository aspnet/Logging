// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Framework.Logging
{
    /// <summary>
    /// A generic interface for logging.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Aggregates most logging patterns to a single method.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter);

        /// <summary>
        /// Checks if the given LogLevel is enabled.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        bool IsEnabled(LogLevel logLevel);

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
        IDisposable BeginScopeImpl(object state);

        /// <summary>
        /// Begins a logical tracked scope. This will track time elapsed in the scope as well as an enter and exit message.
        /// </summary>
        /// <param name="state">The identifier for the scope.</param>
        /// <param name="endMessage">The message to log when the scope terminates.</param>
        /// <param name="logLevel">LogLevel to log entry/exit and time taken messages at.</param>
        /// <param name="startMessage">The message to log at the beginning of the scope.</param>
        /// <param name="trackTime">Whether or not to log the time elapsed in the scope.</param>
        /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
        IDisposable BeginTrackedScopeImpl(object state, LogLevel logLevel, string startMessage, string endMessage, bool trackTime);
    }
}
