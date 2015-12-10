using log4net;
using System;

namespace Microsoft.Extensions.Logging.Log4Net
{
    public class Log4NetLoggerProvider : ILoggerProvider
    {

        private readonly global::log4net.Core.ILogger _logFactory;

        public Log4NetLoggerProvider(global::log4net.Core.ILogger logFactory)
        {

            _logFactory = logFactory;
        }

        public ILogger CreateLogger(string name)
        {
            return new Logger(_logFactory);
        }


        private class Logger : ILogger
        {
            private readonly global::log4net.Core.ILogger _logger;

            public Logger(global::log4net.Core.ILogger logger)
            {
                _logger = logger;
            }

            public IDisposable BeginScopeImpl(object state)
            {
                if (state == null)
                {
                    throw new ArgumentNullException(nameof(state));
                }

                return NDC.Push(state.ToString());
            }

            /// <summary>
            /// 是否开启记录log事件
            /// </summary>
            /// <param name="logLevel"></param>
            /// <returns></returns>
            public bool IsEnabled(LogLevel logLevel)
            {
                return _logger.IsEnabledFor(GetLogLevel(logLevel));
            }

            /// <summary>
            /// Log实现
            /// </summary>
            /// <param name="logLevel"></param>
            /// <param name="eventId"></param>
            /// <param name="state"></param>
            /// <param name="exception"></param>
            /// <param name="formatter"></param>
            public void Log(
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
                    ///new System.Diagnostics.StackTrace().GetFrame(1).GetType()   GetFrame获取调用类
                    _logger.Log(new System.Diagnostics.StackTrace().GetFrame(1).GetType(), nLogLogLevel, message, exception);

                }
            }

            /// <summary>
            /// 微软logLevel和log4net的logLevel适配
            /// </summary>
            /// <param name="logLevel"></param>
            /// <returns></returns>
            private global::log4net.Core.Level GetLogLevel(LogLevel logLevel)
            {
                switch (logLevel)
                {
                    case LogLevel.Trace: return global::log4net.Core.Level.Trace;
                    case LogLevel.Debug: return global::log4net.Core.Level.Debug;
                    case LogLevel.Information: return global::log4net.Core.Level.Info;
                    case LogLevel.Warning: return global::log4net.Core.Level.Warn;
                    case LogLevel.Error: return global::log4net.Core.Level.Error;
                    case LogLevel.Critical: return global::log4net.Core.Level.Fatal;
                }
                return global::log4net.Core.Level.Debug;
            }

        }

        public void Dispose()
        {
        }

    }
}
