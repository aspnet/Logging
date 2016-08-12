// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Serilog;
using Serilog.Core;

namespace Microsoft.Extensions.Logging.AzureWebAppDiagnostics.Internal
{
    /// <summary>
    /// Represents a Serilog logger provider use for Azure WebApp.
    /// </summary>
    public abstract class SerilogLoggerProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public abstract Logger ConfigureLogger(IWebAppLogConfigurationReader reader);
    }
}
