// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Test.Console;
using Microsoft.Extensions.Primitives;
using System.Threading;
using Moq;
using Xunit;

namespace Microsoft.Extensions.Logging.Test
{
    public class ConsoleLoggerTest
    {
        private const int WritesPerMsg = 2;
        private readonly string _paddingString;
        private const string _loggerName = "test";
        private const string _state = "This is a test, and {curly braces} are just fine!";
        private Func<object, Exception, string> _defaultFormatter = (state, exception) => state.ToString();

        private static Tuple<ConsoleLogger, ConsoleSink> SetUp(Func<string, LogLevel, bool> filter, bool includeScopes = false)
        {
            // Arrange
            var sink = new ConsoleSink();
            var console = new TestConsole(sink);
            var logger = new ConsoleLogger(_loggerName, filter, includeScopes);
            logger.Console = console;
            return new Tuple<ConsoleLogger, ConsoleSink>(logger, sink);
        }

        public ConsoleLoggerTest()
        {
            var loglevelStringWithPadding = "INFO: ";
            _paddingString = new string(' ', loglevelStringWithPadding.Length);
        }

        private Tuple<ILoggerFactory, ConsoleSink> SetUpFactory(Func<string, LogLevel, bool> filter)
        {
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;

            var provider = new Mock<ILoggerProvider>();
            provider.Setup(f => f.CreateLogger(
                It.IsAny<string>()))
                .Returns(logger);

            var factory = new LoggerFactory();
            factory.AddProvider(provider.Object);

            return new Tuple<ILoggerFactory, ConsoleSink>(factory, sink);
        }

        [Fact]
        public void LogsWhenMessageIsNotProvided()
        {
            // Arrange
            var t = SetUp(null);
            var logger = (ILogger)t.Item1;
            var sink = t.Item2;
            var exception = new InvalidOperationException("Invalid value");

            // Act
            logger.LogCritical(eventId: 0, exception: null, message: null);
            logger.LogCritical(eventId: 0, message: null);
            logger.LogCritical(eventId: 0, message: null, exception: exception);

            // Assert
            Assert.Equal(6, sink.Writes.Count);
            Assert.Equal(GetMessage("crit", 0, "[null]", null     ), GetMessage(sink.Writes.GetRange(0 * WritesPerMsg, WritesPerMsg)));
            Assert.Equal(GetMessage("crit", 0, "[null]", null     ), GetMessage(sink.Writes.GetRange(1 * WritesPerMsg, WritesPerMsg)));
            Assert.Equal(GetMessage("crit", 0, "[null]", exception), GetMessage(sink.Writes.GetRange(2 * WritesPerMsg, WritesPerMsg)));
        }

        [Fact]
        public void DoesNotLog_NewLine_WhenNoExceptionIsProvided()
        {
            // Arrange
            var t = SetUp(null);
            var logger = (ILogger)t.Item1;
            var sink = t.Item2;
            var logMessage = "Route with name 'Default' was not found.";

            // Act
            logger.LogCritical(logMessage);
            logger.LogCritical(eventId: 10, message: logMessage, exception: null);
            logger.LogCritical(eventId: 10, message: logMessage);
            logger.LogCritical(eventId: 10, message: logMessage, exception: null);

            // Assert
            Assert.Equal(8, sink.Writes.Count);
            Assert.Equal(GetMessage("crit",  0, logMessage, null), GetMessage(sink.Writes.GetRange(0 * WritesPerMsg, WritesPerMsg)));
            Assert.Equal(GetMessage("crit", 10, logMessage, null), GetMessage(sink.Writes.GetRange(1 * WritesPerMsg, WritesPerMsg)));
            Assert.Equal(GetMessage("crit", 10, logMessage, null), GetMessage(sink.Writes.GetRange(2 * WritesPerMsg, WritesPerMsg)));
            Assert.Equal(GetMessage("crit", 10, logMessage, null), GetMessage(sink.Writes.GetRange(3 * WritesPerMsg, WritesPerMsg)));
        }

        [Theory]
        [InlineData("Route with name 'Default' was not found.")]
        public void Writes_NewLine_WhenExceptionIsProvided(string message)
        {
            // Arrange
            var t = SetUp(null);
            var logger = (ILogger)t.Item1;
            var sink = t.Item2;
            var eventId = 10;
            var exception = new InvalidOperationException("Invalid value");
            var expectedHeader = CreateHeader(eventId);
            var expectedMessage =
                _paddingString + message + Environment.NewLine;
            var expectedExceptionMessage =
                exception.ToString() + Environment.NewLine;

            // Act
            logger.LogCritical(eventId, exception, message);

            // Assert
            Assert.Equal(2, sink.Writes.Count);
            Assert.Equal(expectedHeader + expectedMessage + expectedExceptionMessage, sink.Writes[1].Message);
        }

