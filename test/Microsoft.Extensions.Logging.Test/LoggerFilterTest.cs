// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Extensions.Logging.Test;
using Xunit;

namespace Microsoft.Extensions.Logging
{
    public class LoggerFilterTest
    {
        [Fact]
        public void FiltersMessages_OnDefaultLogLevel_BeforeSendingTo_AllRegisteredLoggerProviders()
        {
            // Arrange
            var loggerProvider1 = new TestLoggerProvider(new TestSink(), isEnabled: true);
            var loggerProvider2 = new TestLoggerProvider(new TestSink(), isEnabled: true);
            var loggerFactory = new LoggerFactory()
                .WithFilter(new FilterLoggerSettings()
                {
                    { "Default", LogLevel.Warning }
                });
            loggerFactory.AddProvider(loggerProvider1);
            loggerFactory.AddProvider(loggerProvider2);
            var logger1 = loggerFactory.CreateLogger("Microsoft.Foo");

            // Act
            logger1.LogCritical("critical event");
            logger1.LogDebug("debug event");
            logger1.LogInformation("information event");

            // Assert
            foreach (var sink in new[] { loggerProvider1.Sink, loggerProvider2.Sink })
            {
                var logEventWrites = sink.Writes.Where(wc => wc.LoggerName.StartsWith("Microsoft.Foo"));
                var logEventWrite = Assert.Single(logEventWrites);
                Assert.Equal("critical event", logEventWrite.State?.ToString());
                Assert.Equal(LogLevel.Critical, logEventWrite.LogLevel);
            }
        }

        [Fact]
        public void FiltersMessages_BeforeSendingTo_AllRegisteredLoggerProviders()
        {
            // Arrange
            var loggerProvider1 = new TestLoggerProvider(new TestSink(), isEnabled: true);
            var loggerProvider2 = new TestLoggerProvider(new TestSink(), isEnabled: true);
            var loggerFactory = new LoggerFactory()
                .WithFilter(new FilterLoggerSettings()
                {
                    { "Microsoft", LogLevel.Warning },
                    { "System", LogLevel.Warning },
                    { "SampleApp", LogLevel.Debug },
                });
            loggerFactory.AddProvider(loggerProvider1);
            loggerFactory.AddProvider(loggerProvider2);
            var microsoftAssemblyLogger = loggerFactory.CreateLogger("Microsoft.Foo");
            var systemAssemblyLogger = loggerFactory.CreateLogger("System.Foo");
            var myappAssemblyLogger = loggerFactory.CreateLogger("SampleApp.Program");

            // Act
            microsoftAssemblyLogger.LogCritical("critical event");
            microsoftAssemblyLogger.LogDebug("debug event");
            microsoftAssemblyLogger.LogInformation("information event");
            systemAssemblyLogger.LogCritical("critical event");
            systemAssemblyLogger.LogDebug("debug event");
            systemAssemblyLogger.LogInformation("information event");
            myappAssemblyLogger.LogCritical("critical event");
            myappAssemblyLogger.LogDebug("debug event");
            myappAssemblyLogger.LogInformation("information event");

            // Assert
            foreach (var sink in new[] { loggerProvider1.Sink, loggerProvider2.Sink })
            {
                var logEventWrites = sink.Writes.Where(wc => wc.LoggerName.StartsWith("Microsoft"));
                var logEventWrite = Assert.Single(logEventWrites);
                Assert.Equal(LogLevel.Critical, logEventWrite.LogLevel);
                Assert.Equal("critical event", logEventWrite.State?.ToString());

                logEventWrites = sink.Writes.Where(wc => wc.LoggerName.StartsWith("System"));
                logEventWrite = Assert.Single(logEventWrites);
                Assert.Equal(LogLevel.Critical, logEventWrite.LogLevel);
                Assert.Equal("critical event", logEventWrite.State?.ToString());

                logEventWrites = sink.Writes.Where(wc => wc.LoggerName.StartsWith("SampleApp.Program"));
                logEventWrite = Assert.Single(logEventWrites.Where(wc => wc.LogLevel == LogLevel.Critical));
                Assert.Equal("critical event", logEventWrite.State?.ToString());
                logEventWrite = Assert.Single(logEventWrites.Where(wc => wc.LogLevel == LogLevel.Debug));
                Assert.Equal("debug event", logEventWrite.State?.ToString());
                logEventWrite = Assert.Single(logEventWrites.Where(wc => wc.LogLevel == LogLevel.Information));
                Assert.Equal("information event", logEventWrite.State?.ToString());
            }
        }

        [Fact]
        public void BeginScope_CreatesScopesOn_AllRegisteredLoggerProviders()
        {
            // Arrange
            var loggerProvider1 = new TestLoggerProvider(new TestSink(), isEnabled: true);
            var loggerProvider2 = new TestLoggerProvider(new TestSink(), isEnabled: true);
            var loggerFactory = new LoggerFactory()
                .WithFilter(new FilterLoggerSettings()
                {
                    { "Microsoft", LogLevel.Warning },
                    { "System", LogLevel.Warning },
                    { "SampleApp", LogLevel.Debug },
                });
            loggerFactory.AddProvider(loggerProvider1);
            loggerFactory.AddProvider(loggerProvider2);
            var microsoftAssemblyLogger = loggerFactory.CreateLogger("Microsoft.foo");
            var systemAssemblyLogger = loggerFactory.CreateLogger("System.foo");
            var myappAssemblyLogger = loggerFactory.CreateLogger("SampleApp.Program");

            // Act
            var disposable1 = systemAssemblyLogger.BeginScope("Scope1");
            var disposable2 = microsoftAssemblyLogger.BeginScope("Scope2");
            var disposable3 = myappAssemblyLogger.BeginScope("Scope3");

            // Assert
            foreach (var sink in new[] { loggerProvider1.Sink, loggerProvider2.Sink })
            {
                var scopeContexts = sink.Scopes;
                Assert.Equal(3, scopeContexts.Count);

                Assert.Equal("Scope1", scopeContexts[0].Scope?.ToString());
                Assert.NotNull(disposable1);

                Assert.Equal("Scope2", scopeContexts[1].Scope?.ToString());
                Assert.NotNull(disposable2);

                Assert.Equal("Scope3", scopeContexts[2].Scope?.ToString());
                Assert.NotNull(disposable3);
            }
        }

        [Fact]
        public void DisposeOnLoggerFactory_CallsDisposeOn_AllRegisteredLoggerProviders()
        {
            // Arrange
            var loggerProvider1 = new TestLoggerProvider(new TestSink(), isEnabled: true);
            var loggerProvider2 = new TestLoggerProvider(new TestSink(), isEnabled: true);
            var loggerFactory = new LoggerFactory()
                .WithFilter(new FilterLoggerSettings()
                {
                    { "Microsoft", LogLevel.Warning },
                    { "System", LogLevel.Warning },
                    { "SampleApp", LogLevel.Debug },
                });
            loggerFactory.AddProvider(loggerProvider1);
            loggerFactory.AddProvider(loggerProvider2);
            var logger1 = loggerFactory.CreateLogger("Microsoft.foo");

            // Act
            loggerFactory.Dispose();

            // Assert
            Assert.True(loggerProvider1.DisposeCalled);
            Assert.True(loggerProvider2.DisposeCalled);
        }
    }
}
