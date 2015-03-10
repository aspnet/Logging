using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Extensions;
using System;
using System.Diagnostics;
using System.Security;
using System.Text;

namespace Microsoft.Framework.Logging.EventLog
{
    public class WindowsEventLogLogger : ILogger
    {

        private readonly string _name;

        public WindowsEventLogLogger(string name, IHttpContextAccessor httpContextAccessor, Action<Exception, HttpContext, StringBuilder> extraContextInformationLogger)
        {
            _name = name;
            _httpContextAccessor = httpContextAccessor;
            _extraContextInformationLogger = extraContextInformationLogger;
        }

        const string SourceName = "ASP.NET 5";

        private static bool _knownRegistered = false;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Action<Exception, HttpContext, StringBuilder> _extraContextInformationLogger;

        public static bool TryRegisterEventLogSource(bool throwOnEventSourceRegistrationFailure)
        {
            if (_knownRegistered) return true;

            var knownExists = false;

            try
            {
                knownExists = System.Diagnostics.EventLog.SourceExists(SourceName);
                if (knownExists)
                {
                    _knownRegistered = true;
                    return true;
                } else
                {
                    System.Diagnostics.EventLog.CreateEventSource(SourceName, "Application");
                    _knownRegistered = true;
                    return true;
                }
            } catch (SecurityException)
            {
                if (throwOnEventSourceRegistrationFailure)
                {
                    throw;
                }
            }

            return false;
        }

        public IDisposable BeginScope(object state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= LogLevel.Error;
        }

        public void Write(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (exception == null)
            {
                return;
            }

            var sb = new StringBuilder();

            sb.AppendLine("ASP.NET 5 Exception: ");
            sb.AppendLine();

            if (_httpContextAccessor != null)
            {
                var context = _httpContextAccessor.Value;
                if (context != null)
                {
                    AppendHttpContextInformation(exception, context, sb);
                }
            }

            sb.Append("Log level: ");
            sb.AppendLine(logLevel.ToString());
            sb.AppendLine();

            sb.AppendLine("Exception: ");
            sb.Append(LogFormatter.Formatter(state, exception));

            var message = sb.ToString();

            System.Diagnostics.EventLog.WriteEntry(SourceName, message, EventLogEntryType.Error, eventID);
        }

        const int eventID = 13090;

        private void AppendHttpContextInformation(Exception exception, HttpContext context, StringBuilder sb)
        {
            if (context.Request != null)
            {
                var u = new UriHelper(context.Request);
                var fullUri = u.GetFullUri();
                sb.Append("Request URL: ");
                sb.AppendLine(fullUri);
            }
            if (_extraContextInformationLogger != null)
            {
                _extraContextInformationLogger(exception, context, sb);
            }
        }
    }
}