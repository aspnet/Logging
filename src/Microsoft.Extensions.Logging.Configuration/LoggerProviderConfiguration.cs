using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Logging
{
    internal class LoggerProviderConfiguration<T> : ILoggerProviderConfiguration<T>
    {
        public LoggerProviderConfiguration(ILoggerProviderConfiguration providerConfiguration)
        {
            Configuration = providerConfiguration.GetConfiguration(typeof(T));
        }

        public IConfiguration Configuration { get; }
    }

    internal class LoggerProviderConfiguration : ILoggerProviderConfiguration
    {
        private readonly IEnumerable<LoggingConfiguration> _configurations;

        public LoggerProviderConfiguration(IEnumerable<LoggingConfiguration> configurations)
        {
            _configurations = configurations;
        }

        private const string AliasAttibuteTypeFullName = "Microsoft.Extensions.Logging.ProviderAliasAttribute";
        private const string AliasAttibuteAliasProperty = "Alias";

        public IConfiguration GetConfiguration(Type providerType)
        {
            if (providerType == null)
            {
                throw new ArgumentNullException(nameof(providerType));
            }

            var fullName = providerType.FullName;
            var alias = GetAlias(providerType);
            var configurationBuilder = new ConfigurationBuilder();
            foreach (var configuration in _configurations)
            {
                var sectionFromFullName = configuration.Configuration.GetSection(fullName);
                if (sectionFromFullName.Exists())
                {
                    configurationBuilder.AddConfiguration(sectionFromFullName);
                }

                if (!string.IsNullOrWhiteSpace(alias))
                {
                    var sectionFromAlias = configuration.Configuration.GetSection(alias);
                    if (sectionFromAlias.Exists())
                    {
                        configurationBuilder.AddConfiguration(sectionFromAlias);
                    }
                }
            }
            return configurationBuilder.Build();
        }


        private string GetAlias(Type providerType)
        {
            foreach (var attribute in providerType.GetTypeInfo().GetCustomAttributes(inherit: false))
            {
                if (attribute.GetType().FullName == AliasAttibuteTypeFullName)
                {
                    var valueProperty = attribute
                        .GetType()
                        .GetProperty(AliasAttibuteAliasProperty, BindingFlags.Public | BindingFlags.Instance);

                    if (valueProperty != null)
                    {
                        return valueProperty.GetValue(attribute) as string;
                    }
                }
            }

            return null;
        }
    }
}