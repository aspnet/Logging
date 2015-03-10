using System;
using System.Security;
using System.Diagnostics;
using System.Text;
using Microsoft.Framework.DependencyInjection;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Extensions;
using Microsoft.AspNet.Hosting;

namespace Microsoft.Framework.Logging.EventLog
{
    public class WindowsEventLogLoggerProvider : ILoggerProvider
    {
        private readonly bool _doNotThrowOnEventSourceRegistrationFailure;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Action<Exception, HttpContext, StringBuilder> _extraContextInformationLogger;

        public WindowsEventLogLoggerProvider(bool doNotThrowOnEventSourceRegistrationFailure, IHttpContextAccessor httpContextAccessor, Action<Exception, HttpContext, StringBuilder> extraContextInformationLogger)
        {
            _doNotThrowOnEventSourceRegistrationFailure = doNotThrowOnEventSourceRegistrationFailure;
            _httpContextAccessor = httpContextAccessor;
            _extraContextInformationLogger = extraContextInformationLogger;
        }

        public ILogger Create(string name)
        {
            if (WindowsEventLogLogger.TryRegisterEventLogSource(!_doNotThrowOnEventSourceRegistrationFailure))
            {
                return new WindowsEventLogLogger(name, _httpContextAccessor, _extraContextInformationLogger);
            } else
            {
                return null;
            }
        }
    }

}
