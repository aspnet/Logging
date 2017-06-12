using System.IO;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.AzureAppServices.Internal
{
    public class AzureFileLoggerConfigureOptions : IConfigureOptions<AzureDiagnosticsFileLoggerOptions>
    {
        private readonly WebAppContext _context;

        public AzureFileLoggerConfigureOptions(WebAppContext context)
        {
            _context = context;
        }

        public void Configure(AzureDiagnosticsFileLoggerOptions options)
        {
            options.LogDirectory = Path.Combine(_context.HomeFolder, "LogFiles", "Application");
        }
    }
}