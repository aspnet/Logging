using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.AzureAppServices.Internal
{
    public class AzureFileLoggerConfigureOptions : AzureBatchLoggerConfigureOptions, IConfigureOptions<AzureDiagnosticsFileLoggerOptions>
    {
        private readonly IWebAppContext _context;

        public AzureFileLoggerConfigureOptions(IConfiguration configuration, IWebAppContext context)
            : base(configuration, "AzureDriveEnabled")
        {
            _context = context;
        }

        public void Configure(AzureDiagnosticsFileLoggerOptions options)
        {
            options.LogDirectory = Path.Combine(_context.HomeFolder, "LogFiles", "Application");
        }
    }

    public class AzureBatchLoggerConfigureOptions : IConfigureOptions<BatchingLoggerOptions>
    {
        private readonly IConfiguration _configuration;
        private readonly string _isEnabledKey;

        public AzureBatchLoggerConfigureOptions(IConfiguration configuration, string isEnabledKey)
        {
            _configuration = configuration;
            _isEnabledKey = isEnabledKey;
        }

        public void Configure(BatchingLoggerOptions options)
        {
            options.IsEnabled = TextToBoolean(_configuration.GetSection(_isEnabledKey)?.Value);
        }

        private static bool TextToBoolean(string text)
        {
            bool result;
            if (string.IsNullOrEmpty(text) ||
                !bool.TryParse(text, out result))
            {
                result = false;
            }

            return result;
        }
    }
}