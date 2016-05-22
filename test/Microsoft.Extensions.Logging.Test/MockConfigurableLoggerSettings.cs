// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Logging
{
    class MockConfigurableLoggerSettings : IConfigurableLoggerSettings
    {
        public CancellationTokenSource Cancel { get; set; }

        public IChangeToken ChangeToken => new CancellationChangeToken(Cancel.Token);

        public IDictionary<string, LogLevel> Switches { get; } = new Dictionary<string, LogLevel>();

        public bool IncludeScopes { get; set; }

        public IConfigurableLoggerSettings Reload()
        {
            return this;
        }

        public bool TryGetSwitch(string name, out LogLevel level)
        {
            return Switches.TryGetValue(name, out level);
        }
    }
}
