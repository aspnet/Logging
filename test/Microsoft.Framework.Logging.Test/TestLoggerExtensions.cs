using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Framework.Logging.Test
{
    public static class TestLoggerExtensions
    {
        public class ActionMatchedWithEventNameInfo
        {
            public static Action<ILogger, string, string, Exception> MessageDelegate;
            public const string EventName = "ActionMatch";
            public const string NamedStringFormat = "Request matched controller '{controller}' and action '{action}'.";
            public const string NamedStringFormatWithEventName = "{EventName}: Request matched controller '{controller}' and action '{action}'.";
            public const string FormatString = "{0}: Request matched controller '{1}' and action '{2}'.";

            static ActionMatchedWithEventNameInfo()
            {
                LoggerMessage.Define(
                    out MessageDelegate,
                    LogLevel.Information,
                    eventId: 1,
                    eventName: "ActionMatch",
                    formatString: NamedStringFormat);
            }
        }

        public class ActionMatchedWithoutEventNameInfo
        {
            public static Action<ILogger, string, string, Exception> MessageDelegate;
            public const string NamedStringFormat = "Request matched controller '{controller}' and action '{action}'.";
            public const string FormatString = "Request matched controller '{0}' and action '{1}'.";

            static ActionMatchedWithoutEventNameInfo()
            {
                LoggerMessage.Define(
                    out MessageDelegate,
                    LogLevel.Information,
                    eventId: 1,
                    formatString: NamedStringFormat);
            }
        }

        public class ScopeWithOneParameter
        {
            public static Func<ILogger, string, IDisposable> ScopeDelegate;
            public const string NamedStringFormat = "RequestId: {RequestId}";
            public const string FormatString = "RequestId: {0}";

            static ScopeWithOneParameter()
            {
                LoggerMessage.DefineScope(out ScopeDelegate, NamedStringFormat);
            }
        }

        public class ScopeInfoWithTwoParameters
        {
            public static Func<ILogger, string, string, IDisposable> ScopeDelegate;
            public const string NamedStringFormat = "{param1}, {param2}";
            public const string FormatString = "{0}, {1}";

            static ScopeInfoWithTwoParameters()
            {
                LoggerMessage.DefineScope(out ScopeDelegate, NamedStringFormat);
            }
        }

        public class ScopeInfoWithThreeParameters
        {
            public static Func<ILogger, string, string, int, IDisposable> ScopeDelegate;
            public const string NamedStringFormat = "{param1}, {param2}, {param3}";
            public const string FormatString = "{0}, {1}, {2}";

            static ScopeInfoWithThreeParameters()
            {
                LoggerMessage.DefineScope(out ScopeDelegate, NamedStringFormat);
            }
        }

        //-------------

        public static void ActionMatchedWithEventName(
            this ILogger logger, string controller, string action, Exception exception = null)
        {
            ActionMatchedWithEventNameInfo.MessageDelegate(logger, controller, action, exception);
        }

        public static void ActionMatchedWithoutEventName(
            this ILogger logger, string controller, string action, Exception exception = null)
        {
            ActionMatchedWithoutEventNameInfo.MessageDelegate(logger, controller, action, exception);
        }

        public static IDisposable ScopeWithOneParam(this ILogger logger, string requestId)
        {
            return ScopeWithOneParameter.ScopeDelegate(logger, requestId);
        }

        public static IDisposable ScopeWithTwoParams(this ILogger logger, string param1, string param2)
        {
            return ScopeInfoWithTwoParameters.ScopeDelegate(logger, param1, param2);
        }

        public static IDisposable ScopeWithThreeParams(this ILogger logger, string param1, string param2, int param3)
        {
            return ScopeInfoWithThreeParameters.ScopeDelegate(logger, param1, param2, param3);
        }
    }
}
