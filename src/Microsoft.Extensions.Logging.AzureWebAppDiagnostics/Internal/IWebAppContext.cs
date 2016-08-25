﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.Logging.AzureWebAppDiagnostics.Internal
{
    /// <summary>
    /// Represents an Azure WebApp context
    /// </summary>
    public interface IWebAppContext
    {
        /// <summary>
        /// Gets the path to the home folder if running in Azure WebApp
        /// </summary>
        string HomeFolder { get; }

        /// <summary>
        /// Gets the name of site if running in Azure WebApp
        /// </summary>
        string SiteName { get; }

        /// <summary>
        /// Gets a value indicating whether or new we're in an Azure WebApp
        /// </summary>
        bool IsRunningInAzureWebApp { get; }
    }
}
