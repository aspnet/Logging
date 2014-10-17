// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Logging.Elm
{
    public interface IElmStore
    {
        void Add(LogInfo info);
        IEnumerable<LogInfo> GetLogs();
        IEnumerable<LogInfo> GetLogs(TraceType minLevel);
    }
}