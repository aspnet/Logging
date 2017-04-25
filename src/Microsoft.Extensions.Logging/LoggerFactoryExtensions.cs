// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Extension methods for adding filters to the <see cref="LoggerFactory"/>.
    /// </summary>
    public static class LoggerFactoryExtensions
    {
        /// <summary>
        /// Adds a filter that applies to <paramref name="providerName"/> and <paramref name="categoryName"/>, allowing logs with the given
        /// minimum <see cref="LogLevel"/> or higher.
        /// </summary>
        /// <param name="factory">The <see cref="LoggerFactory" /> to apply the filter to.</param>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="categoryName">The name of the logger category.</param>
        /// <param name="minLevel">The minimum <see cref="LogLevel"/> that logs from
        /// <paramref name="providerName"/> and <paramref name="categoryName"/> are allowed.</param>
        public static LoggerFactory AddFilter(this LoggerFactory factory, string providerName, string categoryName, LogLevel minLevel)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return factory.AddFilter(providerName, categoryName, level => level >= minLevel);
        }

        /// <summary>
        /// Adds a filter that applies to <paramref name="providerName"/> with the given
        /// <paramref name="filter"/>.
        /// </summary>
        /// <param name="factory">The <see cref="LoggerFactory" /> to apply the filter to.</param>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="filter">The filter that applies to logs for <paramref name="providerName"/>.
        /// Returning true means allow log through, false means reject log.</param>
        public static LoggerFactory AddFilter(this LoggerFactory factory, string providerName, Func<LogLevel, bool> filter)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            // Using 'Default' for the category name means this filter will apply for all category names
            return factory.AddFilter(providerName, "Default", filter);
        }
    }
}
