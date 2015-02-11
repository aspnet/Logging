﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
        private const string _secureName = "debug";
        private const string _state = "This is a test";
        private const string _secureState = "Secure this is a test";
        private static readonly Func<object, Exception, string> TheMessageAndError = (message, error) => string.Format(CultureInfo.CurrentCulture, "{0}\r\n{1}", message, error);

        private Tuple<ConsoleLogger, ConsoleSink> SetUpNamed(string name, Func<string, LogLevel, bool> filter)
        {
            // Arrange
            var sink = new ConsoleSink();
            var console = new TestConsole(sink);
            var logger = new ConsoleLogger(name, filter);
            logger.Console = console;
            return new Tuple<ConsoleLogger, ConsoleSink>(logger, sink);
        }

        private Tuple<ConsoleLogger, ConsoleSink> SetUp(Func<string, LogLevel, bool> filter)
        {
            return SetUpNamed(_name, filter);
        }

        [Fact]
        public void LogsWhenNullFilterGiven()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Write(LogLevel.Information, 0, _state, null, null);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
        }

        [Fact]
        public void CriticalFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, logLevel) => logLevel >= LogLevel.Critical);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Write(LogLevel.Warning, 0, _state, null, null);

            // Assert
            Assert.Equal(0, sink.Writes.Count);

            // Act
            logger.Write(LogLevel.Critical, 0, _state, null, null);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
        }

        [Fact]
        public void ErrorFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, logLevel) => logLevel >= LogLevel.Error);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Write(LogLevel.Warning, 0, _state, null, null);

            // Assert
            Assert.Equal(0, sink.Writes.Count);

            // Act
            logger.Write(LogLevel.Error, 0, _state, null, null);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
        }

        [Fact]
        public void WarningFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, logLevel) => logLevel >= LogLevel.Warning);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Write(LogLevel.Information, 0, _state, null, null);

            // Assert
            Assert.Equal(0, sink.Writes.Count);

            // Act
            logger.Write(LogLevel.Warning, 0, _state, null, null);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
        }

        [Fact]
        public void InformationFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, logLevel) => logLevel >= LogLevel.Information);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Write(LogLevel.Verbose, 0, _state, null, null);

            // Assert
            Assert.Equal(0, sink.Writes.Count);

            // Act
            logger.Write(LogLevel.Information, 0, _state, null, null);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
        }

        [Fact]
        public void DebugFilter_LogsWhenAppropriate()
        {
            // Arrange
            var t = SetUp((category, logLevel) => logLevel >= LogLevel.Debug);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Write(LogLevel.Critical, 0, _state, null, null);
            logger.Write(LogLevel.Error, 0, _state, null, null);
            logger.Write(LogLevel.Warning, 0, _state, null, null);
            logger.Write(LogLevel.Information, 0, _state, null, null);
            logger.Write(LogLevel.Verbose, 0, _state, null, null);
            logger.Write(LogLevel.Debug, 0, _state, null, null);

            // Assert
            Assert.Equal(6, sink.Writes.Count);
        }

        [Fact]
        public void WriteCritical_LogsCorrectColors()
        {
            // Arrange
            var t = SetUp(null);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Write(LogLevel.Critical, 0, _state, null, null);

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
            logger.Write(LogLevel.Error, 0, _state, null, null);

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
            logger.Write(LogLevel.Warning, 0, _state, null, null);

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
            logger.Write(LogLevel.Information, 0, _state, null, null);

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
            logger.Write(LogLevel.Verbose, 0, _state, null, null);

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
            logger.Write(LogLevel.Critical, 0, _state, ex, TheMessageAndError);
            logger.Write(LogLevel.Error, 0, _state, ex, TheMessageAndError);
            logger.Write(LogLevel.Warning, 0, _state, ex, TheMessageAndError);
            logger.Write(LogLevel.Information, 0, _state, ex, TheMessageAndError);
            logger.Write(LogLevel.Verbose, 0, _state, ex, TheMessageAndError);

            // Assert
            Assert.Equal(5, sink.Writes.Count);
            Assert.Equal(getMessage(LogLevel.Critical, ex), sink.Writes[0].Message);
            Assert.Equal(getMessage(LogLevel.Error, ex), sink.Writes[1].Message);
            Assert.Equal(getMessage(LogLevel.Warning, ex), sink.Writes[2].Message);
            Assert.Equal(getMessage(LogLevel.Information, ex), sink.Writes[3].Message);
            Assert.Equal(getMessage(LogLevel.Verbose, ex), sink.Writes[4].Message);
        }

        [Fact]
        public void WriteDebug_ProperlyFiltersSensitiveMessages()
        {
            // Arrange
            Func<string, LogLevel, bool> filter = (name, logLevel) => logLevel != LogLevel.Debug || name == "debug";
            var T = SetUp(filter);
            var secureT = SetUpNamed("debug", filter);
            var logger = T.Item1;
            var sink = T.Item2;
            var secureLogger = secureT.Item1;
            var secureSink = secureT.Item2;

            // Act
            logger.WriteDebug(0, _secureState, _state);
            secureLogger.WriteDebug(0, _secureState, _state);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
            Assert.Equal(getMessage(LogLevel.Verbose, _state, _name), sink.Writes[0].Message);

            Assert.Equal(1, secureSink.Writes.Count);
            Assert.Equal(getMessage(LogLevel.Verbose, _secureState, _secureName), secureSink.Writes[0].Message);
        }

        private string getMessage(LogLevel logLevel, Exception exception)
        {
            return string.Format("[{0}:{1}] {2}", logLevel.ToString().ToUpperInvariant(), _name, TheMessageAndError(_state, exception));
        }

        private string getMessage(LogLevel logLevel, string state, string name)
        {
            return string.Format("[{0}:{1}] {2}", logLevel.ToString().ToUpperInvariant(), name, state);
        }
    }
}