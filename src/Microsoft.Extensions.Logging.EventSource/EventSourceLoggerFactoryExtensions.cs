// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging.EventSourceLogger;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Extension methods for the <see cref="ILoggerFactory"/> class.
    /// </summary>
    public static class EventSourceLoggerFactoryExtensions
    {
        /// <summary>
        /// Adds an event logger that is enabled for <see cref="LogLevel"/>.Information or higher.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        public static ILoggerFactory AddEventSourceLogger(this ILoggerFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            LoggingEventSource.Instance.CreateLoggerProvider(factory);

            // Do not call factory.AddProvider() here. EventSourceLoggerProvider will do that automatically 
            // when something starts using Microsoft-Extensions-Logging EventSource. This way there is no overhead
            // from using the EventSourceLogger when no one is interested in the data it exposes.

            return factory;
        }
    }
}
