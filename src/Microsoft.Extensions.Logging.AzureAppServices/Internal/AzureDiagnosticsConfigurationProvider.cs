using System.IO;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Logging.AzureAppServices.Internal
{
    public class AzureDiagnosticsConfigurationProvider
    {
        public static IConfiguration GetAzureLoggingConfiguration(IWebAppContext context)
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