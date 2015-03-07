using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using System;
using System.Text;

namespace Microsoft.Framework.Logging.EventLog {
    public static class WindowsEventLogLoggerExtensions {
        /// <summary>
        /// Adds a Windows event log logger that is enabled for <see cref="LogLevel"/>.Error or higher and will only log entries with associated exceptions. (If the event source needs to be registered and the host does not run under administrator privileges, the event log is not added.)
        /// </summary>
        public static ILoggerFactory AddWindowsEventLogIfHasPermission(this ILoggerFactory factory) {
            factory.AddProvider(new WindowsEventLogLoggerProvider(doNotThrowOnEventSourceRegistrationFailure: true, httpContextAccessor: null, extraContextInformationLogger:null));
            return factory;
        }
        /// <summary>
        /// Adds a Windows event log logger that is enabled for <see cref="LogLevel"/>.Error or higher, will only log entries with associated exceptions and will attempt to log information about the active <see cref="HttpContext"/>.
        /// </summary>
        public static ILoggerFactory AddWindowsEventLogIfHasPermission(this ILoggerFactory factory, IHttpContextAccessor httpContextAccessor, Action<Exception, HttpContext, StringBuilder> eventLogLoggerExtraContext = null) {
            factory.AddProvider(new WindowsEventLogLoggerProvider(doNotThrowOnEventSourceRegistrationFailure: true, httpContextAccessor: httpContextAccessor, extraContextInformationLogger: eventLogLoggerExtraContext));
            return factory;
        }
        /// <summary>
        /// Adds a Windows event log logger that is enabled for <see cref="LogLevel"/>.Error or higher and will only log entries with associated exceptions. (If the event source needs to be registered and the host does not run under administrator privileges, the <see cref="System.Security.SecurityException" /> is propagated.)
        /// </summary>
        public static ILoggerFactory AddWindowsEventLogThrowIfMissingPermission(this ILoggerFactory factory) {
            factory.AddProvider(new WindowsEventLogLoggerProvider(doNotThrowOnEventSourceRegistrationFailure: false, httpContextAccessor: null, extraContextInformationLogger: null));
            return factory;
        }
        /// <summary>
        /// Adds a Windows event log logger that is enabled for <see cref="LogLevel"/>.Error or higher, will only log entries with associated exceptions and will attempt to log information about the active <see cref="HttpContext"/>. (If the event source needs to be registered and the host does not run under administrator privileges, the <see cref="System.Security.SecurityException" /> is propagated.)
        /// </summary>
        public static ILoggerFactory AddWindowsEventLogThrowIfMissingPermission(this ILoggerFactory factory, IHttpContextAccessor httpContextAccessor, Action<Exception, HttpContext, StringBuilder> eventLogLoggerExtraContext = null) {
            factory.AddProvider(new WindowsEventLogLoggerProvider(doNotThrowOnEventSourceRegistrationFailure: false, httpContextAccessor: httpContextAccessor, extraContextInformationLogger: eventLogLoggerExtraContext));
            return factory;
        }
    }
}