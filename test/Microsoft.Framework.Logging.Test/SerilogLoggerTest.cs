// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using Microsoft.Framework.Logging.Serilog;
using Microsoft.Framework.Logging.Test.Serilog;
using Serilog;
using Serilog.Events;
using Xunit;

namespace Microsoft.Framework.Logging.Test
{
    public class SerilogLoggerTest
    {
        private const string _name = "test";
        private const string _state = "This is a test";
        private static readonly Func<object, Exception, string> TheMessageAndError = (message, error) => string.Format(CultureInfo.CurrentCulture, "{0}:{1}", message, error);

        private Tuple<SerilogLogger, SerilogSink> SetUp(LogLevel logLevel)
        {
            // Arrange
            var serilog = new LoggerConfiguration()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId();
            serilog = SetMinLevel(serilog, logLevel);
            var sink = new SerilogSink();
            serilog.WriteTo.Sink(sink);
            var provider = new SerilogLoggerProvider(serilog);
            var logger = (SerilogLogger)provider.CreateLogger(_name);

            return new Tuple<SerilogLogger, SerilogSink>(logger, sink);
        }

        private LoggerConfiguration SetMinLevel(LoggerConfiguration serilog, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    return serilog.MinimumLevel.Verbose();
                case LogLevel.Verbose:
                    return serilog.MinimumLevel.Debug();
                case LogLevel.Information:
                    return serilog.MinimumLevel.Information();
                case LogLevel.Warning:
                    return serilog.MinimumLevel.Warning();
                case LogLevel.Error:
                    return serilog.MinimumLevel.Error();
                case LogLevel.Critical:
                    return serilog.MinimumLevel.Fatal();
                default:
                    return serilog.MinimumLevel.Verbose();
            }
        }

        [Fact]
        public void LogsWhenNullFilterGiven()
        {
            // Arrange
            var t = SetUp(LogLevel.Verbose);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Log(LogLevel.Information, 0, _state, null, null);

            // Assert
            Assert.Single(sink.Writes);
        }

        [Fact]
        public void LogsCorrectLevel()
        {
            // Arrange
            var t = SetUp(LogLevel.Debug);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Log(LogLevel.Debug, 0, _state, null, null);
            logger.Log(LogLevel.Verbose, 0, _state, null, null);
            logger.Log(LogLevel.Information, 0, _state, null, null);
            logger.Log(LogLevel.Warning, 0, _state, null, null);
            logger.Log(LogLevel.Error, 0, _state, null, null);
            logger.Log(LogLevel.Critical, 0, _state, null, null);

            // Assert
            Assert.Equal(6, sink.Writes.Count);
            Assert.Equal(LogEventLevel.Verbose, sink.Writes[0].Level);
            Assert.Equal(LogEventLevel.Debug, sink.Writes[1].Level);
            Assert.Equal(LogEventLevel.Information, sink.Writes[2].Level);
            Assert.Equal(LogEventLevel.Warning, sink.Writes[3].Level);
            Assert.Equal(LogEventLevel.Error, sink.Writes[4].Level);
            Assert.Equal(LogEventLevel.Fatal, sink.Writes[5].Level);
        }

        [Theory]
        [InlineData(LogLevel.Verbose, LogLevel.Verbose, 1)]
        [InlineData(LogLevel.Verbose, LogLevel.Information, 1)]
        [InlineData(LogLevel.Verbose, LogLevel.Warning, 1)]
        [InlineData(LogLevel.Verbose, LogLevel.Error, 1)]
        [InlineData(LogLevel.Verbose, LogLevel.Critical, 1)]
        [InlineData(LogLevel.Information, LogLevel.Verbose, 0)]
        [InlineData(LogLevel.Information, LogLevel.Information, 1)]
        [InlineData(LogLevel.Information, LogLevel.Warning, 1)]
        [InlineData(LogLevel.Information, LogLevel.Error, 1)]
        [InlineData(LogLevel.Information, LogLevel.Critical, 1)]
        [InlineData(LogLevel.Warning, LogLevel.Verbose, 0)]
        [InlineData(LogLevel.Warning, LogLevel.Information, 0)]
        [InlineData(LogLevel.Warning, LogLevel.Warning, 1)]
        [InlineData(LogLevel.Warning, LogLevel.Error, 1)]
        [InlineData(LogLevel.Warning, LogLevel.Critical, 1)]
        [InlineData(LogLevel.Error, LogLevel.Verbose, 0)]
        [InlineData(LogLevel.Error, LogLevel.Information, 0)]
        [InlineData(LogLevel.Error, LogLevel.Warning, 0)]
        [InlineData(LogLevel.Error, LogLevel.Error, 1)]
        [InlineData(LogLevel.Error, LogLevel.Critical, 1)]
        [InlineData(LogLevel.Critical, LogLevel.Verbose, 0)]
        [InlineData(LogLevel.Critical, LogLevel.Information, 0)]
        [InlineData(LogLevel.Critical, LogLevel.Warning, 0)]
        [InlineData(LogLevel.Critical, LogLevel.Error, 0)]
        [InlineData(LogLevel.Critical, LogLevel.Critical, 1)]
        public void LogsWhenEnabled(LogLevel minLevel, LogLevel logLevel, int expected)
        {
            // Arrange
            var t = SetUp(minLevel);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            logger.Log(logLevel, 0, _state, null, null);

            // Assert
            Assert.Equal(expected, sink.Writes.Count);
        }


