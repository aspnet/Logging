using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Logging
{
    public interface ILoggerBuilder
    {
        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> where Logging services are configured.
        /// </summary>
        IServiceCollection Services { get; }
    }

    public class LoggerBuilder : ILoggerBuilder
    {
        public LoggerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}