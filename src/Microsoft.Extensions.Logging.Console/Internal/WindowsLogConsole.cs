// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Logging.Console.Internal
{
    public class WindowsLogConsole : IConsole
    {
        public void Write(string message, ConsoleColor? background, ConsoleColor? foreground)
        {
            var changedColor = false;
            if (background.HasValue)
            {
                System.Console.BackgroundColor = background.Value;
                changedColor = true;
            }

            if (foreground.HasValue)
            {
                System.Console.ForegroundColor = foreground.Value;
                changedColor = true;
            }

            System.Console.Write(message);

            if (changedColor)
            {
                System.Console.ResetColor();
            }
        }

        public void WriteLine(string message, ConsoleColor? background, ConsoleColor? foreground)
        {
            Write(message + Environment.NewLine, background, foreground);
        }

        public void Flush()
        {
            // No action required as for every write, data is sent directly to the console
            // output stream
        }
    }
}