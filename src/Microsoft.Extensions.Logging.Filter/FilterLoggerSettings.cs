﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Filter settings for messages logged by an <see cref="ILogger"/>.
    /// </summary>
    public class FilterLoggerSettings : IFilterLoggerSettings, IEnumerable<KeyValuePair<string, LogLevel>>
    {
        public IDictionary<string, LogLevel> Switches { get; set; } = new Dictionary<string, LogLevel>();

        /// <summary>
        /// Adds a filter for given logger category name and <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="categoryName">The logger category name.</param>
        /// <param name="logLevel">The log level.</param>
        public void Add(string categoryName, LogLevel logLevel)
        {
            Switches.Add(categoryName, logLevel);
        }

        public bool TryGetSwitch(string categoryName, out LogLevel level)
        {
            return Switches.TryGetValue(categoryName, out level);
        }

        IEnumerator<KeyValuePair<string, LogLevel>> IEnumerable<KeyValuePair<string, LogLevel>>.GetEnumerator()
        {
            return Switches.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Switches.GetEnumerator();
        }
    }
}
