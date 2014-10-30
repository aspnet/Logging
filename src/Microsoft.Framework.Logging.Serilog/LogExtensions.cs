using System;

namespace Microsoft.Framework.Logging.Serilog
{
    public static class LogExtensions
    {
        private static readonly Func<object, Exception, string> _logDataFormatter = (state, ex) =>
        {
            return ((LogData)state).ToString();
        };

        public static void Write(this ILogger logger, TraceType traceType, LogData message, Exception exception = null)
        {
            logger.Write(traceType, 0, message, null, _logDataFormatter);
        }

        public static void Verbose(this ILogger logger, LogData message, Exception exception = null)
        {
            logger.Write(TraceType.Verbose, message, exception);
        }
        public static void Information(this ILogger logger, LogData message, Exception exception = null)
        {
            logger.Write(TraceType.Information, message, exception);
        }
        public static void Warning(this ILogger logger, LogData message, Exception exception = null)
        {
            logger.Write(TraceType.Warning, message, exception);
        }
        public static void Error(this ILogger logger, LogData message, Exception exception = null)
        {
            logger.Write(TraceType.Error, message, exception);
        }
        public static void Critical(this ILogger logger, LogData message, Exception exception = null)
        {
            logger.Write(TraceType.Critical, message, exception);
        }
    }
}