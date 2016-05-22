// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if NET451
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Logging
{
    public class MockEventLogSettings : EventLogSettings
    {
        public CancellationTokenSource Cancel { get; set; }

        public override IChangeToken ChangeToken => new CancellationChangeToken(Cancel.Token);

        public IDictionary<string, LogLevel> Switches { get; } = new Dictionary<string, LogLevel>();

        public override IConfigurableLoggerSettings Reload()
        {
            return this;
        }

        public override bool TryGetSwitch(string name, out LogLevel level)
        {
            return Switches.TryGetValue(name, out level);
        }
    }
}
#endif