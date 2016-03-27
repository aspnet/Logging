﻿using System;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Logging.Console
{
    public interface IConsoleLoggerSettings
    {
        bool IncludeScopes { get; }

        IChangeToken ChangeToken { get; }

        IConsoleLoggerSettings Reload();
    }
}
