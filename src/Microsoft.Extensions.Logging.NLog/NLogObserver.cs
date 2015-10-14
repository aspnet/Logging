using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using NLog;
using LogLevel = System.Diagnostics.Tracing.LogLevel;

namespace Microsoft.Extensions.Logging.NLog
{
    public class NLogObserver : IObserver<KeyValuePair<string, object>>, IDisposable
    {
        public NLogObserver(global::NLog.Logger logger)
        {
            _logger = logger;
        }
        public void OnCompleted()
        {
            //TODO
        }
        public void OnError(Exception error)
        {
            // TODO
        }
        public void OnNext(KeyValuePair<string, object> value)
        {
            var loggerArguments = value.Value as LoggerArguments;
            if (loggerArguments != null)
            {
                LogLevel logLevel = loggerArguments.Level;
                if (!IsEnabled(logLevel))
                {
                    return;
                }
                var nLogLogLevel = GetLogLevel(logLevel);
                string logItemName = value.Key;

                string payloadStr = loggerArguments.Arguments.PrintData();

                if (string.IsNullOrEmpty(payloadStr))
                {
                    return;
                }
                var eventInfo = LogEventInfo.Create(nLogLogLevel, _logger.Name, payloadStr);
                eventInfo.Properties["EventId"] = logItemName;
                _logger.Log(eventInfo);
            }

        }

        public bool IsEnabled(LogLevel logLevel)
        {
            var level = GetLogLevel(logLevel);
            return _logger.IsEnabled(level);
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        private global::NLog.LogLevel GetLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Verbose: return global::NLog.LogLevel.Trace;
                case LogLevel.Debug: return global::NLog.LogLevel.Debug;
                case LogLevel.Informational: return global::NLog.LogLevel.Info;
                case LogLevel.Warning: return global::NLog.LogLevel.Warn;
                case LogLevel.Error: return global::NLog.LogLevel.Error;
                case LogLevel.Critical: return global::NLog.LogLevel.Fatal;
            }
            return global::NLog.LogLevel.Debug;
        }
        private readonly global::NLog.Logger _logger;
    }
}