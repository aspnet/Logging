using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Logging.Observer
{
    public class LoggerObserver : IObserver<KeyValuePair<string, object>>
    {
        ILogger logger;
        List<KeyValuePair<string, IDisposable>> scopes = new List<KeyValuePair<string, IDisposable>>();

        public LoggerObserver(ILogger logger)
        {
            this.logger = logger;
        }
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            var loggerArguments = value.Value as LoggerArguments;
            if (loggerArguments != null)
            {
                System.Diagnostics.Tracing.LogLevel logLevel = loggerArguments.Level;
                if (!IsEnabled(logLevel))
                {
                    return;
                }

                string logItemName = value.Key;

                string loggerName = loggerArguments.LoggerName;
                string payloadStr = ConsoleObserver.GetPayload(loggerArguments.Arguments);

                if (string.IsNullOrEmpty(payloadStr))
                {
                    return;
                }

                if (logItemName.EndsWith(".Start"))
                {
                    IDisposable d = logger.BeginScope(payloadStr);
                    string v = logItemName.Substring(0, logItemName.Length - 6);
                    scopes.Add(new KeyValuePair<string, IDisposable>(v, d));
                    return;
                }

                if (logItemName.EndsWith(".Stop"))
                {
                    string v = logItemName.Substring(0, logItemName.Length - 5);
                    var kvp = scopes.FindLast(x => x.Key == v);
                    kvp.Value.Dispose();
                    return;
                }

                switch (logLevel)
                {
                    case System.Diagnostics.Tracing.LogLevel.LogAlways:
                    case System.Diagnostics.Tracing.LogLevel.Verbose:
                        logger.LogVerbose(payloadStr);
                        break;
                    case System.Diagnostics.Tracing.LogLevel.Informational:
                        logger.LogInformation(payloadStr);
                        break;
                    case System.Diagnostics.Tracing.LogLevel.Warning:
                        logger.LogWarning(payloadStr);
                        break;
                    case System.Diagnostics.Tracing.LogLevel.Error:
                        logger.LogError(payloadStr);
                        break;
                    case System.Diagnostics.Tracing.LogLevel.Critical:
                        logger.LogCritical(payloadStr);
                        break;
                }
            }
        }

        public bool IsEnabled(System.Diagnostics.Tracing.LogLevel logLevel)
        {
            LogLevel mLogLevel = GetMFLlogLevel(logLevel);
            return logger.IsEnabled(mLogLevel);
        }

        internal LogLevel GetMFLlogLevel(System.Diagnostics.Tracing.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case System.Diagnostics.Tracing.LogLevel.LogAlways:
                case System.Diagnostics.Tracing.LogLevel.Verbose:
                    return LogLevel.Verbose;
                case System.Diagnostics.Tracing.LogLevel.Informational:
                    return LogLevel.Information;
                case System.Diagnostics.Tracing.LogLevel.Warning:
                    return LogLevel.Warning;
                case System.Diagnostics.Tracing.LogLevel.Error:
                    return LogLevel.Error;
                case System.Diagnostics.Tracing.LogLevel.Critical:
                    return LogLevel.Critical;
                default:
                    return LogLevel.Verbose;
            }
        }
    }
}
