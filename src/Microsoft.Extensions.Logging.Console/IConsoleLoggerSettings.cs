using System;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Logging.Console
{
    [Obsolete("Use ConsoleLoggerOptions")]
    public interface IConsoleLoggerSettings
    {
        bool IncludeScopes { get; }

        IChangeToken ChangeToken { get; }

        bool TryGetSwitch(string name, out LogLevel level);

        IConsoleLoggerSettings Reload();
    }
}
