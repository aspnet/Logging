// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;

namespace Microsoft.Extensions.Logging.Console.Internal
{
    /// <summary>
    /// For non-Windows platform consoles which understand the ANSI escape code sequences to represent color
    /// </summary>
    public class AnsiLogConsole : IConsole
    {
        private readonly Output _output;
        private readonly IAnsiSystemConsole _systemConsole;

        public AnsiLogConsole(IAnsiSystemConsole systemConsole)
        {
            _output = new Output();
            _systemConsole = systemConsole;
        }

        public void Write(string message, ConsoleColor? background, ConsoleColor? foreground, bool toErrorStream = false)
        {
            // Order: backgroundcolor, foregroundcolor, Message, reset foregroundcolor, reset backgroundcolor
            if (background.HasValue)
            {
                _output.StringBuilder.Append(GetBackgroundColorEscapeCode(background.Value));
            }

            if (foreground.HasValue)
            {
                _output.StringBuilder.Append(GetForegroundColorEscapeCode(foreground.Value));
            }

            _output.StringBuilder.Append(message);

            if (foreground.HasValue)
            {
                _output.StringBuilder.Append("\x1B[39m\x1B[22m"); // reset to default foreground color
            }

            if (background.HasValue)
            {
                _output.StringBuilder.Append("\x1B[49m"); // reset to the background color
            }

            _output.ToErrorStream = toErrorStream;
        }

        public void WriteLine(string message, ConsoleColor? background, ConsoleColor? foreground, bool toErrorStream = false)
        {
            Write(message, background, foreground, toErrorStream);
            _output.StringBuilder.AppendLine();
            _output.ToErrorStream = toErrorStream;
        }

        public void Flush()
        {
            _systemConsole.Write(_output.StringBuilder.ToString(), _output.ToErrorStream);
            _output.StringBuilder.Clear();
        }

        private static string GetForegroundColorEscapeCode(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return "\x1B[30m";
                case ConsoleColor.DarkRed:
                    return "\x1B[31m";
                case ConsoleColor.DarkGreen:
                    return "\x1B[32m";
                case ConsoleColor.DarkYellow:
                    return "\x1B[33m";
                case ConsoleColor.DarkBlue:
                    return "\x1B[34m";
                case ConsoleColor.DarkMagenta:
                    return "\x1B[35m";
                case ConsoleColor.DarkCyan:
                    return "\x1B[36m";
                case ConsoleColor.Gray:
                    return "\x1B[37m";
                case ConsoleColor.Red:
                    return "\x1B[1m\x1B[31m";
                case ConsoleColor.Green:
                    return "\x1B[1m\x1B[32m";
                case ConsoleColor.Yellow:
                    return "\x1B[1m\x1B[33m";
                case ConsoleColor.Blue:
                    return "\x1B[1m\x1B[34m";
                case ConsoleColor.Magenta:
                    return "\x1B[1m\x1B[35m";
                case ConsoleColor.Cyan:
                    return "\x1B[1m\x1B[36m";
                case ConsoleColor.White:
                    return "\x1B[1m\x1B[37m";
                default:
                    return "\x1B[39m\x1B[22m"; // default foreground color
            }
        }

        private static string GetBackgroundColorEscapeCode(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return "\x1B[40m";
                case ConsoleColor.Red:
                    return "\x1B[41m";
                case ConsoleColor.Green:
                    return "\x1B[42m";
                case ConsoleColor.Yellow:
                    return "\x1B[43m";
                case ConsoleColor.Blue:
                    return "\x1B[44m";
                case ConsoleColor.Magenta:
                    return "\x1B[45m";
                case ConsoleColor.Cyan:
                    return "\x1B[46m";
                case ConsoleColor.White:
                    return "\x1B[47m";
                default:
                    return "\x1B[49m"; // Use default background color
            }
        }

        private class Output
        {
            public StringBuilder StringBuilder { get; } = new StringBuilder();
            public bool ToErrorStream { get; set; }
        }
    }
}