        [Fact]
        public void LogsCorrectMessage()
        {
            // Arrange
            var t = SetUp(LogLevel.Verbose);
            var logger = t.Item1;
            var sink = t.Item2;

            var exception = new Exception();

            // Act
            logger.Log(LogLevel.Information, 0, null, null, null);
            logger.Log(LogLevel.Information, 0, _state, null, null);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
            Assert.Equal(_state, sink.Writes[0].RenderMessage());
        }

        [Fact]
        public void SingleScopeProperty()
        {
            // Arrange
            var t = SetUp(LogLevel.Verbose);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            using (logger.BeginScopeImpl(new FoodScope("pizza")))
            {
                logger.Log(LogLevel.Information, 0, _state, null, null);
            }

            // Assert
            Assert.Single(sink.Writes);
            Assert.True(sink.Writes[0].Properties.ContainsKey("Name"));
            Assert.Equal("\"pizza\"", sink.Writes[0].Properties["Name"].ToString());
        }

        [Fact]
        public void NestedScopeSameProperty()
        {
            // Arrange
            var t = SetUp(LogLevel.Verbose);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            using (logger.BeginScopeImpl(new FoodScope("avocado")))
            {
                using(logger.BeginScopeImpl(new FoodScope("bacon")))
                {
                    logger.Log(LogLevel.Information, 0, _state, null, null);
                }
            }

            // Assert
            // should retain the property of the most specific scope
            Assert.Single(sink.Writes);
            Assert.True(sink.Writes[0].Properties.ContainsKey("Name"));
            Assert.Equal("\"bacon\"", sink.Writes[0].Properties["Name"].ToString());
        }

        [Fact]
        public void NestedScopesDifferentProperties()
        {
            // Arrange
            var t = SetUp(LogLevel.Verbose);
            var logger = t.Item1;
            var sink = t.Item2;

            // Act
            using (logger.BeginScopeImpl(new FoodScope("spaghetti")))
            {
                using (logger.BeginScopeImpl(new LuckyScope(7)))
                {
                    logger.Log(LogLevel.Information, 0, _state, null, null);
                }
            }

            // Assert
            Assert.Single(sink.Writes);
            Assert.True(sink.Writes[0].Properties.ContainsKey("Name"));
            Assert.Equal("\"spaghetti\"", sink.Writes[0].Properties["Name"].ToString());
            Assert.True(sink.Writes[0].Properties.ContainsKey("LuckyNumber"));
            Assert.Equal("7", sink.Writes[0].Properties["LuckyNumber"].ToString());
        }

        private class FoodScope : ReflectionBasedLogValues
        {
            private string _name;

            public FoodScope(string name)
            {
                _name = name;
            }

            public string Name { get { return _name; } }

            public override string ToString()
            {
                return string.Format("Scope {0}", Name);
            }
        }

        private class LuckyScope : ReflectionBasedLogValues
        {
            private int _luckyNumber;

            public LuckyScope(int luckyNumber)
            {
                _luckyNumber = luckyNumber;
            }

            public int LuckyNumber { get { return _luckyNumber; } }

            public override string ToString()
            {
                return string.Format("Scope {0}", LuckyNumber);
            }
        }
    }
}