// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Logging.Elm
{
    public class LogInfo
    {
        public LogContext Context { get; set; }

        public string Name { get; set; }

        public object State { get; set; }

        public Exception Exception { get; set; }

        public TraceType Severity { get; set; }

        public int EventID { get; set; }

        public DateTime Time { get; set; }
    }
}