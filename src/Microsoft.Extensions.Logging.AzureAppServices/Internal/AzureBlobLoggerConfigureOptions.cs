using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.AzureAppServices.Internal
{
    public class AzureBlobLoggerConfigureOptions : IConfigureOptions<AzureDiagnosticsBlobLoggerOptions>
    {
        private readonly IConfiguration _configuration;

        public AzureBlobLoggerConfigureOptions(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(AzureDiagnosticsBlobLoggerOptions options)
        {
            options.ContainerUrl = _configuration.GetSection("APPSETTING_DIAGNOSTICS_AZUREBLOBCONTAINERSASURL")?.Value;
        }
    }
}