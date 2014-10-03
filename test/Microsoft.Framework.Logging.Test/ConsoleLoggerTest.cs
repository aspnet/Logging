using System;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Console;
using Microsoft.Framework.Logging.Test.Console;
using Xunit;

namespace Microsoft.Framework.Logging.Test
{
    public class ConsoleLoggerTest
    {
        private const string name = "test";
        private const string state = "This is a test";

        private Tuple<ConsoleLogger, ConsoleSink> SetUp(Func<string, TraceType, bool> filter)
        {
            // Arrange
            var sink = new ConsoleSink();
            var console = new TestConsole(sink);
            var logger = new ConsoleLogger(name, filter);
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
            logger.WriteCore(TraceType.Information, 0, state, null, null);

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
            logger.WriteCore(TraceType.Warning, 0, state, null, null);

            // Assert
            Assert.Equal(0, sink.Writes.Count);

            // Act
            logger.WriteCore(TraceType.Critical, 0, state, null, null);

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
            logger.WriteCore(TraceType.Warning, 0, state, null, null);

            // Assert
            Assert.Equal(0, sink.Writes.Count);

            // Act
            logger.WriteCore(TraceType.Error, 0, state, null, null);

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
            logger.WriteCore(TraceType.Information, 0, state, null, null);

            // Assert
            Assert.Equal(0, sink.Writes.Count);

            // Act
            logger.WriteCore(TraceType.Warning, 0, state, null, null);

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
            logger.WriteCore(TraceType.Verbose, 0, state, null, null);

            // Assert
            Assert.Equal(0, sink.Writes.Count);

            // Act
            logger.WriteCore(TraceType.Information, 0, state, null, null);

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
            logger.WriteCore(TraceType.Critical, 0, state, null, null);
            logger.WriteCore(TraceType.Error, 0, state, null, null);
            logger.WriteCore(TraceType.Warning, 0, state, null, null);
            logger.WriteCore(TraceType.Information, 0, state, null, null);
            logger.WriteCore(TraceType.Verbose, 0, state, null, null);

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
            logger.WriteCore(TraceType.Critical, 0, name, null, null);

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
            logger.WriteCore(TraceType.Error, 0, name, null, null);

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
            logger.WriteCore(TraceType.Warning, 0, name, null, null);

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
            logger.WriteCore(TraceType.Information, 0, name, null, null);

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
            logger.WriteCore(TraceType.Verbose, 0, name, null, null);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
            var write = sink.Writes[0];
            Assert.Equal(System.Console.BackgroundColor, write.BackgroundColor);
            Assert.Equal(ConsoleColor.Gray, write.ForegroundColor);
        }
    }
}