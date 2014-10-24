using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.Logging.Elm.Views
{
    public class RequestPageModel
    {
        public Guid RequestID { get; set; }
        
        public IEnumerable<LogInfo> Logs { get; set; }

        public ElmOptions Options { get; set; }
    }
}