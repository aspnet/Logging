using System;

namespace Microsoft.Framework.Logging.Console
{
    public interface IConsole
    {
        ConsoleColor BackgroundColor { get; set; }
        ConsoleColor ForegroundColor { get; set; }
        void WriteLine(string format, object arg0, object arg1, object arg2);
    }
}