﻿using System;
using System.Globalization;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Console;
using Microsoft.Framework.Logging.Test.Console;
using Xunit;

namespace Microsoft.Framework.Logging.Test
{
    public class ConsoleLoggerTest
    {
        private const string _name = "test";
        private const string _state = "This is a test";
        private static readonly Func<object, Exception, string> TheMessageAndError = (message, error) => string.Format(CultureInfo.CurrentCulture, "{0}\r\n{1}", message, error);

        private Tuple<ConsoleLogger, ConsoleSink> SetUp(Func<string, TraceType, bool> filter)
        {
            // Arrange
            var sink = new ConsoleSink();
            var console = new TestConsole(sink);
            var logger = new ConsoleLogger(_name, filter);
            logger.Console = console;
            return new Tuple<ConsoleLogger, ConsoleSink>(logger, sink);
        }

        [Fact]
        public void LogsWhenNullFilterGiven()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.WriteCore(TraceType.Information, 0, _state, null, null);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
        }

        [Fact]
        public void CriticalFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, traceType) => traceType <= TraceType.Critical);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.WriteCore(TraceType.Warning, 0, _state, null, null);

            // Assert
            Assert.Equal(0, sink.Writes.Count);

            // Act
            logger.WriteCore(TraceType.Critical, 0, _state, null, null);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
        }

        [Fact]
        public void ErrorFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, traceType) => traceType <= TraceType.Error);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.WriteCore(TraceType.Warning, 0, _state, null, null);

            // Assert
            Assert.Equal(0, sink.Writes.Count);

            // Act
            logger.WriteCore(TraceType.Error, 0, _state, null, null);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
        }

        [Fact]
        public void WarningFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, traceType) => traceType <= TraceType.Warning);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.WriteCore(TraceType.Information, 0, _state, null, null);

            // Assert
            Assert.Equal(0, sink.Writes.Count);

            // Act
            logger.WriteCore(TraceType.Warning, 0, _state, null, null);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
        }

        [Fact]
        public void InformationFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, traceType) => traceType <= TraceType.Information);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.WriteCore(TraceType.Verbose, 0, _state, null, null);

            // Assert
            Assert.Equal(0, sink.Writes.Count);

            // Act
            logger.WriteCore(TraceType.Information, 0, _state, null, null);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
        }

        [Fact]
        public void VerboseFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, traceType) => traceType <= TraceType.Verbose);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.WriteCore(TraceType.Critical, 0, _state, null, null);
            logger.WriteCore(TraceType.Error, 0, _state, null, null);
            logger.WriteCore(TraceType.Warning, 0, _state, null, null);
            logger.WriteCore(TraceType.Information, 0, _state, null, null);
            logger.WriteCore(TraceType.Verbose, 0, _state, null, null);

            // Assert
            Assert.Equal(5, sink.Writes.Count);
        }

        [Fact]
        public void WriteCritical_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.WriteCore(TraceType.Critical, 0, _state, null, null);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
            var write = sink.Writes[0];
            Assert.Equal(ConsoleColor.Red, write.BackgroundColor);
            Assert.Equal(ConsoleColor.White, write.ForegroundColor);
        }

        [Fact]
        public void WriteError_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.WriteCore(TraceType.Error, 0, _state, null, null);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
            var write = sink.Writes[0];
            Assert.Equal(System.Console.BackgroundColor, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Red, write.ForegroundColor);
        }

        [Fact]
        public void WriteWarning_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.WriteCore(TraceType.Warning, 0, _state, null, null);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
            var write = sink.Writes[0];
            Assert.Equal(System.Console.BackgroundColor, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Yellow, write.ForegroundColor);
        }

        [Fact]
        public void WriteInformation_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.WriteCore(TraceType.Information, 0, _state, null, null);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
            var write = sink.Writes[0];
            Assert.Equal(System.Console.BackgroundColor, write.BackgroundColor);
            Assert.Equal(ConsoleColor.White, write.ForegroundColor);
        }

        [Fact]
        public void WriteVerbose_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.WriteCore(TraceType.Verbose, 0, _state, null, null);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
            var write = sink.Writes[0];
            Assert.Equal(System.Console.BackgroundColor, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Gray, write.ForegroundColor);
        }

        [Fact]
        public void WriteCore_LogsCorrectMessages()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;
            var ex = new Exception();

            // Act
            logger.WriteCore(TraceType.Critical, 0, _state, ex, TheMessageAndError);
            logger.WriteCore(TraceType.Error, 0, _state, ex, TheMessageAndError);
            logger.WriteCore(TraceType.Warning, 0, _state, ex, TheMessageAndError);
            logger.WriteCore(TraceType.Information, 0, _state, ex, TheMessageAndError);
            logger.WriteCore(TraceType.Verbose, 0, _state, ex, TheMessageAndError);

            // Assert
            Assert.Equal(5, sink.Writes.Count);
            Assert.Equal(getMessage(TraceType.Critical, ex), sink.Writes[0].Message);
            Assert.Equal(getMessage(TraceType.Error, ex), sink.Writes[1].Message);
            Assert.Equal(getMessage(TraceType.Warning, ex), sink.Writes[2].Message);
            Assert.Equal(getMessage(TraceType.Information, ex), sink.Writes[3].Message);
            Assert.Equal(getMessage(TraceType.Verbose, ex), sink.Writes[4].Message);
        }

        private string getMessage(TraceType traceType, Exception exception)
        {
            return string.Format("[{0}:{1}] {2}", traceType.ToString().ToUpperInvariant(), _name, TheMessageAndError(_state, exception));
        }

        [Fact]
        public void LogsToCorrectStream()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.WriteCore(TraceType.Critical, 0, _state, null, null);
            logger.WriteCore(TraceType.Error, 0, _state, null, null);
            logger.WriteCore(TraceType.Warning, 0, _state, null, null);
            logger.WriteCore(TraceType.Information, 0, _state, null, null);
            logger.WriteCore(TraceType.Verbose, 0, _state, null, null);

            // Assert
            Assert.Equal(5, sink.Writes.Count);
            Assert.True(sink.Writes[0].Error);
            Assert.True(sink.Writes[1].Error);
            Assert.False(sink.Writes[2].Error);
            Assert.False(sink.Writes[3].Error);
            Assert.False(sink.Writes[4].Error);
        }
    }
}