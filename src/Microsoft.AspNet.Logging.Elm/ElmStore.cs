// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.AspNet.Logging.Elm
{
    public class ElmStore : IElmStore
    {
        private static List<LogInfo> Log { get; set; } = new List<LogInfo>();

        public IEnumerable<LogInfo> GetLogs()
        {
            return Log;
        }

        public void Write(LogInfo info)
        {
            Log.Add(info);
        }
    }
}