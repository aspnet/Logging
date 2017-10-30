// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging.Console.Internal;

namespace Microsoft.Extensions.Logging.Test.Console
{
    public class TestConsole : IConsole
    {
        public static readonly ConsoleColor? DefaultBackgroundColor;
        public static readonly ConsoleColor? DefaultForegroundColor;

        private ConsoleSink _sink;

        public TestConsole(ConsoleSink sink)
        {
            _sink = sink;
            BackgroundColor = DefaultBackgroundColor;
            ForegroundColor = DefaultForegroundColor;
        }

        public ConsoleColor? BackgroundColor { get; private set; }

        public ConsoleColor? ForegroundColor { get; private set; }

        public void Write(string message, ConsoleColor? background, ConsoleColor? foreground, bool toErrorStream = false)
        {
            var consoleContext = new ConsoleContext();

            if (toErrorStream)
            {
                consoleContext.Error = message;
            }
            else
            {
                consoleContext.Message = message;
            }

            if (background.HasValue)
            {
                consoleContext.BackgroundColor = background.Value;
            }

            if (foreground.HasValue)
            {
                consoleContext.ForegroundColor = foreground.Value;
            }

            _sink.Write(consoleContext);

            ResetColor();
        }

        public void WriteLine(string message, ConsoleColor? background, ConsoleColor? foreground, bool toErrorStream = false)
        {
            Write(message + Environment.NewLine, background, foreground, toErrorStream);
        }

        public void Flush()
        {
        }

        private void ResetColor()
        {
            BackgroundColor = DefaultBackgroundColor;
            ForegroundColor = DefaultForegroundColor;
        }
    }
}