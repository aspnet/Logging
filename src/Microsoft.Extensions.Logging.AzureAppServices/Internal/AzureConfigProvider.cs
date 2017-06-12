// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Logging.AzureAppServices.Internal
{
    public class AzureConfigProvider
    {
        public IConfiguration GetAzureLoggingConfiguration(WebAppContext context)
        {
            var settingsFolder = Path.Combine(context.HomeFolder, "site", "diagnostics");
            var settingsFile = Path.Combine(settingsFolder, "settings.json");

            // TODO: This is a workaround because the file provider doesn't handle missing folders/files
            Directory.CreateDirectory(settingsFolder);
            if (!File.Exists(settingsFile))
            {
                File.WriteAllText(settingsFile, "{}");
            }

            return new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile(settingsFile, optional: true, reloadOnChange: true)
                .Build();
        }
    }
}
