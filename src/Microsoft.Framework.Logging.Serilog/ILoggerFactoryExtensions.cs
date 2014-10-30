using Serilog;
using Microsoft.Framework.Logging.Serilog;

namespace Microsoft.Framework.Logging
{
    public static class ILoggerFactoryExtensions
    {
        public static ILoggerFactory AddSerilog(this ILoggerFactory factory, LoggerConfiguration loggerConfiguration)
        {
            Check.NotNull(factory, "factory");
            Check.NotNull(loggerConfiguration, "loggerConfiguration");

            factory.AddProvider(new SerilogLoggerProvider(loggerConfiguration));

            return factory;
        }
    }
}