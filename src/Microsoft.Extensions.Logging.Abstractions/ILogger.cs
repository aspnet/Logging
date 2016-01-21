// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Represents a type used to perform logging.
    /// </summary>
    /// <remarks>Aggregates most logging patterns to a single method.</remarks>
    public interface ILogger
    {
        /// <summary>
        /// Writes a event to the logger.
        /// </summary>
        /// <param name="logLevel">Event will be written on this level.</param>
        /// <param name="eventId">Id of this event.</param>
        /// <param name="state">The event to be written. Can be also an object.</param>
        /// <param name="exception">The exception correlated to this event.</param>
        /// <param name="formatter">Method to create a <c>string</c> message of the <paramref name="state"/> and <paramref name="exception"/>.</param>
        void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter);

        /// <summary>
        /// Checks if the given LogLevel is enabled.
        /// </summary>
        /// <param name="logLevel">level to be checked.</param>
        /// <returns><c>true</c> if enabled.</returns>
        bool IsEnabled(LogLevel logLevel);

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
        IDisposable BeginScopeImpl(object state);
    }
}
