using System;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Logging.Elm
{
    public class ElmLoggerProvider : ILoggerProvider
    {
        public ILogger Create(string name)
        {
            return new ElmLogger();
        }
    }
}
