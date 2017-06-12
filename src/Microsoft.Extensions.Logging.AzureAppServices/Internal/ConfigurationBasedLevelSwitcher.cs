using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.AzureAppServices.Internal
{
    public class ConfigurationBasedLevelSwitcher: IConfigureOptions<LoggerFilterOptions>
    {
        private readonly IConfiguration _configuration;
        private readonly Type _provider;
        private readonly string _levelKey;
        private readonly string _enableKey;



        //.SetFileLoggingEnabled(TextToBoolean(_configuration.GetSection("AzureDriveEnabled")?.Value))
        //.SetFileLoggingLevel(TextToLogLevel(_configuration.GetSection("AzureDriveTraceLevel")?.Value))
        //.SetFileLoggingFolder(_fileLogFolder)
        //.SetBlobLoggingEnabled(TextToBoolean(_configuration.GetSection("AzureBlobEnabled")?.Value))
        //.SetBlobLoggingLevel(TextToLogLevel(_configuration.GetSection("AzureBlobTraceLevel")?.Value))


        public ConfigurationBasedLevelSwitcher(IConfiguration configuration, Type provider, string levelKey, string enableKey)
        {
            _configuration = configuration;
            _provider = provider;
            _levelKey = levelKey;
            _enableKey = enableKey;
        }

        public void Configure(LoggerFilterOptions options)
        {
            options.Rules.Add(new LoggerFilterRule(_provider.FullName, null, GetLogLevel(), null));
        }

        private LogLevel? GetLogLevel()
        {
            if (TextToBoolean(_configuration.GetSection(_enableKey)?.Value))
            {
                return TextToLogLevel(_configuration.GetSection(_levelKey)?.Value);
            }

            return LogLevel.None;
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

        private static LogLevel TextToLogLevel(string text)
        {
            switch (text?.ToUpperInvariant())
            {
                case "ERROR":
                    return LogLevel.Error;
                case "WARNING":
                    return LogLevel.Warning;
                case "INFORMATION":
                    return LogLevel.Information;
                case "VERBOSE":
                    return LogLevel.Trace;
                default:
                    return LogLevel.None;
            }
        }
    }
}