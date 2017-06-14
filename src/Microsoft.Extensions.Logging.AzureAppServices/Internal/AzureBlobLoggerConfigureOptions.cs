using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.AzureAppServices.Internal
{
    public class AzureBlobLoggerConfigureOptions : AzureBatchLoggerConfigureOptions, IConfigureOptions<AzureDiagnosticsBlobLoggerOptions>
    {
        private readonly IConfiguration _configuration;
        private readonly IWebAppContext _context;

        public AzureBlobLoggerConfigureOptions(IConfiguration configuration, IWebAppContext context)
            : base(configuration, "AzureBlobEnabled")
        {
            _configuration = configuration;
            _context = context;
        }

        public void Configure(AzureDiagnosticsBlobLoggerOptions options)
        {
            base.Configure(options);
            options.ContainerUrl = _configuration.GetSection("APPSETTING_DIAGNOSTICS_AZUREBLOBCONTAINERSASURL")?.Value;
            options.ApplicationName = _context.SiteName;
            options.ApplicationInstanceId = _context.SiteInstanceId;
        }
    }
}