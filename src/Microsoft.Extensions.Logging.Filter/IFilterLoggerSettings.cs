﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Filter settings for messages logged by an <see cref="ILogger"/>.
    /// </summary>
    public interface IFilterLoggerSettings
    {
        bool TryGetSwitch(string categoryName, out LogLevel level);
    }
}