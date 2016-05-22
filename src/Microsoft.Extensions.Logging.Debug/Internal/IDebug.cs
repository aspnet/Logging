// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.Logging.Debug.Internal
{
    public interface IDebug
    {
        bool IsAttached { get; }
        void Write(string message, string name);
        void WriteLine(string message, string name);
    }
}
