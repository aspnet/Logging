using Serilog;
using Microsoft.Framework.Logging.Serilog;

namespace Microsoft.Framework.Logging
{
    public static class ILoggerFactoryExtensions
    {
        public static ILoggerFactory AddSerilog(
            [NotNull] this ILoggerFactory factory, [NotNull] LoggerConfiguration loggerConfiguration)
        {
            factory.AddProvider(new SerilogLoggerProvider(loggerConfiguration));

            return factory;
        }
    }
}