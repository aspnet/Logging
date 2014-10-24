// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Logging.Elm
{
    public class ElmStore : IElmStore
    {
        private readonly List<LogInfo> _logs = new List<LogInfo>();

        public IEnumerable<LogInfo> GetLogs()
        {
            return _logs;
        }

        public IEnumerable<LogInfo> GetLogs(TraceType minLevel)
        {
            return _logs.Where(l => l.Severity >= minLevel);
        }

        public void Add(LogInfo info)
        {
            _logs.Add(info);
        }
    }
}