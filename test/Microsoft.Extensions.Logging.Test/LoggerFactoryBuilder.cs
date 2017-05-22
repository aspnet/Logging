using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Logging.Test
{
    public class LoggerFactoryBuilder
    {
        private ServiceCollection _serviceCollection;

        public LoggerFactoryBuilder()
        {
            _serviceCollection = new ServiceCollection();
        }

        public static LoggerFactoryBuilder Create(IConfiguration configuration = null)
        {
            return new LoggerFactoryBuilder()
                .WithServices(collection =>
                {
                    if (configuration != null)
                    {
                        LoggingServiceCollectionExtensions.AddLogging(collection, configuration);
                    }
                    else
                    {
                        LoggingServiceCollectionExtensions.AddLogging(collection);
                    }
                });
        }

        public LoggerFactoryBuilder WithProvider(ILoggerProvider provider)
        {
            return WithServices(collection => ServiceCollectionServiceExtensions.AddSingleton(collection, provider));
        }

        public LoggerFactoryBuilder WithFilters(Action<LoggerFilterOptions> filterConfiguration)
        {
            OptionsServiceCollectionExtensions.Configure(_serviceCollection, filterConfiguration);
            return this;
        }

        public LoggerFactoryBuilder WithServices(Action<IServiceCollection> serviceConfiguration)
        {
            serviceConfiguration(_serviceCollection);
            return this;
        }

        public ILoggerFactory Build()
        {
            return ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(_serviceCollection).GetRequiredService<ILoggerFactory>();
        }
    }
}