using System;
using Microsoft.Extensions.Logging;

namespace SampleApp
{
    internal static class LoggerExtensions
    {
        private static Func<ILogger, string, IDisposable> _purchaseOrderScope;
        private static Action<ILogger, DateTimeOffset, int, Exception> _programStarting;
        private static Action<ILogger, DateTimeOffset, Exception> _programStopping;

        static LoggerExtensions()
        {
            LoggerMessage.DefineScope(out _purchaseOrderScope, "PO:{PurchaseOrder}");
            LoggerMessage.Define(out _programStarting, LogLevel.Information, 1, "Starting", "at '{StartTime}' and 0x{Hello:X} is hex of 42");
            LoggerMessage.Define(out _programStopping, LogLevel.Information, 2, "Stopping", "at '{StopTime}'");
        }

        public static IDisposable PurchaseOrderScope(this ILogger logger, string purchaseOrder)
        {
            return _purchaseOrderScope(logger, purchaseOrder);
        }

        public static void ProgramStarting(this ILogger logger, DateTimeOffset startTime, int hello, Exception exception = null)
        {
            _programStarting(logger, startTime, hello, exception);
        }

        public static void ProgramStopping(this ILogger logger, DateTimeOffset stopTime, Exception exception = null)
        {
            _programStopping(logger, stopTime, exception);
        }
    }
}

