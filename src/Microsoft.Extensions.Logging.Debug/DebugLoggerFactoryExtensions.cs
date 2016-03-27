// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging.Debug;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Extension methods for the <see cref="LoggerFactory"/> class.
    /// </summary>
    public static class DebugLoggerFactoryExtensions
    {
        /// <summary>
        /// Adds a debug sink.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        public static LoggerFactory AddDebug(this LoggerFactory factory)
        {
            factory.AddSink(new DebugSink());
            return factory;
        }
    }
}