// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Logging.Test.Debug;
using Xunit;

namespace Microsoft.Extensions.Logging
{
    public class DebugLoggerTest
    {
        private const string _loggerName = "test";
        private const string _state = "This is a test, and {curly braces} are just fine!";
        private readonly Func<object, Exception, string> _defaultFormatter = (state, exception) => state.ToString();

        [Fact]
        public void CallingBeginScopeOnLogger_ReturnsNonNullableInstance()
        {
            // Arrange
            var logger = new DebugLogger("Test");

            // Act
            var disposable = logger.BeginScope("Scope1");

            // Assert
            Assert.NotNull(disposable);
        }

        [Fact]
        public void CallingLogWithCurlyBracesAfterFormatter_DoesNotThrow()
        {
            // Arrange
            var logger = new DebugLogger("Test");
            var message = "{test string}";

            // Act
            logger.Log(LogLevel.Debug, 0, message, null, (s, e) => s);
        }

        [Fact]
        public void LogsWhenNullFilterGiven()
        {
            // Arrange
            var logger = new DebugLogger(_loggerName, filter: null, includeScopes: false);
            var sink = new TestDebug();
            logger.Debug = sink;

            var logLevel = LogLevel.Information;
            var expectedMessage = $"{logLevel}: {_state}";

            // Act
            logger.Log(LogLevel.Information, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(1, sink.Messages.Count);
            Assert.Equal(expectedMessage, sink.Messages[0]);
        }

        [Fact]
        public void CriticalFilter_LogsWhenAppropriate()
        {
            // Arrange
            var logger = new DebugLogger(_loggerName, filter: (category, logLevel) => logLevel >= LogLevel.Critical, includeScopes: false);
            var sink = new TestDebug();
            logger.Debug = sink;

            // Act
            logger.Log(LogLevel.Warning, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(0, sink.Messages.Count);

            // Act
            logger.Log(LogLevel.Critical, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(1, sink.Messages.Count);
        }

        [Fact]
        public void ErrorFilter_LogsWhenAppropriate()
        {
            // Arrange
            var logger = new DebugLogger(_loggerName, filter: (category, logLevel) => logLevel >= LogLevel.Error, includeScopes: false);
            var sink = new TestDebug();
            logger.Debug = sink;

            // Act
            logger.Log(LogLevel.Warning, 0, _state, null, null);

            // Assert
            Assert.Equal(0, sink.Messages.Count);

            // Act
            logger.Log(LogLevel.Error, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(1, sink.Messages.Count);
        }

        [Fact]
        public void WarningFilter_LogsWhenAppropriate()
        {
            // Arrange
            var logger = new DebugLogger(_loggerName, filter: (category, logLevel) => logLevel >= LogLevel.Warning, includeScopes: false);
            var sink = new TestDebug();
            logger.Debug = sink;

            // Act
            logger.Log(LogLevel.Information, 0, _state, null, null);

            // Assert
            Assert.Equal(0, sink.Messages.Count);

            // Act
            logger.Log(LogLevel.Warning, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(1, sink.Messages.Count);
        }

        [Fact]
        public void InformationFilter_LogsWhenAppropriate()
        {
            // Arrange
            var logger = new DebugLogger(_loggerName, filter: (category, logLevel) => logLevel >= LogLevel.Information, includeScopes: false);
            var sink = new TestDebug();
            logger.Debug = sink;

            // Act
            logger.Log(LogLevel.Debug, 0, _state, null, null);

            // Assert
            Assert.Equal(0, sink.Messages.Count);

            // Act
            logger.Log(LogLevel.Information, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(1, sink.Messages.Count);
        }

        [Fact]
        public void DebugFilter_LogsWhenAppropriate()
        {
            // Arrange
            var logger = new DebugLogger(_loggerName, filter: (category, logLevel) => logLevel >= LogLevel.Debug, includeScopes: false);
            var sink = new TestDebug();
            logger.Debug = sink;

            // Act
            logger.Log(LogLevel.Trace, 0, _state, null, null);

            // Assert
            Assert.Equal(0, sink.Messages.Count);

            // Act
            logger.Log(LogLevel.Debug, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(1, sink.Messages.Count);
        }

        [Fact]
        public void TraceFilter_LogsWhenAppropriate()
        {
            // Arrange
            var logger = new DebugLogger(_loggerName, filter: (category, logLevel) => logLevel >= LogLevel.Trace, includeScopes: false);
            var sink = new TestDebug();
            logger.Debug = sink;

            // Act
            logger.Log(LogLevel.Critical, 0, _state, null, _defaultFormatter);
            logger.Log(LogLevel.Error, 0, _state, null, _defaultFormatter);
            logger.Log(LogLevel.Warning, 0, _state, null, _defaultFormatter);
            logger.Log(LogLevel.Information, 0, _state, null, _defaultFormatter);
            logger.Log(LogLevel.Debug, 0, _state, null, _defaultFormatter);
            logger.Log(LogLevel.Trace, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(6, sink.Messages.Count);
        }

        [Fact]
        public void DebugLogger_ReloadSettings_CanChangeLogLevel()
        {
            // Arrange
            var settings = new MockConfigurableLoggerSettings()
            {
                Cancel = new CancellationTokenSource(),
                Switches =
                {
                    ["Test"] = LogLevel.Information,
                }
            };

            var provider = new DebugLoggerProvider(settings);
            var logger = (DebugLogger)provider.CreateLogger("Test");
            logger.Debug = new TestDebug();

            Assert.False(logger.IsEnabled(LogLevel.Trace));

            settings.Switches["Test"] = LogLevel.Trace;

            var cancellationTokenSource = settings.Cancel;
            settings.Cancel = new CancellationTokenSource();

            // Act
            cancellationTokenSource.Cancel();

            // Assert
            Assert.True(logger.IsEnabled(LogLevel.Trace));
        }

        [Fact]
        public void DebugLogger_ReloadSettings_CanReloadMultipleTimes()
        {
            // Arrange
            var settings = new MockConfigurableLoggerSettings()
            {
                Cancel = new CancellationTokenSource(),
                Switches =
                {
                    ["Test"] = LogLevel.Information,
                }
            };

            var provider = new DebugLoggerProvider(settings);
            var logger = (DebugLogger)provider.CreateLogger("Test");
            logger.Debug = new TestDebug();

            Assert.False(logger.IsEnabled(LogLevel.Trace));

            // Act & Assert
            for (var i = 0; i < 10; i++)
            {
                settings.Switches["Test"] = i % 2 == 0 ? LogLevel.Information : LogLevel.Trace;

                var cancellationTokenSource = settings.Cancel;
                settings.Cancel = new CancellationTokenSource();

                cancellationTokenSource.Cancel();

                Assert.Equal(i % 2 == 1, logger.IsEnabled(LogLevel.Trace));
            }
        }
    }
}
