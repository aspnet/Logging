// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Logging.Console.Internal
{
    public interface IConsole
    {
        void Write(string message, ConsoleColor? background, ConsoleColor? foreground, bool toErrorStream = false);
        void WriteLine(string message, ConsoleColor? background, ConsoleColor? foreground, bool toErrorStream = false);
        void Flush();
    }
}