        [Fact]
        public void ThrowsException_WhenNoFormatterIsProvided()
        {
            // Arrange
            var t = SetUp(null);
            var logger = (ILogger)t.Item1;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => logger.Log<object>(LogLevel.Trace, 1, "empty", new Exception(), null));
        }

        [Fact]
        public void LogsWhenNullFilterGiven()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;
            var expectedHeader = CreateHeader(0);
            var expectedMessage =
                    _paddingString
                    + _state
                    + Environment.NewLine;

            // Act
            logger.Log(LogLevel.Information, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(2, sink.Writes.Count);
            Assert.Equal(expectedHeader + expectedMessage, sink.Writes[1].Message);
        }

        [Fact]
        public void CriticalFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, logLevel) => logLevel >= LogLevel.Critical);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Log(LogLevel.Warning, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(0, sink.Writes.Count);

            // Act
            logger.Log(LogLevel.Critical, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(2, sink.Writes.Count);
        }

        [Fact]
        public void ErrorFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, logLevel) => logLevel >= LogLevel.Error);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Log(LogLevel.Warning, 0, _state, null, null);

            // Assert
            Assert.Equal(0, sink.Writes.Count);

            // Act
            logger.Log(LogLevel.Error, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(2, sink.Writes.Count);
        }

        [Fact]
        public void WarningFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, logLevel) => logLevel >= LogLevel.Warning);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Log(LogLevel.Information, 0, _state, null, null);

            // Assert
            Assert.Equal(0, sink.Writes.Count);

            // Act
            logger.Log(LogLevel.Warning, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(2, sink.Writes.Count);
        }

        [Fact]
        public void InformationFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, logLevel) => logLevel >= LogLevel.Information);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Log(LogLevel.Debug, 0, _state, null, null);

            // Assert
            Assert.Equal(0, sink.Writes.Count);

            // Act
            logger.Log(LogLevel.Information, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(2, sink.Writes.Count);
        }

        [Fact]
        public void DebugFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, logLevel) => logLevel >= LogLevel.Debug);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Log(LogLevel.Trace, 0, _state, null, null);

            // Assert
            Assert.Equal(0, sink.Writes.Count);

            // Act
            logger.Log(LogLevel.Debug, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(2, sink.Writes.Count);
        }

        [Fact]
        public void TraceFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, logLevel) => logLevel >= LogLevel.Trace);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Log(LogLevel.Critical, 0, _state, null, _defaultFormatter);
            logger.Log(LogLevel.Error, 0, _state, null, _defaultFormatter);
            logger.Log(LogLevel.Warning, 0, _state, null, _defaultFormatter);
            logger.Log(LogLevel.Information, 0, _state, null, _defaultFormatter);
            logger.Log(LogLevel.Debug, 0, _state, null, _defaultFormatter);
            logger.Log(LogLevel.Trace, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(12, sink.Writes.Count);
        }

        [Fact]
        public void WriteCritical_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Log(LogLevel.Critical, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(2, sink.Writes.Count);
            var write = sink.Writes[0];
            Assert.Equal(ConsoleColor.Red, write.BackgroundColor);
            Assert.Equal(ConsoleColor.White, write.ForegroundColor);
            write = sink.Writes[1];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WriteError_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Log(LogLevel.Error, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(2, sink.Writes.Count);
            var write = sink.Writes[0];
            Assert.Equal(ConsoleColor.Red, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Black, write.ForegroundColor);
            write = sink.Writes[1];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WriteWarning_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Log(LogLevel.Warning, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(2, sink.Writes.Count);
            var write = sink.Writes[0];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Yellow, write.ForegroundColor);
            write = sink.Writes[1];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WriteInformation_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Log(LogLevel.Information, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(2, sink.Writes.Count);
            var write = sink.Writes[0];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.DarkGreen, write.ForegroundColor);
            write = sink.Writes[1];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WriteDebug_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Log(LogLevel.Debug, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(2, sink.Writes.Count);
            var write = sink.Writes[0];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Gray, write.ForegroundColor);
            write = sink.Writes[1];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WriteTrace_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Log(LogLevel.Trace, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(2, sink.Writes.Count);
            var write = sink.Writes[0];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Gray, write.ForegroundColor);
            write = sink.Writes[1];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WriteCore_LogsCorrectMessages()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;
            var ex = new Exception("Exception message" + Environment.NewLine + "with a second line");

            // Act
            logger.Log(LogLevel.Critical, 0, _state, ex, _defaultFormatter);
            logger.Log(LogLevel.Error, 0, _state, ex, _defaultFormatter);
            logger.Log(LogLevel.Warning, 0, _state, ex, _defaultFormatter);
            logger.Log(LogLevel.Information, 0, _state, ex, _defaultFormatter);
            logger.Log(LogLevel.Debug, 0, _state, ex, _defaultFormatter);
            logger.Log(LogLevel.Trace, 0, _state, ex, _defaultFormatter);

            // Assert
            Assert.Equal(12, sink.Writes.Count);
            Assert.Equal(GetMessage("crit", 0, ex), GetMessage(sink.Writes.GetRange(0 * WritesPerMsg, WritesPerMsg)));
            Assert.Equal(GetMessage("fail", 0, ex), GetMessage(sink.Writes.GetRange(1 * WritesPerMsg, WritesPerMsg)));
            Assert.Equal(GetMessage("warn", 0, ex), GetMessage(sink.Writes.GetRange(2 * WritesPerMsg, WritesPerMsg)));
            Assert.Equal(GetMessage("info", 0, ex), GetMessage(sink.Writes.GetRange(3 * WritesPerMsg, WritesPerMsg)));
            Assert.Equal(GetMessage("dbug", 0, ex), GetMessage(sink.Writes.GetRange(4 * WritesPerMsg, WritesPerMsg)));
            Assert.Equal(GetMessage("trce", 0, ex), GetMessage(sink.Writes.GetRange(5 * WritesPerMsg, WritesPerMsg)));
        }

        [Fact]
        public void NoLogScope_DoesNotWriteAnyScopeContentToOutput()
        {
            // Arrange
            var t = SetUp(filter: null, includeScopes: true);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Log(LogLevel.Warning, 0, _state, null, _defaultFormatter);

            // Assert
            Assert.Equal(2, sink.Writes.Count);
            var write = sink.Writes[0];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Yellow, write.ForegroundColor);
            write = sink.Writes[1];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WritingScopes_LogsWithCorrectColors()
        {
            // Arrange
            var t = SetUp(filter: null, includeScopes: true);
            var logger = t.Item1;
            var sink = t.Item2;
            var id = Guid.NewGuid();
            var scopeMessage = "RequestId: {RequestId}";

            // Act
            using (logger.BeginScope(scopeMessage, id))
            {
                logger.Log(LogLevel.Information, 0, _state, null, _defaultFormatter);
            }

            // Assert
            Assert.Equal(2, sink.Writes.Count);
            var write = sink.Writes[0];
            Assert.Equal(ConsoleColor.Black, write.BackgroundColor);
            Assert.Equal(ConsoleColor.DarkGreen, write.ForegroundColor);
            write = sink.Writes[1];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WritingScopes_LogsExpectedMessage()
        {
            // Arrange
            var t = SetUp(filter: null, includeScopes: true);
            var logger = t.Item1;
            var sink = t.Item2;
            var expectedHeader = CreateHeader(0);
            var expectedScope =
                _paddingString
                + "=> RequestId: 100"
                + Environment.NewLine;
            var expectedMessage = _paddingString + _state + Environment.NewLine;

            // Act
            using (logger.BeginScope("RequestId: {RequestId}", 100))
            {
                logger.Log(LogLevel.Information, 0, _state, null, _defaultFormatter);
            }

            // Assert
            Assert.Equal(2, sink.Writes.Count);
            // scope
            var write = sink.Writes[1];
            Assert.Equal(expectedHeader + expectedScope + expectedMessage, write.Message);
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WritingNestedScope_LogsNullScopeName()
        {
            // Arrange
            var t = SetUp(filter: null, includeScopes: true);
            var logger = t.Item1;
            var sink = t.Item2;
            var expectedHeader = CreateHeader(0);
            var expectedScope =
                _paddingString
                + "=> [null] => Request matched action: (null)"
                + Environment.NewLine;
            var expectedMessage = _paddingString + _state + Environment.NewLine;

            // Act
            using (logger.BeginScope(null))
            {
                using (logger.BeginScope("Request matched action: {ActionName}", new object[] { null }))
                {
                    logger.Log(LogLevel.Information, 0, _state, null, _defaultFormatter);
                }
            }

            // Assert
            Assert.Equal(2, sink.Writes.Count);
            // scope
            var write = sink.Writes[1];
            Assert.Equal(expectedHeader + expectedScope + expectedMessage, write.Message);
        }

        [Fact]
        public void WritingNestedScopes_LogsExpectedMessage()
        {
            // Arrange
            var t = SetUp(filter: null, includeScopes: true);
            var logger = t.Item1;
            var sink = t.Item2;
            var expectedHeader = CreateHeader(0);
            var expectedScope =
                _paddingString
                + "=> RequestId: 100 => Request matched action: Index"
                + Environment.NewLine;
            var expectedMessage = _paddingString + _state + Environment.NewLine;

            // Act
            using (logger.BeginScope("RequestId: {RequestId}", 100))
            {
                using (logger.BeginScope("Request matched action: {ActionName}", "Index"))
                {
                    logger.Log(LogLevel.Information, 0, _state, null, _defaultFormatter);
                }
            }

            // Assert
            Assert.Equal(2, sink.Writes.Count);
            // scope
            var write = sink.Writes[1];
            Assert.Equal(expectedHeader + expectedScope + expectedMessage, write.Message);
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void WritingMultipleScopes_LogsExpectedMessage()
        {
            // Arrange
            var t = SetUp(filter: null, includeScopes: true);
            var logger = t.Item1;
            var sink = t.Item2;
            var expectedHeader = CreateHeader(0);
            var expectedMessage = _paddingString + _state + Environment.NewLine;
            var expectedScope1 =
                _paddingString
                + "=> RequestId: 100 => Request matched action: Index"
                + Environment.NewLine;
            var expectedScope2 =
                _paddingString
                + "=> RequestId: 100 => Created product: Car"
                + Environment.NewLine;

            // Act
            using (logger.BeginScope("RequestId: {RequestId}", 100))
            {
                using (logger.BeginScope("Request matched action: {ActionName}", "Index"))
                {
                    logger.Log(LogLevel.Information, 0, _state, null, _defaultFormatter);
                }

                using (logger.BeginScope("Created product: {ProductName}", "Car"))
                {
                    logger.Log(LogLevel.Information, 0, _state, null, _defaultFormatter);
                }
            }

            // Assert
            Assert.Equal(4, sink.Writes.Count);
            // scope
            var write = sink.Writes[1];
            Assert.Equal(expectedHeader + expectedScope1 + expectedMessage, write.Message);
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
            write = sink.Writes[3];
            Assert.Equal(expectedHeader + expectedScope2 + expectedMessage, write.Message);
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
        }

        [Fact]
        public void CallingBeginScopeOnLogger_AlwaysReturnsNewDisposableInstance()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            var disposable1 = logger.BeginScope("Scope1");
            var disposable2 = logger.BeginScope("Scope2");

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
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            var disposable = logger.BeginScope("Scope1");

            // Assert
            Assert.NotNull(disposable);
        }

#if NET452
        private static void DomainFunc()
        {
            var t = SetUp(filter: null, includeScopes: true);
            var logger = t.Item1;
            using (logger.BeginScope("newDomain scope"))
            {
                logger.LogInformation("Test");
            }
        }

        [Fact]
        public void ScopeWithChangingAppDomains_DoesNotAccessUnloadedAppDomain()
        {
            // Arrange
            var t = SetUp(filter: null, includeScopes: true);
            var logger = t.Item1;
            var sink = t.Item2;
            var setupInfo = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
            };
            var domain = AppDomain.CreateDomain("newDomain", null, setupInfo);

            // Act
            domain.DoCallBack(DomainFunc);
            AppDomain.Unload(domain);
            var disposable = logger.BeginScope("Scope1");
            logger.LogInformation("Test");

            // Assert
            Assert.NotNull(disposable);
            Assert.Equal(2, sink.Writes.Count);
        }
#endif

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

        [Fact]
        public void ConsoleLogger_Settings_LogLevelIgnoreCase()
        {
            var section = new Mock<IConfigurationSection>();
            section.SetupGet(x => x["MyTest"])
                .Returns("INFOrmAtiOn");

            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x.GetSection("LogLevel"))
                .Returns(section.Object);

            var settings = new ConfigurationConsoleLoggerSettings(configuration.Object);

            LogLevel logLevel = LogLevel.None;
            settings.TryGetSwitch("MyTest", out logLevel);

            Assert.Equal(LogLevel.Information, logLevel);
        }

        [Fact]
        public void WriteCore_NullMessageWithException()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;
            var ex = new Exception("Exception message" + Environment.NewLine + "with a second line");
            string message = null;
            var expected = ex.ToString() + Environment.NewLine;

            // Act
            logger.Log(LogLevel.Critical, 0, message, ex, (s,e) => s);
            logger.Log(LogLevel.Error, 0, message, ex, (s,e) => s);
            logger.Log(LogLevel.Warning, 0, message, ex, (s,e) => s);
            logger.Log(LogLevel.Information, 0, message, ex, (s,e) => s);
            logger.Log(LogLevel.Debug, 0, message, ex, (s,e) => s);
            logger.Log(LogLevel.Trace, 0, message, ex, (s,e) => s);

            // Assert
            Assert.Equal(6, sink.Writes.Count);
            Assert.Equal(expected, sink.Writes[0].Message);
            Assert.Equal(expected, sink.Writes[1].Message);
            Assert.Equal(expected, sink.Writes[2].Message);
            Assert.Equal(expected, sink.Writes[3].Message);
            Assert.Equal(expected, sink.Writes[4].Message);
            Assert.Equal(expected, sink.Writes[5].Message);
        }

        [Fact]
        public void WriteCore_MessageWithNullException()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;
            Exception ex = null;

            // Act
            logger.Log(LogLevel.Critical, 0, _state, ex, (s,e) => s);
            logger.Log(LogLevel.Error, 0, _state, ex, (s,e) => s);
            logger.Log(LogLevel.Warning, 0, _state, ex, (s,e) => s);
            logger.Log(LogLevel.Information, 0, _state, ex, (s,e) => s);
            logger.Log(LogLevel.Debug, 0, _state, ex, (s,e) => s);
            logger.Log(LogLevel.Trace, 0, _state, ex, (s,e) => s);

            // Assert
            Assert.Equal(12, sink.Writes.Count);
            Assert.Equal(GetMessage("crit", 0, ex), GetMessage(sink.Writes.GetRange(0 * WritesPerMsg, WritesPerMsg)));
            Assert.Equal(GetMessage("fail", 0, ex), GetMessage(sink.Writes.GetRange(1 * WritesPerMsg, WritesPerMsg)));
            Assert.Equal(GetMessage("warn", 0, ex), GetMessage(sink.Writes.GetRange(2 * WritesPerMsg, WritesPerMsg)));
            Assert.Equal(GetMessage("info", 0, ex), GetMessage(sink.Writes.GetRange(3 * WritesPerMsg, WritesPerMsg)));
            Assert.Equal(GetMessage("dbug", 0, ex), GetMessage(sink.Writes.GetRange(4 * WritesPerMsg, WritesPerMsg)));
            Assert.Equal(GetMessage("trce", 0, ex), GetMessage(sink.Writes.GetRange(5 * WritesPerMsg, WritesPerMsg)));
        }

        [Fact]
        public void WriteCore_NullMessageWithNullException()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;
            Exception ex = null;
            string message = null;

            // Act
            logger.Log(LogLevel.Critical, 0, message, ex, (s,e) => s);
            logger.Log(LogLevel.Error, 0, message, ex, (s,e) => s);
            logger.Log(LogLevel.Warning, 0, message, ex, (s,e) => s);
            logger.Log(LogLevel.Information, 0, message, ex, (s,e) => s);
            logger.Log(LogLevel.Debug, 0, message, ex, (s,e) => s);
            logger.Log(LogLevel.Trace, 0, message, ex, (s,e) => s);

            // Assert
            Assert.Equal(0, sink.Writes.Count);
        }

        private string GetMessage(string logLevelString, int eventId, Exception exception)
            => GetMessage(logLevelString, eventId, _state, exception);

        private string GetMessage<TState>(string logLevelString, int eventId, TState state, Exception exception)
        {
            var loglevelStringWithPadding = $"{logLevelString}: ";

            return
                loglevelStringWithPadding
                + $"{_loggerName}[{eventId}]"
                + Environment.NewLine
                + _paddingString
                + ReplaceMessageNewLinesWithPadding(state?.ToString())
                + Environment.NewLine
                + ( exception != null
                    ? exception.ToString() + Environment.NewLine
                    : string.Empty );
        }

        private string ReplaceMessageNewLinesWithPadding(string message)
        {
            return message.Replace(Environment.NewLine, Environment.NewLine + _paddingString);
        }

        private string GetMessage(List<ConsoleContext> contexts)
        {
            return string.Join("", contexts.Select(c => c.Message));
        }

        private string CreateHeader(int eventId = 0)
        {
            return $": {_loggerName}[{eventId}]{Environment.NewLine}";
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
