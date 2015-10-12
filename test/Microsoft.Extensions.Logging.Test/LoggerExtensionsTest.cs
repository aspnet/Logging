// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging.Internal;
using Xunit;

namespace Microsoft.Extensions.Logging.Test
{
    public class LoggerExtensionsTest
    {
        private const string _name = "test";
        private const string _state = "testing";
        private const string _format = "{0}, {1}";
        private static Exception _exception = new InvalidOperationException();

        private TestLogger SetUp(TestSink sink)
        {
            // Arrange
            var logger = new TestLogger(_name, sink, enabled: true);
            return logger;
        }

        [Fact]
        public void MessageOnly_LogsCorrectValues()
        {
            // Arrange
            var sink = new TestSink();
            var logger = SetUp(sink);

            // Act
            logger.LogVerbose(_state);
            logger.LogInformation(_state);
            logger.LogWarning(_state);
            logger.LogError(_state);
            logger.LogCritical(_state);
            logger.LogDebug(_state);

            // Assert
            Assert.Equal(6, sink.Writes.Count);

            var verbose = sink.Writes[0];
            Assert.Equal(LogLevel.Verbose, verbose.LogLevel);
            Assert.Equal(_state, verbose.State);
            Assert.Equal(0, verbose.EventId);
            Assert.Equal(null, verbose.Exception);

            var information = sink.Writes[1];
            Assert.Equal(LogLevel.Information, information.LogLevel);
            Assert.Equal(_state, information.State);
            Assert.Equal(0, information.EventId);
            Assert.Equal(null, information.Exception);

            var warning = sink.Writes[2];
            Assert.Equal(LogLevel.Warning, warning.LogLevel);
            Assert.Equal(_state, warning.State);
            Assert.Equal(0, warning.EventId);
            Assert.Equal(null, warning.Exception);

            var error = sink.Writes[3];
            Assert.Equal(LogLevel.Error, error.LogLevel);
            Assert.Equal(_state, error.State);
            Assert.Equal(0, error.EventId);
            Assert.Equal(null, error.Exception);

            var critical = sink.Writes[4];
            Assert.Equal(LogLevel.Critical, critical.LogLevel);
            Assert.Equal(_state, critical.State);
            Assert.Equal(0, critical.EventId);
            Assert.Equal(null, critical.Exception);

            var debug = sink.Writes[5];
            Assert.Equal(LogLevel.Debug, debug.LogLevel);
            Assert.Equal(_state, debug.State);
            Assert.Equal(0, debug.EventId);
            Assert.Equal(null, debug.Exception);
        }

        [Fact]
        public void FormatMessage_LogsCorrectValues()
        {
            // Arrange
            var sink = new TestSink();
            var logger = SetUp(sink);

            // Act
            logger.LogVerbose(_format, "test1", "test2");
            logger.LogInformation(_format, "test1", "test2");
            logger.LogWarning(_format, "test1", "test2");
            logger.LogError(_format, "test1", "test2");
            logger.LogCritical(_format, "test1", "test2");
            logger.LogDebug(_format, "test1", "test2");

            // Assert
            Assert.Equal(6, sink.Writes.Count);

            var verbose = sink.Writes[0];
            Assert.Equal(LogLevel.Verbose, verbose.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), verbose.State?.ToString());
            Assert.Equal(0, verbose.EventId);
            Assert.Equal(null, verbose.Exception);

            var information = sink.Writes[1];
            Assert.Equal(LogLevel.Information, information.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), information.State?.ToString());
            Assert.Equal(0, information.EventId);
            Assert.Equal(null, information.Exception);

