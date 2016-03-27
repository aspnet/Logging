// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Logging.Console
{
    public class ConsoleLoggerSettings : IConsoleLoggerSettings
    {
        public IChangeToken ChangeToken { get; set; }

        public bool IncludeScopes { get; set; }

        public IConsoleLoggerSettings Reload()
        {
            return this;
        }
    }
}
