// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Provides a mechanism to filter log entries based on their category name,
    /// <see cref="LogLevel"/> and target <see cref="ILogSink"/>.
    /// </summary>
    public interface ILogFilter
    {
        bool IsEnabled(ILogSink sink, string categoryName, LogLevel level);
    }
}