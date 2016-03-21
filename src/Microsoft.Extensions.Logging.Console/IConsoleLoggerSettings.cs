using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Logging.Console
{
    public interface IConsoleLoggerSettings
    {
        bool IncludeScopes { get; }

        IChangeMonitor<IConsoleLoggerSettings> Monitor { get; }

        bool TryGetSwitch(string name, out LogLevel level);
    }
}
