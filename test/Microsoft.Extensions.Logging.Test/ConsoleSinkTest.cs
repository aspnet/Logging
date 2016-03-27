// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Test.Console;
using Microsoft.Extensions.Primitives;
using System.Threading;
using Xunit;
using Microsoft.Extensions.Logging.Internal;

namespace Microsoft.Extensions.Logging.Test
{
    public class ConsoleSinkTest
    {
        private readonly string _paddingString;
        private const string _categoryName = "test";
        private const string _state = "This is a test, and {curly braces} are just fine!";
        private Func<object, Exception, string> _defaultFormatter = (state, exception) => state.ToString();

        private Tuple<ConsoleSink, ConsoleWriter> SetUp(Func<string, LogLevel, bool> filter, bool includeScopes = false)
        {
            // Arrange
            var writer = new ConsoleWriter();
            var console = new TestConsole(writer);
            var sink = new ConsoleSink(filter, includeScopes);
            sink.Console = console;
            return new Tuple<ConsoleSink, ConsoleWriter>(sink, writer);
        }

        public ConsoleSinkTest()
        {
            var loglevelStringWithPadding = "INFO: ";
            _paddingString = new string(' ', loglevelStringWithPadding.Length);
        }

        [Fact]
        public void ThrowsException_WhenNoMessageAndExceptionAreProvided()
        {
            // Arrange
            var t = SetUp(null);
            var sink = t.Item1;
            var writer = t.Item2;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => sink.Log(_categoryName, LogLevel.Critical, 0, null, null, _defaultFormatter));
        }

        [Fact]
        public void DoesNotLog_NewLine_WhenNoExceptionIsProvided()
        {
            // Arrange
            var t = SetUp(null);
            var sink = t.Item1;
            var writer = t.Item2;
            var logMessage = "Route with name 'Default' was not found.";
            var expectedMessage = _paddingString + logMessage + Environment.NewLine;

            // Act
            sink.Log(_categoryName, LogLevel.Critical, 10, logMessage, null, _defaultFormatter);
            sink.Log(_categoryName, LogLevel.Critical, 10, logMessage, null, _defaultFormatter);
            sink.Log(_categoryName, LogLevel.Critical, 10, logMessage, null, _defaultFormatter);
            sink.Log(_categoryName, LogLevel.Critical, 10, logMessage, null, _defaultFormatter);

            // Assert
            Assert.Equal(12, writer.Writes.Count);
            Assert.Equal(expectedMessage, writer.Writes[2].Message);
            Assert.Equal(expectedMessage, writer.Writes[5].Message);
            Assert.Equal(expectedMessage, writer.Writes[8].Message);
            Assert.Equal(expectedMessage, writer.Writes[11].Message);
        }

        [Theory]
        [InlineData("Route with name 'Default' was not found.")]
        public void Writes_NewLine_WhenExceptionIsProvided(string message)
        {
            // Arrange
            var t = SetUp(null);
            var sink = t.Item1;
            var writer = t.Item2;
            var eventId = 10;
            var exception = new InvalidOperationException("Invalid value");
            var expectedMessage =
                _paddingString + message + Environment.NewLine;
            var expectedExceptionMessage =
                exception.ToString() + Environment.NewLine;

            // Act
            sink.Log(_categoryName, LogLevel.Critical, eventId, message, exception, _defaultFormatter);

            // Assert
            Assert.Equal(4, writer.Writes.Count);
            Assert.Equal(expectedMessage, writer.Writes[2].Message);
            Assert.Equal(expectedExceptionMessage, writer.Writes[3].Message);
        }

