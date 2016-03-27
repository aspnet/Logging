// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Logging.Filtering
{
    /// <summary>
    /// Settings for the <see cref="LogFilter"/> class.
    /// </summary>
    public interface ILogFilterSettings
    {
        bool TryGetSwitch(Type sinkType, string categoryName, out LogLevel logLevel);
    }
}