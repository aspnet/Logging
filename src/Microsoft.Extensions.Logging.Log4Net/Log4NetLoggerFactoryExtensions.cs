//using log4net.Repository.Hierarchy;

namespace Microsoft.Extensions.Logging.Log4Net
{
    public static class Log4NetLoggerFactoryExtensions
    {
        public static ILoggerFactory AddLog4Net(
            this ILoggerFactory factory,
            global::log4net.Core.ILogger logFactory)
        {
            factory.AddProvider(new Log4Net.Log4NetLoggerProvider(logFactory));
            return factory;
        }
    }
}
