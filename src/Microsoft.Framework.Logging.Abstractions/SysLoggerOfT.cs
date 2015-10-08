using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Framework.Logging
{
    /// <summary>
    /// Encapsulates System.Diagnostics.Tracing.Logger for DI
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public class SysLogger<T> 
    {
        private readonly System.Diagnostics.Tracing.Logger _logger;

        /// <summary>
        /// Creates a new Logger
        /// </summary>
        /// <param name="factory">The factory.</param>
        public SysLogger(ILoggerFactory factory)
        {
            _logger = factory.CreateSystemLogger<T>();
        }

        public bool IsEnabled(System.Diagnostics.Tracing.LogLevel level)
        {
            return _logger.IsEnabled(level);
        }

        public void Log(System.Diagnostics.Tracing.LogLevel level, string logItemName, object arguments = null)
        {
            _logger.Log(level, logItemName, arguments);
        }

        /// <summary>
        /// </summary>
        public IDisposable ActivityStart(System.Diagnostics.Tracing.LogLevel level, string activityName, object arguments)
        {
            return _logger.ActivityStart(level, activityName, arguments);
        }

        /// <summary>
        /// </summary>
        public virtual IDisposable Subscribe(IObserver<KeyValuePair<string, object>> observer, System.Diagnostics.Tracing.LogLevel level)
        {
            return _logger.Subscribe(observer, level);
        }
    }
}
