// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Extensions.Logging.Debug.Internal;

namespace Microsoft.Extensions.Logging.Test.Debug
{
    public class TestDebug : IDebug
    {
        public List<string> Messages { get; } = new List<string>();

        public bool IsAttached => true;

        public void Write(string message, string name)
        {
            Messages.Add(message);
        }

        public void WriteLine(string message, string name)
        {
            Messages.Add(message);
        }
    }
}
