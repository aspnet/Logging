using System;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Elm;

namespace Microsoft.AspNet.Logging.Elm
{
    public class ElmLogger : ILogger
    {
        // TODO: allow filtering by name and TraceType

        public void Write(TraceType traceType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            var message = string.Empty;
            if (formatter != null)
            {
                message = formatter(state, exception);
            }
            else
            {
                if (state != null)
                {
                    message += state;
                }
                if (exception != null)
                {
                    message += Environment.NewLine + exception;
                }
            }
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            // TODO: write to a database instead, and write more interesting information
            ElmLog.Log.Add(string.Format("[{0}]: {1}", traceType, message));
        }

        public bool IsEnabled(TraceType traceType)
        {
            return true;
        }

        public IDisposable BeginScope(object state)
        {
            throw new NotImplementedException();
        }
    }
}