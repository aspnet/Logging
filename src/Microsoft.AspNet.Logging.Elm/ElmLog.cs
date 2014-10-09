using System;
using System.Collections.Generic;

namespace Microsoft.Framework.Logging.Elm
{
    public static class ElmLog
    {
        static ElmLog()
        {
            Log = new List<string>();
        }

        public static List<string> Log { get; set; }
    }
}