            var warning = sink.Writes[2];
            Assert.Equal(LogLevel.Warning, warning.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), warning.State?.ToString());
            Assert.Equal(0, warning.EventId);
            Assert.Equal(null, warning.Exception);

            var error = sink.Writes[3];
            Assert.Equal(LogLevel.Error, error.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), error.State?.ToString());
            Assert.Equal(0, error.EventId);
            Assert.Equal(null, error.Exception);

            var critical = sink.Writes[4];
            Assert.Equal(LogLevel.Critical, critical.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), critical.State?.ToString());
            Assert.Equal(0, critical.EventId);
            Assert.Equal(null, critical.Exception);

            var debug = sink.Writes[5];
            Assert.Equal(LogLevel.Debug, debug.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), debug.State?.ToString());
            Assert.Equal(0, debug.EventId);
            Assert.Equal(null, debug.Exception);
        }

        [Fact]
        public void MessageAndEventId_LogsCorrectValues()
        {
            // Arrange
            var sink = new TestSink();
            var logger = SetUp(sink);

            // Act
            logger.LogVerbose(1, _state);
            logger.LogInformation(2, _state);
            logger.LogWarning(3, _state);
            logger.LogError(4, _state);
            logger.LogCritical(5, _state);
            logger.LogDebug(6, _state);

            // Assert
            Assert.Equal(6, sink.Writes.Count);

            var verbose = sink.Writes[0];
            Assert.Equal(LogLevel.Verbose, verbose.LogLevel);
            Assert.Equal(_state, verbose.State);
            Assert.Equal(1, verbose.EventId);
            Assert.Equal(null, verbose.Exception);

            var information = sink.Writes[1];
            Assert.Equal(LogLevel.Information, information.LogLevel);
            Assert.Equal(_state, information.State);
            Assert.Equal(2, information.EventId);
            Assert.Equal(null, information.Exception);

            var warning = sink.Writes[2];
            Assert.Equal(LogLevel.Warning, warning.LogLevel);
            Assert.Equal(_state, warning.State);
            Assert.Equal(3, warning.EventId);
            Assert.Equal(null, warning.Exception);

            var error = sink.Writes[3];
            Assert.Equal(LogLevel.Error, error.LogLevel);
            Assert.Equal(_state, error.State);
            Assert.Equal(4, error.EventId);
            Assert.Equal(null, error.Exception);

            var critical = sink.Writes[4];
            Assert.Equal(LogLevel.Critical, critical.LogLevel);
            Assert.Equal(_state, critical.State);
            Assert.Equal(5, critical.EventId);
            Assert.Equal(null, critical.Exception);

            var debug = sink.Writes[5];
            Assert.Equal(LogLevel.Debug, debug.LogLevel);
            Assert.Equal(_state, debug.State);
            Assert.Equal(6, debug.EventId);
            Assert.Equal(null, debug.Exception);
        }

        [Fact]
        public void FormatMessageAndEventId_LogsCorrectValues()
        {
            // Arrange
            var sink = new TestSink();
            var logger = SetUp(sink);

            // Act
            logger.LogVerbose(1, _format, "test1", "test2");
            logger.LogInformation(2, _format, "test1", "test2");
            logger.LogWarning(3, _format, "test1", "test2");
            logger.LogError(4, _format, "test1", "test2");
            logger.LogCritical(5, _format, "test1", "test2");
            logger.LogDebug(6, _format, "test1", "test2");

            // Assert
            Assert.Equal(6, sink.Writes.Count);

            var verbose = sink.Writes[0];
            Assert.Equal(LogLevel.Verbose, verbose.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), verbose.State?.ToString());
            Assert.Equal(1, verbose.EventId);
            Assert.Equal(null, verbose.Exception);

            var information = sink.Writes[1];
            Assert.Equal(LogLevel.Information, information.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), information.State?.ToString());
            Assert.Equal(2, information.EventId);
            Assert.Equal(null, information.Exception);

            var warning = sink.Writes[2];
            Assert.Equal(LogLevel.Warning, warning.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), warning.State?.ToString());
            Assert.Equal(3, warning.EventId);
            Assert.Equal(null, warning.Exception);

            var error = sink.Writes[3];
            Assert.Equal(LogLevel.Error, error.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), error.State?.ToString());
            Assert.Equal(4, error.EventId);
            Assert.Equal(null, error.Exception);

            var critical = sink.Writes[4];
            Assert.Equal(LogLevel.Critical, critical.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), critical.State?.ToString());
            Assert.Equal(5, critical.EventId);
            Assert.Equal(null, critical.Exception);

            var debug = sink.Writes[5];
            Assert.Equal(LogLevel.Debug, debug.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), debug.State?.ToString());
            Assert.Equal(6, debug.EventId);
            Assert.Equal(null, debug.Exception);
        }

        [Fact]
        public void MessageAndError_LogsCorrectValues()
        {
            // Arrange
            var sink = new TestSink();
            var logger = SetUp(sink);

            // Act
            logger.LogWarning(_state, _exception);
            logger.LogError(_state, _exception);
            logger.LogCritical(_state, _exception);

            // Assert
            Assert.Equal(3, sink.Writes.Count);

            var warning = sink.Writes[0];
            Assert.Equal(LogLevel.Warning, warning.LogLevel);
            Assert.Equal(_state, warning.State);
            Assert.Equal(0, warning.EventId);
            Assert.Equal(_exception, warning.Exception);

            var error = sink.Writes[1];
            Assert.Equal(LogLevel.Error, error.LogLevel);
            Assert.Equal(_state, error.State);
            Assert.Equal(0, error.EventId);
            Assert.Equal(_exception, error.Exception);

            var critical = sink.Writes[2];
            Assert.Equal(LogLevel.Critical, critical.LogLevel);
            Assert.Equal(_state, critical.State);
            Assert.Equal(0, critical.EventId);
            Assert.Equal(_exception, critical.Exception);
        }

        [Fact]
        public void FormatMessageAndError_LogsCorrectValues()
        {
            // Arrange
            var sink = new TestSink();
            var logger = SetUp(sink);

            // Act
            logger.LogWarning(_exception, _format, "test1", "test2");
            logger.LogError(_exception, _format, "test1", "test2");
            logger.LogCritical(_exception, _format, "test1", "test2");

            // Assert
            Assert.Equal(3, sink.Writes.Count);
            
            var warning = sink.Writes[0];
            Assert.Equal(LogLevel.Warning, warning.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), warning.State?.ToString());
            Assert.Equal(0, warning.EventId);
            Assert.Equal(_exception, warning.Exception);

            var error = sink.Writes[1];
            Assert.Equal(LogLevel.Error, error.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), error.State?.ToString());
            Assert.Equal(0, error.EventId);
            Assert.Equal(_exception, error.Exception);

            var critical = sink.Writes[2];
            Assert.Equal(LogLevel.Critical, critical.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), critical.State?.ToString());
            Assert.Equal(0, critical.EventId);
            Assert.Equal(_exception, critical.Exception);
        }

        [Fact]
        public void MessageEventIdAndError_LogsCorrectValues()
        {
            // Arrange
            var sink = new TestSink();
            var logger = SetUp(sink);

            // Act
            logger.LogWarning(3, _state, _exception);
            logger.LogError(4, _state, _exception);
            logger.LogCritical(5, _state, _exception);

            // Assert
            Assert.Equal(3, sink.Writes.Count);

            var warning = sink.Writes[0];
            Assert.Equal(LogLevel.Warning, warning.LogLevel);
            Assert.Equal(_state, warning.State);
            Assert.Equal(3, warning.EventId);
            Assert.Equal(_exception, warning.Exception);

            var error = sink.Writes[1];
            Assert.Equal(LogLevel.Error, error.LogLevel);
            Assert.Equal(_state, error.State);
            Assert.Equal(4, error.EventId);
            Assert.Equal(_exception, error.Exception);

            var critical = sink.Writes[2];
            Assert.Equal(LogLevel.Critical, critical.LogLevel);
            Assert.Equal(_state, critical.State);
            Assert.Equal(5, critical.EventId);
            Assert.Equal(_exception, critical.Exception);
        }

        [Fact]
        public void FormatMessageEventIdAndError_LogsCorrectValues()
        {
            // Arrange
            var sink = new TestSink();
            var logger = SetUp(sink);

            // Act
            logger.LogWarning(3, _exception, _format, "test1", "test2");
            logger.LogError(4, _exception, _format, "test1", "test2");
            logger.LogCritical(5, _exception, _format, "test1", "test2");
            
            // Assert
            Assert.Equal(3, sink.Writes.Count);

            var warning = sink.Writes[0];
            Assert.Equal(LogLevel.Warning, warning.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), warning.State?.ToString());
            Assert.Equal(3, warning.EventId);
            Assert.Equal(_exception, warning.Exception);

            var error = sink.Writes[1];
            Assert.Equal(LogLevel.Error, error.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), error.State?.ToString());
            Assert.Equal(4, error.EventId);
            Assert.Equal(_exception, error.Exception);

            var critical = sink.Writes[2];
            Assert.Equal(LogLevel.Critical, critical.LogLevel);
            Assert.Equal(string.Format(_format, "test1", "test2"), critical.State?.ToString());
            Assert.Equal(5, critical.EventId);
            Assert.Equal(_exception, critical.Exception);
        }

        [Fact]
        public void LogValues_LogsCorrectValues()
        {
            // Arrange
            var sink = new TestSink();
            var logger = SetUp(sink);
            var testLogValues = new TestLogValues()
            {
                Value = 1
            };

            // Act
            logger.LogVerbose(testLogValues);
            logger.LogInformation(testLogValues);
            logger.LogWarning(testLogValues);
            logger.LogError(testLogValues);
            logger.LogCritical(testLogValues);
            logger.LogDebug(testLogValues);

            // Assert
            Assert.Equal(6, sink.Writes.Count);

            var verbose = sink.Writes[0];
            Assert.Equal(LogLevel.Verbose, verbose.LogLevel);
            Assert.Same(testLogValues, verbose.State);
            Assert.Equal(0, verbose.EventId);
            Assert.Equal(null, verbose.Exception);
            Assert.Equal("Test 1", verbose.Formatter(verbose.State, verbose.Exception));

            var information = sink.Writes[1];
            Assert.Equal(LogLevel.Information, information.LogLevel);
            Assert.Same(testLogValues, information.State);
            Assert.Equal(0, information.EventId);
            Assert.Equal(null, information.Exception);
            Assert.Equal("Test 1", information.Formatter(information.State, information.Exception));

            var warning = sink.Writes[2];
            Assert.Equal(LogLevel.Warning, warning.LogLevel);
            Assert.Same(testLogValues, warning.State);
            Assert.Equal(0, warning.EventId);
            Assert.Equal(null, warning.Exception);
            Assert.Equal("Test 1", warning.Formatter(warning.State, warning.Exception));

            var error = sink.Writes[3];
            Assert.Equal(LogLevel.Error, error.LogLevel);
            Assert.Same(testLogValues, error.State);
            Assert.Equal(0, error.EventId);
            Assert.Equal(null, error.Exception);
            Assert.Equal("Test 1", error.Formatter(error.State, error.Exception));

            var critical = sink.Writes[4];
            Assert.Equal(LogLevel.Critical, critical.LogLevel);
            Assert.Same(testLogValues, critical.State);
            Assert.Equal(0, critical.EventId);
            Assert.Equal(null, critical.Exception);
            Assert.Equal("Test 1", critical.Formatter(critical.State, critical.Exception));

            var debug = sink.Writes[5];
            Assert.Equal(LogLevel.Debug, debug.LogLevel);
            Assert.Same(testLogValues, debug.State);
            Assert.Equal(0, debug.EventId);
            Assert.Equal(null, debug.Exception);
            Assert.Equal("Test 1", debug.Formatter(debug.State, debug.Exception));
        }

        [Fact]
        public void LogValuesAndEventId_LogsCorrectValues()
        {
            // Arrange
            var sink = new TestSink();
            var logger = SetUp(sink);
            var testLogValues = new TestLogValues()
            {
                Value = 1
            };

            // Act
            logger.LogVerbose(1, testLogValues);
            logger.LogInformation(2, testLogValues);
            logger.LogWarning(3, testLogValues);
            logger.LogError(4, testLogValues);
            logger.LogCritical(5, testLogValues);
            logger.LogDebug(6, testLogValues);

            // Assert
            Assert.Equal(6, sink.Writes.Count);

            var verbose = sink.Writes[0];
            Assert.Equal(LogLevel.Verbose, verbose.LogLevel);
            Assert.Same(testLogValues, verbose.State);
            Assert.Equal(1, verbose.EventId);
            Assert.Equal(null, verbose.Exception);
            Assert.Equal("Test 1", verbose.Formatter(verbose.State, verbose.Exception));

            var information = sink.Writes[1];
            Assert.Equal(LogLevel.Information, information.LogLevel);
            Assert.Same(testLogValues, information.State);
            Assert.Equal(2, information.EventId);
            Assert.Equal(null, information.Exception);
            Assert.Equal("Test 1", information.Formatter(information.State, information.Exception));

            var warning = sink.Writes[2];
            Assert.Equal(LogLevel.Warning, warning.LogLevel);
            Assert.Same(testLogValues, warning.State);
            Assert.Equal(3, warning.EventId);
            Assert.Equal(null, warning.Exception);
            Assert.Equal("Test 1", warning.Formatter(warning.State, warning.Exception));

            var error = sink.Writes[3];
            Assert.Equal(LogLevel.Error, error.LogLevel);
            Assert.Same(testLogValues, error.State);
            Assert.Equal(4, error.EventId);
            Assert.Equal(null, error.Exception);
            Assert.Equal("Test 1", error.Formatter(error.State, error.Exception));

            var critical = sink.Writes[4];
            Assert.Equal(LogLevel.Critical, critical.LogLevel);
            Assert.Same(testLogValues, critical.State);
            Assert.Equal(5, critical.EventId);
            Assert.Equal(null, critical.Exception);
            Assert.Equal("Test 1", critical.Formatter(critical.State, critical.Exception));

            var debug = sink.Writes[5];
            Assert.Equal(LogLevel.Debug, debug.LogLevel);
            Assert.Same(testLogValues, debug.State);
            Assert.Equal(6, debug.EventId);
            Assert.Equal(null, debug.Exception);
            Assert.Equal("Test 1", debug.Formatter(debug.State, debug.Exception));
        }

        [Fact]
        public void LogValuesAndError_LogsCorrectValues()
        {
            // Arrange
            var sink = new TestSink();
            var logger = SetUp(sink);
            var testLogValues = new TestLogValues()
            {
                Value = 1
            };

            // Act
            logger.LogVerbose(testLogValues, _exception);
            logger.LogInformation(testLogValues, _exception);
            logger.LogWarning(testLogValues, _exception);
            logger.LogError(testLogValues, _exception);
            logger.LogCritical(testLogValues, _exception);
            logger.LogDebug(testLogValues, _exception);

            // Assert
            Assert.Equal(6, sink.Writes.Count);

            var verbose = sink.Writes[0];
            Assert.Equal(LogLevel.Verbose, verbose.LogLevel);
            Assert.Same(testLogValues, verbose.State);
            Assert.Equal(0, verbose.EventId);
            Assert.Equal(_exception, verbose.Exception);
            Assert.Equal(
                "Test 1" + Environment.NewLine + _exception,
                verbose.Formatter(verbose.State, verbose.Exception));

            var information = sink.Writes[1];
            Assert.Equal(LogLevel.Information, information.LogLevel);
            Assert.Same(testLogValues, information.State);
            Assert.Equal(0, information.EventId);
            Assert.Equal(_exception, information.Exception);
            Assert.Equal(
                "Test 1" + Environment.NewLine + _exception,
                information.Formatter(information.State, information.Exception));

            var warning = sink.Writes[2];
            Assert.Equal(LogLevel.Warning, warning.LogLevel);
            Assert.Same(testLogValues, warning.State);
            Assert.Equal(0, warning.EventId);
            Assert.Equal(_exception, warning.Exception);
            Assert.Equal(
                "Test 1" + Environment.NewLine + _exception,
                warning.Formatter(warning.State, warning.Exception));

            var error = sink.Writes[3];
            Assert.Equal(LogLevel.Error, error.LogLevel);
            Assert.Same(testLogValues, error.State);
            Assert.Equal(0, error.EventId);
            Assert.Equal(_exception, error.Exception);
            Assert.Equal(
                "Test 1" + Environment.NewLine + _exception,
                error.Formatter(error.State, error.Exception));

            var critical = sink.Writes[4];
            Assert.Equal(LogLevel.Critical, critical.LogLevel);
            Assert.Same(testLogValues, critical.State);
            Assert.Equal(0, critical.EventId);
            Assert.Equal(_exception, critical.Exception);
            Assert.Equal(
                "Test 1" + Environment.NewLine + _exception,
                critical.Formatter(critical.State, critical.Exception));

            var debug = sink.Writes[5];
            Assert.Equal(LogLevel.Debug, debug.LogLevel);
            Assert.Same(testLogValues, debug.State);
            Assert.Equal(0, debug.EventId);
            Assert.Equal(_exception, debug.Exception);
            Assert.Equal(
                "Test 1" + Environment.NewLine + _exception,
                debug.Formatter(debug.State, debug.Exception));
        }

        public void LogValuesEventIdAndError_LogsCorrectValues()
        {
            // Arrange
            var sink = new TestSink();
            var logger = SetUp(sink);
            var testLogValues = new TestLogValues()
            {
                Value = 1
            };

            // Act
            logger.LogVerbose(1, testLogValues, _exception);
            logger.LogInformation(2, testLogValues, _exception);
            logger.LogWarning(3, testLogValues, _exception);
            logger.LogError(4, testLogValues, _exception);
            logger.LogCritical(5, testLogValues, _exception);
            logger.LogDebug(6, testLogValues, _exception);

            // Assert
            Assert.Equal(6, sink.Writes.Count);

            var verbose = sink.Writes[0];
            Assert.Equal(LogLevel.Verbose, verbose.LogLevel);
            Assert.Same(testLogValues, verbose.State);
            Assert.Equal(1, verbose.EventId);
            Assert.Equal(_exception, verbose.Exception);
            Assert.Equal(
                "Test 1" + Environment.NewLine + _exception,
                verbose.Formatter(verbose.State, verbose.Exception));

            var information = sink.Writes[1];
            Assert.Equal(LogLevel.Information, information.LogLevel);
            Assert.Same(testLogValues, information.State);
            Assert.Equal(2, information.EventId);
            Assert.Equal(_exception, information.Exception);
            Assert.Equal(
                "Test 1" + Environment.NewLine + _exception,
                information.Formatter(information.State, information.Exception));

            var warning = sink.Writes[2];
            Assert.Equal(LogLevel.Warning, warning.LogLevel);
            Assert.Same(testLogValues, warning.State);
            Assert.Equal(3, warning.EventId);
            Assert.Equal(_exception, warning.Exception);
            Assert.Equal(
                "Test 1" + Environment.NewLine + _exception,
                warning.Formatter(warning.State, warning.Exception));

            var error = sink.Writes[3];
            Assert.Equal(LogLevel.Error, error.LogLevel);
            Assert.Same(testLogValues, error.State);
            Assert.Equal(4, error.EventId);
            Assert.Equal(_exception, error.Exception);
            Assert.Equal(
                "Test 1" + Environment.NewLine + _exception,
                error.Formatter(error.State, error.Exception));

            var critical = sink.Writes[4];
            Assert.Equal(LogLevel.Critical, critical.LogLevel);
            Assert.Same(testLogValues, critical.State);
            Assert.Equal(5, critical.EventId);
            Assert.Equal(_exception, critical.Exception);
            Assert.Equal(
                "Test 1" + Environment.NewLine + _exception,
                critical.Formatter(critical.State, critical.Exception));

            var debug = sink.Writes[4];
            Assert.Equal(LogLevel.Debug, debug.LogLevel);
            Assert.Same(testLogValues, debug.State);
            Assert.Equal(6, debug.EventId);
            Assert.Equal(_exception, debug.Exception);
            Assert.Equal(
                "Test 1" + Environment.NewLine + _exception,
                debug.Formatter(debug.State, debug.Exception));
        }

        [Fact]
        public void BeginScope_CreatesScope_WithFormatStringValues()
        {
            // Arrange
            var testSink = new TestSink(
                writeEnabled: (writeContext) => true, 
                beginEnabled: (beginScopeContext) => true);
            var logger = new TestLogger("TestLogger", testSink, enabled: true);
            var actionName = "App.Controllers.Home.Index";
            var expectedStringMessage = "Executing action " + actionName;

            // Act
            var scope = logger.BeginScope("Executing action {ActionName}", actionName);

            // Assert
            Assert.Equal(1, testSink.Scopes.Count);
            Assert.IsType<FormattedLogValues>(testSink.Scopes[0].Scope);
            var scopeState = (FormattedLogValues)testSink.Scopes[0].Scope;
            Assert.Equal(expectedStringMessage, scopeState.ToString());
            var scopeProperties = scopeState.GetValues();
            Assert.NotNull(scopeProperties);
            Assert.Contains(scopeProperties, (kvp) =>
            {
                return (string.Equals(kvp.Key, "ActionName") && string.Equals(kvp.Value?.ToString(), actionName));
            });
        }

        private class TestLogValues : ILogValues
        {
            public int Value { get; set; }

            public IEnumerable<KeyValuePair<string, object>> GetValues()
            {
                return new[] { new KeyValuePair<string, object>(nameof(Value), Value) };
            }

            public override string ToString()
            {
                return "Test " + Value;
            }
        }
    }
}