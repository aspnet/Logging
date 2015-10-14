// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if DNX451
using System;
using System.Linq;
using Microsoft.Extensions.Logging.EventLog;
using Xunit;

namespace Microsoft.Extensions.Logging
{
    public class EventLogLoggerTest
    {
        [Fact]
        public void CallingBeginScopeOnLogger_AlwaysReturnsNewDisposableInstance()
        {
            // Arrange
            var logger = new EventLogLogger("Test");

            // Act
            var disposable1 = logger.BeginScopeImpl("Scope1");
            var disposable2 = logger.BeginScopeImpl("Scope2");

            // Assert
            Assert.NotNull(disposable1);
            Assert.NotNull(disposable2);
            Assert.NotSame(disposable1, disposable2);
        }

        [Fact]
        public void CallingBeginScopeOnLogger_ReturnsNonNullableInstance()
        {
            // Arrange
            var logger = new EventLogLogger("Test");

            // Act
            var disposable = logger.BeginScopeImpl("Scope1");

            // Assert
            Assert.NotNull(disposable);
        }

        [Fact]
        public void EventLoggerProvider_UsesLoggerFactoryDefault_MinimumLevel()
        {
            // Arrange
            var loggerFactory = new LoggerFactory();
            var minimumLogLevel = loggerFactory.MinimumLevel;

            // Act
            loggerFactory.AddEventLog();

            // Assert
            var providers = loggerFactory.GetProviders();
            var eventLogProvider = Assert.IsType<EventLogLoggerProvider>(providers.FirstOrDefault());
            Assert.NotNull(eventLogProvider.Settings);
            var filter = eventLogProvider.Settings.Filter;
            Assert.NotNull(filter);
            Assert.True(filter("testlogger", minimumLogLevel));

            // less than minimum log level
            LogLevel level;
            Assert.True(Enum.TryParse(Enum.GetName(typeof(LogLevel), (int)minimumLogLevel - 1), out level));
            Assert.False(filter("testlogger", level));

            // more than minimum log level
            Assert.True(Enum.TryParse(Enum.GetName(typeof(LogLevel), (int)minimumLogLevel + 1), out level));
            Assert.True(filter("testlogger", level));
        }
    }
}
#endif