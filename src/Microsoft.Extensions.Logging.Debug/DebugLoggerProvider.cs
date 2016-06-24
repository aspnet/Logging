// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.Extensions.Logging.Debug
{
    /// <summary>
    /// The provider for the <see cref="DebugLogger"/>.
    /// </summary>
    public class DebugLoggerProvider : ConfigurableLoggerProvider<DebugLogger>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLoggerProvider"/> class.
        /// </summary>
        /// <param name="filter">The function used to filter events based on the log level.</param>
        /// <param name="includeScopes">A value which indicates whether log scope information should be displayed.</param>
        public DebugLoggerProvider(Func<string, LogLevel, bool> filter, bool includeScopes)
            : base(filter, includeScopes)
        {
        }

        public DebugLoggerProvider(IConfigurableLoggerSettings settings)
            : base(settings)
        {
        }

        protected override DebugLogger CreateLoggerImplementation(string name, Func<string, LogLevel, bool> filter, bool includeScopes)
        {
            return new DebugLogger(name, filter, includeScopes);
        }
    }
}
