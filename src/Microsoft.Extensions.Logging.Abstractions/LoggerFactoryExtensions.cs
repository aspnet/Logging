// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Internal;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// ILoggerFactory extension methods for common scenarios.
    /// </summary>
    public static class LoggerFactoryExtensions
    {
        /// <summary>
        /// Creates a new ILogger instance using the full name of the given type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="factory">The factory.</param>
        public static ILogger CreateLogger<T>(this ILoggerFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return factory.CreateLogger(TypeNameHelper.GetTypeDisplayName(typeof(T), fullName: true));
        }

        public static System.Diagnostics.Tracing.Logger CreateSystemLogger<T>(this ILoggerFactory factory)
        {
            return factory.CreateSystemLogger(TypeNameHelper.GetTypeDisplayName(typeof(T), fullName: true));
        }

    }
}