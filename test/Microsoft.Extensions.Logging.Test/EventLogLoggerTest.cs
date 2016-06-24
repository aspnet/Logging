// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if NET451
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.AspNetCore.Testing.xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Logging.EventLog.Internal;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Microsoft.Extensions.Logging
{
    public class EventLogLoggerTest
    {
        private const string _loggerName = "Test";
        private const string _state = "This is a test, and {curly braces} are just fine!";
        private readonly Func<object, Exception, string> _defaultFormatter = (state, exception) => state.ToString();

        [Fact]
        public void CallingBeginScopeOnLogger_ReturnsNonNullableInstance()
        {
            // Arrange
            var logger = new EventLogLogger("Test");

            // Act
            var disposable = logger.BeginScope("Scope1");

            // Assert
            Assert.NotNull(disposable);
        }

        [Fact]
        public void WindowsEventLog_Constructor_CreatesWithExpectedInformation()
        {
            // Arrange
            var logName = "foo";
            var machineName = "bar";
            var sourceName = "blah";

            // Act
            var windowsEventLog = new WindowsEventLog(logName, machineName, sourceName);

            // Assert
            Assert.NotNull(windowsEventLog.DiagnosticsEventLog);
            Assert.Equal(logName, windowsEventLog.DiagnosticsEventLog.Log);
            Assert.Equal(machineName, windowsEventLog.DiagnosticsEventLog.MachineName);
            Assert.Equal(sourceName, windowsEventLog.DiagnosticsEventLog.Source);
        }

        [Fact]
        public void Constructor_CreatesWindowsEventLog_WithExpectedInformation()
        {
            // Arrange & Act
            var eventLogLogger = new EventLogLogger("Test");

            // Assert
            var windowsEventLog = Assert.IsType<WindowsEventLog>(eventLogLogger.EventLog);
            Assert.Equal(EventLogSettings.DefaultLogName, windowsEventLog.DiagnosticsEventLog.Log);
            Assert.Equal(EventLogSettings.DefaultSourceName, windowsEventLog.DiagnosticsEventLog.Source);
            Assert.Equal(EventLogSettings.DefaultMachineName, windowsEventLog.DiagnosticsEventLog.MachineName);
        }

        [Fact]
        public void Constructor_CreatesWindowsEventLog_WithSuppliedEventLogSettings()
        {
            // Arrange
            var settings = new EventLogSettings
            {
                LogName = "bar",
                SourceName = "foo",
                MachineName = "blah"
            };

            // Act
            var eventLogLogger = new EventLogLogger("Test", filter: null, includeScopes: false, eventLogSettings: settings);

            // Assert
            var windowsEventLog = Assert.IsType<WindowsEventLog>(eventLogLogger.EventLog);
            Assert.Equal(settings.LogName, windowsEventLog.DiagnosticsEventLog.Log);
            Assert.Equal(settings.SourceName, windowsEventLog.DiagnosticsEventLog.Source);
            Assert.Equal(settings.MachineName, windowsEventLog.DiagnosticsEventLog.MachineName);
        }

        [Theory]
        [InlineData(50)]
        [InlineData(49)]
        [InlineData(36)]
        public void MessageWithinMaxSize_WritesFullMessage(int messageSize)
        {
            // Arrange
            var loggerName = "Test";
            var maxMessageSize = 50 + loggerName.Length + Environment.NewLine.Length;
            var message = new string('a', messageSize);
            var expectedMessage = loggerName + Environment.NewLine + message;
            var testEventLog = new TestEventLog(maxMessageSize);
            var logger = new EventLogLogger(loggerName, filter: null, includeScopes: false, eventLog: testEventLog);

            // Act
            logger.LogInformation(message);

            // Assert
            Assert.Equal(1, testEventLog.Messages.Count);
            Assert.Equal(expectedMessage, testEventLog.Messages[0]);
        }

        public static TheoryData<int, string[]> WritesSplitMessagesData
        {
            get
            {
                var loggerName = "Test";

                return new TheoryData<int, string[]>
                {
                    // loggername + newline combined length is 7
                    {
                        1,
                        new[]
                        {
                            loggerName + Environment.NewLine + "a"
                        }
                    },
                    {
                        5,
                        new[]
                        {
                            loggerName + Environment.NewLine + "a...",
                            "...aaaa"
                        }
                    },
                    {
                        10, // equaling the max message size
                        new[]
                        {
                            loggerName + Environment.NewLine + "a...",
                            "...aaaa...",
                            "...aaaaa"
                        }
                    },
                    {
                        15,
                        new[]
                        {
                            loggerName + Environment.NewLine + "a...",
                            "...aaaa...",
                            "...aaaa...",
                            "...aaaaaa"
                        }
                    }
                };
            }
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        [MemberData(nameof(WritesSplitMessagesData))]
        public void MessageExceedingMaxSize_WritesSplitMessages(int messageSize, string[] expectedMessages)
        {
            // Arrange
            var loggerName = "Test";
            var maxMessageSize = 10;
            var message = new string('a', messageSize);
            var testEventLog = new TestEventLog(maxMessageSize);
            var logger = new EventLogLogger(loggerName, filter: null, includeScopes: false, eventLog: testEventLog);

            // Act
            logger.LogInformation(message);

            // Assert
            Assert.Equal(expectedMessages.Length, testEventLog.Messages.Count);
            Assert.Equal(expectedMessages, testEventLog.Messages);
        }

        [Fact]
        public void LogsWhenNullFilterGiven()
        {
            // Arrange
            var maxMessageSize = _state.Length + _loggerName.Length + Environment.NewLine.Length;
            var logger = new EventLogLogger(_loggerName, filter: null, includeScopes: false);
            var sink = new TestEventLog(maxMessageSize);
            logger.EventLog = sink;

            var expectedMessage = _loggerName + Environment.NewLine + _state;

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
            var maxMessageSize = _state.Length + _loggerName.Length + Environment.NewLine.Length;
            var logger = new EventLogLogger(_loggerName, filter: (category, logLevel) => logLevel >= LogLevel.Critical, includeScopes: false);
            var sink = new TestEventLog(maxMessageSize);
            logger.EventLog = sink;

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
            var maxMessageSize = _state.Length + _loggerName.Length + Environment.NewLine.Length;
            var logger = new EventLogLogger(_loggerName, filter: (category, logLevel) => logLevel >= LogLevel.Error, includeScopes: false);
            var sink = new TestEventLog(maxMessageSize);
            logger.EventLog = sink;

            // Act
            logger.Log(LogLevel.Warning, 0, _state, null, _defaultFormatter);

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
            var maxMessageSize = _state.Length + _loggerName.Length + Environment.NewLine.Length;
            var logger = new EventLogLogger(_loggerName, filter: (category, logLevel) => logLevel >= LogLevel.Warning, includeScopes: false);
            var sink = new TestEventLog(maxMessageSize);
            logger.EventLog = sink;

            // Act
            logger.Log(LogLevel.Information, 0, _state, null, _defaultFormatter);

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
            var maxMessageSize = _state.Length + _loggerName.Length + Environment.NewLine.Length;
            var logger = new EventLogLogger(_loggerName, filter: (category, logLevel) => logLevel >= LogLevel.Information, includeScopes: false);
            var sink = new TestEventLog(maxMessageSize);
            logger.EventLog = sink;

            // Act
            logger.Log(LogLevel.Debug, 0, _state, null, _defaultFormatter);

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
            var maxMessageSize = _state.Length + _loggerName.Length + Environment.NewLine.Length;
            var logger = new EventLogLogger(_loggerName, filter: (category, logLevel) => logLevel >= LogLevel.Debug, includeScopes: false);
            var sink = new TestEventLog(maxMessageSize);
            logger.EventLog = sink;

            // Act
            logger.Log(LogLevel.Trace, 0, _state, null, _defaultFormatter);

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
            var maxMessageSize = _state.Length + _loggerName.Length + Environment.NewLine.Length;
            var logger = new EventLogLogger(_loggerName, filter: (category, logLevel) => logLevel >= LogLevel.Trace, includeScopes: false);
            var sink = new TestEventLog(maxMessageSize);
            logger.EventLog = sink;

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
        public void EventLogger_ReloadSettings_CanChangeLogLevel()
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

            var provider = new EventLogLoggerProvider(settings);
            var logger = (EventLogLogger)provider.CreateLogger("Test");
            logger.EventLog = new TestEventLog(100);

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
        public void EventLogLogger_ReloadSettings_CanReloadMultipleTimes()
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

            var provider = new EventLogLoggerProvider(settings);
            var logger = (EventLogLogger)provider.CreateLogger("Test");
            logger.EventLog = new TestEventLog(100);

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

        private class TestEventLog : IEventLog
        {
            public TestEventLog(int maxMessageSize)
            {
                MaxMessageSize = maxMessageSize;
                Messages = new List<string>();
            }

            public int MaxMessageSize { get; }

            public List<string> Messages { get; }

            public void WriteEntry(string message, EventLogEntryType type, int eventID, short category)
            {
                Messages.Add(message);
            }
        }
    }
}
#endif