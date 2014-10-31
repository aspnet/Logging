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
        private static readonly Func<object, Exception, string> TheMessageAndError = (message, error)
            => string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", message, Environment.NewLine, error);
        private static readonly Func<object, Exception, string> LoggerStructureFormatter = (state, ex) 
            => ((LoggerStructureBase)state).ToString();

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

            logger.Write(LogLevel.Verbose, 0, data, null, TheMessage);
        }

        public static void WriteVerbose(this ILogger logger, LoggerStructureBase message, Exception exception = null)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            logger.Write(TraceType.Verbose, message, exception);
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

            logger.Write(LogLevel.Information, 0, message, null, TheMessage);
        }

        public static void WriteInformation(this ILogger logger, LoggerStructureBase message, Exception exception = null)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            logger.Write(TraceType.Information, message, exception);
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

            logger.Write(LogLevel.Warning, 0,
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

            logger.Write(LogLevel.Warning, 0, message, error, TheMessageAndError);
        }

        public static void WriteWarning(this ILogger logger, LoggerStructureBase message, Exception exception = null)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            logger.Write(TraceType.Warning, message, exception);
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

            logger.Write(LogLevel.Error, 0, message, null, TheMessage);
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

            logger.Write(LogLevel.Error, 0, message, error, TheMessageAndError);
        }

        public static void WriteError(this ILogger logger, LoggerStructureBase message, Exception exception = null)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            logger.Write(TraceType.Error, message, exception);
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

            logger.Write(LogLevel.Critical, 0, message, null, TheMessage);
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

            logger.Write(LogLevel.Critical, 0, message, error, TheMessageAndError);
        }

        public static void WriteCritical(this ILogger logger, LoggerStructureBase message, Exception exception = null)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            logger.Write(TraceType.Critical, message, exception);
        }

        private static void Write(this ILogger logger, TraceType traceType, LoggerStructureBase message, Exception exception = null)
        {
            logger.Write(traceType, 0, message, null, LoggerStructureFormatter);
        }
    }
}
