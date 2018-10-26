// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Logging.Performance
{
    [AspNetCoreBenchmark]
    public class LogValuesBenchmarks : LoggingBenchmarkBase
    {
        private ILogger _logger;

        [Benchmark]
        public void NotFiltered_InsideScope()
        {
            TwoArgumentErrorMessage(_logger, 1, "string", Exception);
        }

        [GlobalSetup]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<ILoggerProvider, LoggerProvider<MessageFormattingLogger>>();

            _logger = services.BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger("Logger");
        }

        public class MessageFormattingLogger: ILogger
        {
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                formatter(state, exception);
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        }
    }

    [AspNetCoreBenchmark]
    public class ScopesOverheadBenchmark: LoggingBenchmarkBase
    {
        private ILogger _logger;

        //[Params(true, false)]
        public bool HasISupportLoggingScopeLogger { get; set; } = false;

        //[Params(true, false)]
        public bool CaptureScopes { get; set; } = false;

        // Baseline as this is the fastest way to do nothing
        [Benchmark(Baseline = true)]
        public void FilteredByLevel()
        {
            TwoArgumentTraceMessage(_logger, 1, "string", Exception);
        }

        [Benchmark]
        public void FilteredByLevel_InsideScope()
        {
            using (_logger.BeginScope("string"))
            {
                TwoArgumentTraceMessage(_logger, 1, "string", Exception);
            }
        }

        [Benchmark]
        public void NotFiltered()
        {
            TwoArgumentErrorMessage(_logger, 1, "string", Exception);
        }

        [Benchmark]
        public void NotFiltered_InsideScope()
        {
            using (_logger.BeginScope("string"))
            {
                TwoArgumentErrorMessage(_logger, 1, "string", Exception);
            }
        }

        [GlobalSetup]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            if (HasISupportLoggingScopeLogger)
            {
                services.AddSingleton<ILoggerProvider, LoggerProviderWithISupportExternalScope>();
            }
            else
            {
                services.AddSingleton<ILoggerProvider, LoggerProvider<NoopLogger>>();
            }

            services.Configure<LoggerFilterOptions>(options => options.CaptureScopes = CaptureScopes);

            _logger = services.BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger("Logger");
        }

        class LoggerProviderWithISupportExternalScope: LoggerProvider<NoopLogger>, ISupportExternalScope
        {
            public void SetScopeProvider(IExternalScopeProvider scopeProvider)
            {
            }
        }
    }
}