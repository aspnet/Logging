// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Framework.Internal;
using Serilog.Core;
using Serilog.Events;
using SLogger = Serilog.ILogger;

namespace Microsoft.Framework.Logging.Serilog
{
    public class SerilogLogger : ILogger
    {
        private readonly SerilogLoggerProvider _provider;
        private readonly string _name;
        private readonly SLogger _logger;

        public SerilogLogger(
            [NotNull] SerilogLoggerProvider provider,
            [NotNull] SLogger logger,
            string name)
        {
            _provider = provider;
            _name = name;
            _logger = logger.ForContext(Constants.SourceContextPropertyName, name);
        }

        public IDisposable BeginScopeImpl(object state)
        {
            return _provider.BeginScopeImpl(_name, state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(ConvertLevel(logLevel));
        }

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            var level = ConvertLevel(logLevel);
            if (!_logger.IsEnabled(level))
            {
                return;
            }

            var logger = _logger;
            string messageTemplate = null;

            var structure = state as ILogValues;
            if (structure != null)
            {
                foreach (var property in structure.GetValues())
                {
                    if (property.Key == "{OriginalFormat}" && property.Value is string)
                    {
                        messageTemplate = (string)property.Value;
                    }

                    logger = logger.ForContext(property.Key, property.Value);
                }
            }

            if (messageTemplate == null && state != null)
            {
                messageTemplate = LogFormatter.Formatter(state, null);
            }

            if (string.IsNullOrEmpty(messageTemplate))
            {
                return;
            }

            if (eventId != 0)
            {
                logger = logger.ForContext("EventId", eventId, false);
            }

            logger.Write(level, exception, messageTemplate);
        }

        private LogEventLevel ConvertLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return LogEventLevel.Fatal;
                case LogLevel.Error:
                    return LogEventLevel.Error;
                case LogLevel.Warning:
                    return LogEventLevel.Warning;
                case LogLevel.Information:
                    return LogEventLevel.Information;
                case LogLevel.Verbose:
                    return LogEventLevel.Debug;
                case LogLevel.Debug:
                default:
                    return LogEventLevel.Verbose;
            }
        }
    }
}