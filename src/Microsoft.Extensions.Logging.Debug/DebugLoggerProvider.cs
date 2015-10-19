// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Logging.Debug
{
    /// <summary>
    /// The provider for the <see cref="DebugLogger"/>.
    /// </summary>
    public class DebugLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLoggerProvider"/> class.
        /// </summary>
        /// <param name="filter">The function used to filter events based on the log level.</param>
        public DebugLoggerProvider(Func<string, LogLevel, bool> filter)
        {
            Filter = filter;
        }

        // to enable unit testing
        internal Func<string, LogLevel, bool> Filter { get; }

        /// <inheritdoc />
        public ILogger CreateLogger(string name)
        {
            return new DebugLogger(name, Filter);
        }

        public void Dispose()
        {
        }
    }
}
