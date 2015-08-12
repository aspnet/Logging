using System;
using Microsoft.Framework.Internal;
using NLog;

namespace Microsoft.Framework.Logging.NLog
{
    public class NLogLogger : ILogger
    {
        private readonly global::NLog.Logger _logger;

        public NLogLogger(global::NLog.Logger logger)
        {
            _logger = logger;
        }

        public virtual void Log(
            LogLevel logLevel,
            int eventId,
            object state,
            Exception exception,
            Func<object, Exception, string> formatter)
        {
            var nLogLogLevel = GetLogLevel(logLevel);
            var message = string.Empty;
            if (formatter != null)
            {
                message = formatter(state, exception);
            }
            else
            {
                message = LogFormatter.Formatter(state, exception);
            }
            if (!string.IsNullOrEmpty(message))
            {
                var eventInfo = LogEventInfo.Create(nLogLogLevel, _logger.Name, message, exception);
                eventInfo.Properties["EventId"] = eventId;
                _logger.Log(eventInfo);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(GetLogLevel(logLevel));
        }

        private global::NLog.LogLevel GetLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Verbose: return global::NLog.LogLevel.Trace;
                case LogLevel.Debug: return global::NLog.LogLevel.Debug;
                case LogLevel.Information: return global::NLog.LogLevel.Info;
                case LogLevel.Warning: return global::NLog.LogLevel.Warn;
                case LogLevel.Error: return global::NLog.LogLevel.Error;
                case LogLevel.Critical: return global::NLog.LogLevel.Fatal;
            }
            return global::NLog.LogLevel.Debug;
        }

        public virtual IDisposable BeginScopeImpl([NotNull] object state)
        {
            return NestedDiagnosticsContext.Push(state.ToString());
        }
    }
}
