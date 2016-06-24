// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Logging.Abstractions
{
    public interface IConfigurableLogger : ILogger
    {
        string Name { get; }

        Func<string, LogLevel, bool> Filter { get; set; }

        bool IncludeScopes { get; set; }
    }
}