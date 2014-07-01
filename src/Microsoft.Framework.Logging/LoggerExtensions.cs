// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Framework.Logging
{
    /// <summary>
    /// ILogger extension methods for common scenarios.
    /// </summary>
    public static class LoggerExtensions
    {
        private static readonly Func<object, Exception, string> TheMessage = (message, error) => (string)message;
        private static readonly Func<object, Exception, string> TheMessageAndError = (message, error) => string.Format(CultureInfo.CurrentCulture, "{0}\r\n{1}", message, error);

        /// <summary>
        /// Checks if the given TraceEventType is enabled.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public static bool IsEnabled(this ILogger logger, TraceType eventType)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            return logger.WriteCore(eventType, 0, null, null, null);
        }

        /// <summary>
        /// Writes a begin log message
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="eventType">The event type.</param>
        /// <param name="message">The message to be written.</param>
        /// <returns>
        /// Returns an IDisposable that calls <see cref="WriteEnd(ILogger, TraceType, string)"/>.
        /// </returns>
        public static IDisposable WriteStart(this ILogger logger, TraceType eventType, string message)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger"); 
            }

            logger.WriteCore(eventType | TraceType.Start, 0, message, null, TheMessage);

            return new LogicalOperation(logger, eventType, message);
        }

        /// <summary>
        /// Writes an end log message
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="eventType">The event type.</param>
        /// <param name="message">The message to be written.</param>
        /// <returns></returns>
        public static void WriteStop(this ILogger logger, TraceType eventType, string message)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            logger.WriteCore(eventType | TraceType.Stop, 0, message, null, TheMessage);
        }

        /// <summary>
        /// Writes a verbose log message.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="data"></param>
        // FYI, this field is called data because naming it message triggers CA1303 and CA2204 for callers.
        public static void WriteVerbose(this ILogger logger, string data)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            logger.WriteCore(TraceType.Verbose, 0, data, null, TheMessage);
        }

        /// <summary>
        /// Writes an informational log message.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void WriteInformation(this ILogger logger, string message)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            logger.WriteCore(TraceType.Information, 0, message, null, TheMessage);
        }

        /// <summary>
        /// Writes a warning log message.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteWarning(this ILogger logger, string message, params string[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            logger.WriteCore(TraceType.Warning, 0,
                string.Format(CultureInfo.InvariantCulture, message, args), null, TheMessage);
        }

        /// <summary>
        /// Writes a warning log message.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="error"></param>
        public static void WriteWarning(this ILogger logger, string message, Exception error)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            logger.WriteCore(TraceType.Warning, 0, message, error, TheMessageAndError);
        }

        /// <summary>
        /// Writes an error log message.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void WriteError(this ILogger logger, string message)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            logger.WriteCore(TraceType.Error, 0, message, null, TheMessage);
        }

        /// <summary>
        /// Writes an error log message.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="error"></param>
        public static void WriteError(this ILogger logger, string message, Exception error)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            logger.WriteCore(TraceType.Error, 0, message, error, TheMessageAndError);
        }

        /// <summary>
        /// Writes a critical log message.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void WriteCritical(this ILogger logger, string message)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            logger.WriteCore(TraceType.Critical, 0, message, null, TheMessage);
        }

        /// <summary>
        /// Writes a critical log message.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="error"></param>
        public static void WriteCritical(this ILogger logger, string message, Exception error)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            logger.WriteCore(TraceType.Critical, 0, message, error, TheMessageAndError);
        }
    }
}
