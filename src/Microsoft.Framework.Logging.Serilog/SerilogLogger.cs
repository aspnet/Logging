﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Serilog.Events;
using Serilog.Core;

namespace Microsoft.Framework.Logging.Serilog
{
    public class SerilogLogger : ILogger
    {
        private readonly SerilogLoggerProvider _provider;
        private readonly string _name;
        private readonly global::Serilog.ILogger _logger;

        public SerilogLogger(
            SerilogLoggerProvider provider,
            [NotNull] global::Serilog.ILogger logger,
            string name)
        {
            _provider = provider;
            _name = name;
            _logger = logger.ForContext(Constants.SourceContextPropertyName, name);
        }

        public IDisposable BeginScope(object state)
        {
            return _provider.BeginScope(_name, state);
        }

        public bool IsEnabled(TraceType eventType)
        {
            return _logger.IsEnabled(ConvertLevel(eventType));
        }

        public void Write(TraceType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            var level = ConvertLevel(eventType);
            if (!_logger.IsEnabled(level))
            {
                return;
            }

            var logger = _logger;
            var now = DateTimeOffset.Now;
            
            var message = formatter.Invoke(state, exception);
            var structure = state as ILoggerStructure;
            if (structure != null)
            {
                logger = logger.ForContext(new[] { new StructureEnricher(structure) });
            }
            if (exception != null)
            {
                logger = logger.ForContext(new[] { new ExceptionEnricher(exception) });
            }

            logger.Write(level, "{Message:l}", message);
        }

        private LogEventLevel ConvertLevel(TraceType eventType)
        {
            switch (eventType)
            {
                case TraceType.Critical:
                    return LogEventLevel.Fatal;
                case TraceType.Error:
                    return LogEventLevel.Error;
                case TraceType.Warning:
                    return LogEventLevel.Warning;
                case TraceType.Information:
                    return LogEventLevel.Information;
                case TraceType.Verbose:
                    return LogEventLevel.Verbose;
                default:
                    throw new NotSupportedException();
            }
        }

        private class StructureEnricher : ILogEventEnricher
        {
            private readonly ILoggerStructure _structure;

            public StructureEnricher(ILoggerStructure structure)
            {
                _structure = structure;
            }

            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                foreach (var value in _structure.GetValues())
                {
                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                        value.Key,
                        value.Value));
                }
            }
        }

        private class ExceptionEnricher : ILogEventEnricher
        {
            private readonly Exception _exception;

            public ExceptionEnricher(Exception exception)
            {
                _exception = exception;
            }

            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                    "Exception",
                    _exception.ToString()));

                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                    "ExceptionType",
                    _exception.GetType().FullName));

                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                    "ExceptionMessage",
                    _exception.Message));
            }
        }
    }
}