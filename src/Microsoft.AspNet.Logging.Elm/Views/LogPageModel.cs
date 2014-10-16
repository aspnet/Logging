using System.Collections.Generic;

namespace Microsoft.AspNet.Logging.Elm.Views
{
    public class LogPageModel
    {
        public IEnumerable<LogInfo> Logs { get; set; }
    }
}