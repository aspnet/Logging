using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Logging.Test
{
    public static class TestLoggerBuilder
    {
        public static ILoggerBuilder Create(IConfiguration configuration = null)
        {
            var serviceCollection = new ServiceCollection();
            if (configuration != null)
            {
                return serviceCollection.AddLogging(configuration);
            }
            else
            {
                return serviceCollection.AddLogging();
            }
        }
    }

    public static class LoggerBuilderTestExtensions
    {
        public static ILoggerFactory Build(this ILoggerBuilder builder)
        {
            return builder.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        }
    }
}
