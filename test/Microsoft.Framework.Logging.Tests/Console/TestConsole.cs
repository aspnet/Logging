// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Framework.Logging.Console.Internal;

namespace Microsoft.Framework.Logging.Tests.Console
{
    public class TestConsole : IConsole
    {
        private ConsoleSink _sink;
        
        public TestConsole(ConsoleSink sink)
        {
            _sink = sink;
        }

        public ConsoleColor BackgroundColor { get; set; }

        public ConsoleColor ForegroundColor { get; set; }

        public void ResetColor()
        {
            System.Console.ResetColor();
        }

        public void WriteLine(string message)
        {
            _sink.Write(new ConsoleContext()
            {
                ForegroundColor = ForegroundColor,
                BackgroundColor = BackgroundColor,
                Message = message
            });
        }
    }
}