        [Fact]
        public void ThrowsException_WhenNoMessageIsProvided()
        {
            // Arrange
            var t = SetUp(null);
            var sink = t.Item1;
            var exception = new InvalidOperationException("Invalid value");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => sink.Log(_categoryName, LogLevel.Critical, 10, null, exception, _defaultFormatter));
        }

        [Fact]
        public void ThrowsException_WhenNoFormatterIsProvided()
        {
            // Arrange
            var t = SetUp(null);
            var sink = t.Item1;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => sink.Log<object>(_categoryName, LogLevel.Trace, 1, "empty", new Exception(), null));
        }

        [Fact]
        public void LogsWhenNullFilterGiven()
        {
            // Arrange
            var t = SetUp(null);
            var sink = t.Item1;
            var writer = t.Item2;
            var expectedMessage = _paddingString + _state + Environment.NewLine;

            // Act
            sink.Log(_categoryName, LogLevel.Information, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(3, writer.Writes.Count);
            Assert.Equal(expectedMessage, writer.Writes[2].Message);
        }

        [Fact]
        public void CriticalFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, logLevel) => logLevel >= LogLevel.Critical);
            var sink = t.Item1;
            var writer = t.Item2;

            // Act
            sink.Log(_categoryName, LogLevel.Warning, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(0, writer.Writes.Count);

            // Act
            sink.Log(_categoryName, LogLevel.Critical, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(3, writer.Writes.Count);
        }

        [Fact]
        public void ErrorFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, logLevel) => logLevel >= LogLevel.Error);
            var sink = t.Item1;
            var writer = t.Item2;

            // Act
            sink.Log(_categoryName, LogLevel.Warning, 0, _state, null, null);

            // Assert
            Assert.Equal(0, writer.Writes.Count);

            // Act
            sink.Log(_categoryName, LogLevel.Error, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(3, writer.Writes.Count);
        }

        [Fact]
        public void WarningFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, logLevel) => logLevel >= LogLevel.Warning);
            var sink = t.Item1;
            var writer = t.Item2;

            // Act
            sink.Log(_categoryName, LogLevel.Information, 0, _state, null, null);

            // Assert
            Assert.Equal(0, writer.Writes.Count);

            // Act
            sink.Log(_categoryName, LogLevel.Warning, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(3, writer.Writes.Count);
        }

        [Fact]
        public void InformationFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, logLevel) => logLevel >= LogLevel.Information);
            var sink = t.Item1;
            var writer = t.Item2;

            // Act
            sink.Log(_categoryName, LogLevel.Debug, 0, _state, null, null);

            // Assert
            Assert.Equal(0, writer.Writes.Count);

            // Act
            sink.Log(_categoryName, LogLevel.Information, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(3, writer.Writes.Count);
        }

        [Fact]
        public void DebugFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, logLevel) => logLevel >= LogLevel.Debug);
            var sink = t.Item1;
            var writer = t.Item2;

            // Act
            sink.Log(_categoryName, LogLevel.Trace, 0, _state, null, null);

            // Assert
            Assert.Equal(0, writer.Writes.Count);

            // Act
            sink.Log(_categoryName, LogLevel.Debug, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(3, writer.Writes.Count);
        }

        [Fact]
        public void TraceFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, logLevel) => logLevel >= LogLevel.Trace);
            var sink = t.Item1;
            var writer = t.Item2;

            // Act
            sink.Log(_categoryName, LogLevel.Critical, 0, _state, null, _defaultFormatter);
            sink.Log(_categoryName, LogLevel.Error, 0, _state, null, _defaultFormatter);
            sink.Log(_categoryName, LogLevel.Warning, 0, _state, null, _defaultFormatter);
            sink.Log(_categoryName, LogLevel.Information, 0, _state, null, _defaultFormatter);
            sink.Log(_categoryName, LogLevel.Debug, 0, _state, null, _defaultFormatter);
            sink.Log(_categoryName, LogLevel.Trace, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(18, writer.Writes.Count);
        }

        [Fact]
        public void WriteCritical_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var sink = t.Item1;
            var writer = t.Item2;

            // Act
            sink.Log(_categoryName, LogLevel.Critical, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(3, writer.Writes.Count);
            var write = writer.Writes[0];
            Assert.Equal(ConsoleColor.Red, write.BackgroundColor);
            Assert.Equal(ConsoleColor.White, write.ForegroundColor);
            write = writer.Writes[1];
            Assert.Equal(ConsoleColor.Red, write.BackgroundColor);
            Assert.Equal(ConsoleColor.White, write.ForegroundColor);
            write = writer.Writes[2];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WriteError_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var sink = t.Item1;
            var writer = t.Item2;

            // Act
            sink.Log(_categoryName, LogLevel.Error, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(3, writer.Writes.Count);
            var write = writer.Writes[0];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Red, write.ForegroundColor);
            write = writer.Writes[1];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Red, write.ForegroundColor);
            write = writer.Writes[2];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WriteWarning_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var sink = t.Item1;
            var writer = t.Item2;

            // Act
            sink.Log(_categoryName, LogLevel.Warning, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(3, writer.Writes.Count);
            var write = writer.Writes[0];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Yellow, write.ForegroundColor);
            write = writer.Writes[1];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Yellow, write.ForegroundColor);
            write = writer.Writes[2];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WriteInformation_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var sink = t.Item1;
            var writer = t.Item2;

            // Act
            sink.Log(_categoryName, LogLevel.Information, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(3, writer.Writes.Count);
            var write = writer.Writes[0];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.DarkGreen, write.ForegroundColor);
            write = writer.Writes[1];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.DarkGreen, write.ForegroundColor);
            write = writer.Writes[2];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WriteDebug_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var sink = t.Item1;
            var writer = t.Item2;

            // Act
            sink.Log(_categoryName, LogLevel.Debug, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(3, writer.Writes.Count);
            var write = writer.Writes[0];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Gray, write.ForegroundColor);
            write = writer.Writes[1];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Gray, write.ForegroundColor);
            write = writer.Writes[2];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WriteTrace_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var sink = t.Item1;
            var writer = t.Item2;

            // Act
            sink.Log(_categoryName, LogLevel.Trace, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(3, writer.Writes.Count);
            var write = writer.Writes[0];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Gray, write.ForegroundColor);
            write = writer.Writes[1];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Gray, write.ForegroundColor);
            write = writer.Writes[2];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WriteCore_LogsCorrectMessages()
        {
            // Arrange
            var t = SetUp(null);
            var sink = t.Item1;
            var writer = t.Item2;
            var ex = new Exception();

            // Act
            sink.Log(_categoryName, LogLevel.Critical, 0, _state, ex, _defaultFormatter);
            sink.Log(_categoryName, LogLevel.Error, 0, _state, ex, _defaultFormatter);
            sink.Log(_categoryName, LogLevel.Warning, 0, _state, ex, _defaultFormatter);
            sink.Log(_categoryName, LogLevel.Information, 0, _state, ex, _defaultFormatter);
            sink.Log(_categoryName, LogLevel.Debug, 0, _state, ex, _defaultFormatter);
            sink.Log(_categoryName, LogLevel.Trace, 0, _state, ex, _defaultFormatter);

            // Assert
            Assert.Equal(24, writer.Writes.Count);
            Assert.Equal(GetMessage("crit", 0, ex), GetMessage(writer.Writes.GetRange(0, 4)));
            Assert.Equal(GetMessage("fail", 0, ex), GetMessage(writer.Writes.GetRange(4, 4)));
            Assert.Equal(GetMessage("warn", 0, ex), GetMessage(writer.Writes.GetRange(8, 4)));
            Assert.Equal(GetMessage("info", 0, ex), GetMessage(writer.Writes.GetRange(12, 4)));
            Assert.Equal(GetMessage("dbug", 0, ex), GetMessage(writer.Writes.GetRange(16, 4)));
            Assert.Equal(GetMessage("trce", 0, ex), GetMessage(writer.Writes.GetRange(20, 4)));
        }

        [Fact]
        public void NoLogScope_DoesNotWriteAnyScopeContentToOutput()
        {
            // Arrange
            var t = SetUp(filter: null, includeScopes: true);
            var sink = t.Item1;
            var writer = t.Item2;

            // Act
            sink.Log(_categoryName, LogLevel.Warning, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(3, writer.Writes.Count);
            var write = writer.Writes[0];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Yellow, write.ForegroundColor);
            write = writer.Writes[1];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Yellow, write.ForegroundColor);
            write = writer.Writes[2];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WritingScopes_LogsWithCorrectColors()
        {
            // Arrange
            var t = SetUp(filter: null, includeScopes: true);
            var sink = t.Item1;
            var writer = t.Item2;
            var id = Guid.NewGuid();
            var scopeMessage = "RequestId: {RequestId}";

            // Act
            using (sink.BeginScope(_categoryName, new FormattedLogValues(scopeMessage, id)))
            {
                sink.Log(_categoryName, LogLevel.Information, 0, _state, null, _defaultFormatter);
            }

            // Assert
            Assert.Equal(4, writer.Writes.Count);
            var write = writer.Writes[0];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.DarkGreen, write.ForegroundColor);
            write = writer.Writes[1];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.DarkGreen, write.ForegroundColor);
            write = writer.Writes[2];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
            write = writer.Writes[3];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WritingScopes_LogsExpectedMessage()
        {
            // Arrange
            var t = SetUp(filter: null, includeScopes: true);
            var sink = t.Item1;
            var writer = t.Item2;
            var expectedMessage =
                _paddingString
                + $"=> RequestId: 100"
                + Environment.NewLine;

            // Act
            using (sink.BeginScope(_categoryName, new FormattedLogValues("RequestId: {RequestId}", 100)))
            {
                sink.Log(_categoryName, LogLevel.Information, 0, _state, null, _defaultFormatter);
            }

            // Assert
            Assert.Equal(4, writer.Writes.Count);
            // scope
            var write = writer.Writes[2];
            Assert.Equal(expectedMessage, write.Message);
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WritingNestedScopes_LogsExpectedMessage()
        {
            // Arrange
            var t = SetUp(filter: null, includeScopes: true);
            var sink = t.Item1;
            var writer = t.Item2;
            var expectedMessage =
                _paddingString
                + $"=> RequestId: 100 => Request matched action: Index"
                + Environment.NewLine;

            // Act
            using (sink.BeginScope(_categoryName, new FormattedLogValues("RequestId: {RequestId}", 100)))
            {
                using (sink.BeginScope(_categoryName, new FormattedLogValues("Request matched action: {ActionName}", "Index")))
                {
                    sink.Log(_categoryName, LogLevel.Information, 0, _state, null, _defaultFormatter);
                }
            }

            // Assert
            Assert.Equal(4, writer.Writes.Count);
            // scope
            var write = writer.Writes[2];
            Assert.Equal(expectedMessage, write.Message);
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WritingMultipleScopes_LogsExpectedMessage()
        {
            // Arrange
            var t = SetUp(filter: null, includeScopes: true);
            var sink = t.Item1;
            var writer = t.Item2;
            var expectedMessage1 =
                _paddingString
                + $"=> RequestId: 100 => Request matched action: Index"
                + Environment.NewLine;
            var expectedMessage2 =
                _paddingString
                + $"=> RequestId: 100 => Created product: Car"
                + Environment.NewLine;

            // Act
            using (sink.BeginScope(_categoryName, new FormattedLogValues("RequestId: {RequestId}", 100)))
            {
                using (sink.BeginScope(_categoryName, new FormattedLogValues("Request matched action: {ActionName}", "Index")))
                {
                    sink.Log(_categoryName, LogLevel.Information, 0, _state, null, _defaultFormatter);
                }

                using (sink.BeginScope(_categoryName, new FormattedLogValues("Created product: {ProductName}", "Car")))
                {
                    sink.Log(_categoryName, LogLevel.Information, 0, _state, null, _defaultFormatter);
                }
            }

            // Assert
            Assert.Equal(8, writer.Writes.Count);
            // scope
            var write = writer.Writes[2];
            Assert.Equal(expectedMessage1, write.Message);
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
            write = writer.Writes[6];
            Assert.Equal(expectedMessage2, write.Message);
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void CallingBeginScopeOnLogger_AlwaysReturnsNewDisposableInstance()
        {
            // Arrange
            var t = SetUp(null);
            var sink = t.Item1;
            var writer = t.Item2;

            // Act
            var disposable1 = sink.BeginScope(_categoryName, "Scope1");
            var disposable2 = sink.BeginScope(_categoryName, "Scope2");

            // Assert
            Assert.NotNull(disposable1);
            Assert.NotNull(disposable2);
            Assert.NotSame(disposable1, disposable2);
        }

        [Fact]
        public void CallingBeginScopeOnLogger_ReturnsNonNullableInstance()
        {
            // Arrange
            var t = SetUp(null);
            var sink = t.Item1;
            var writer = t.Item2;

            // Act
            var disposable = sink.BeginScope(_categoryName, "Scope1");

            // Assert
            Assert.NotNull(disposable);
        }

        [Fact]
        public void ConsoleLogger_ReloadSettings_CanChangeLogLevel()
        {
            // Arrange
            var settings = new MockConsoleLoggerSettings()
            {
                Cancel = new CancellationTokenSource(),
                Switches =
                {
                    ["Test"] = LogLevel.Information,
                }
            };

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(settings);

            var logger = loggerFactory.CreateLogger("Test");
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
        public void ConsoleLogger_ReloadSettings_CanReloadMultipleTimes()
        {
            // Arrange
            var settings = new MockConsoleLoggerSettings()
            {
                Cancel = new CancellationTokenSource(),
                Switches =
                {
                    ["Test"] = LogLevel.Information,
                }
            };

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(settings);

            var logger = loggerFactory.CreateLogger("Test");
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

        private string GetMessage(string logLevelString, int eventId, Exception exception)
        {
            var loglevelStringWithPadding = $"{logLevelString}: ";

            return
                loglevelStringWithPadding + $"{_categoryName}[{eventId}]" + Environment.NewLine
                + _paddingString + ReplaceMessageNewLinesWithPadding(_state.ToString())
                + Environment.NewLine + ReplaceMessageNewLinesWithPadding(exception.ToString()) + Environment.NewLine;
        }

        private string ReplaceMessageNewLinesWithPadding(string message)
        {
            return message.Replace(Environment.NewLine, Environment.NewLine + _paddingString);
        }

        private string GetMessage(List<ConsoleContext> contexts)
        {
            return string.Join("", contexts.Select(c => c.Message));
        }

        private class MockConsoleLoggerSettings : IConsoleLoggerSettings
        {
            public CancellationTokenSource Cancel { get; set; }

            public IChangeToken ChangeToken => new CancellationChangeToken(Cancel.Token);

            public IDictionary<string, LogLevel> Switches { get; } = new Dictionary<string, LogLevel>();

            public bool IncludeScopes { get; set; }

            public IConsoleLoggerSettings Reload()
            {
                return this;
            }

            public bool TryGetSwitch(string name, out LogLevel level)
            {
                return Switches.TryGetValue(name, out level);
            }
        }
    }
}