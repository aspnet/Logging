using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Encapsulates System.Diagnostics.Tracing.Logger for DI
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public class Logger<T> : ILogger<T>
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new Logger
        /// </summary>
        /// <param name="factory">The factory.</param>
        public Logger(ILoggerFactory factory)
        {
            _logger = factory.CreateLogger<T>();
        }

        public bool IsEnabled(LogLevel level)
        {
            return _logger.IsEnabled(level);
        }

        public void Log(string logItemName, LogLevel level, object arguments = null)
        {
            _logger.Log(logItemName, level, arguments);
        }

        /// <summary>
        /// </summary>
        public IDisposable ActivityStart(string activityName, LogLevel level = LogLevel.Critical, object arguments = null)
        {
            return _logger.ActivityStart(activityName, level, arguments);
        }

        /// <summary>
        /// </summary>
        public virtual IDisposable Subscribe(IObserver<KeyValuePair<string, object>> observer, Func<string, LogLevel, bool> filter = null)
        {
            return (_logger as Logger).Subscribe(observer, filter);
        }
    }
}
