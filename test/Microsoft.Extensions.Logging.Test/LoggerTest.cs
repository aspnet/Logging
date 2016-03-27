// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.Extensions.Logging
{
    public class LoggerTest
    {
        [Fact]
        public void Log_IgnoresExceptionInIntermediateLoggersAndThrowsAggregateException()
        {
            // Arrange
            var store = new List<string>();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSink(new CustomSink(ThrowExceptionAt.None, store));
            loggerFactory.AddSink(new CustomSink(ThrowExceptionAt.Log, store));
            loggerFactory.AddSink(new CustomSink(ThrowExceptionAt.None, store));
            var logger = loggerFactory.CreateLogger("Test");

            // Act
            var aggregateException = Assert.Throws<AggregateException>(() => logger.LogInformation("Hello!"));

            // Assert
            Assert.Equal(new[] { "provider1.Test-Hello!", "provider3.Test-Hello!" }, store);
            Assert.NotNull(aggregateException);
            Assert.StartsWith("An error occurred while writing to logger(s).", aggregateException.Message);
            Assert.Equal(1, aggregateException.InnerExceptions.Count);
            var exception = aggregateException.InnerExceptions[0];
            Assert.Equal("provider2.Test-Error occurred while logging data.", exception.Message);
        }

        [Fact]
        public void BeginScope_IgnoresExceptionInIntermediateLoggersAndThrowsAggregateException()
        {
            // Arrange
            var store = new List<string>();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSink(new CustomSink(ThrowExceptionAt.None, store));
            loggerFactory.AddSink(new CustomSink(ThrowExceptionAt.BeginScope, store));
            loggerFactory.AddSink(new CustomSink(ThrowExceptionAt.None, store));
            var logger = loggerFactory.CreateLogger("Test");

            // Act
            var aggregateException = Assert.Throws<AggregateException>(() => logger.BeginScope("Scope1"));

            // Assert
            Assert.Equal(new[] { "provider1.Test-Scope1", "provider3.Test-Scope1" }, store);
            Assert.NotNull(aggregateException);
            Assert.StartsWith("An error occurred while writing to logger(s).", aggregateException.Message);
            Assert.Equal(1, aggregateException.InnerExceptions.Count);
            var exception = aggregateException.InnerExceptions[0];
            Assert.Equal("provider2.Test-Error occurred while creating scope.", exception.Message);
        }

        [Fact]
        public void IsEnabled_IgnoresExceptionInIntermediateLoggers()
        {
            // Arrange
            var store = new List<string>();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSink(new CustomSink(ThrowExceptionAt.None, store));
            loggerFactory.AddSink(new CustomSink(ThrowExceptionAt.IsEnabled, store));
            loggerFactory.AddSink(new CustomSink(ThrowExceptionAt.None, store));
            var logger = loggerFactory.CreateLogger("Test");

            // Act
            var aggregateException = Assert.Throws<AggregateException>(() => logger.LogInformation("Hello!"));

            // Assert
            Assert.Equal(new[] { "provider1.Test-Hello!", "provider3.Test-Hello!" }, store);
            Assert.NotNull(aggregateException);
            Assert.StartsWith("An error occurred while writing to logger(s).", aggregateException.Message);
            Assert.Equal(1, aggregateException.InnerExceptions.Count);
            var exception = aggregateException.InnerExceptions[0];
            Assert.Equal("provider2.Test-Error occurred while checking if logger is enabled.", exception.Message);
        }

        [Fact]
        public void Log_AggregatesExceptionsFromMultipleLoggers()
        {
            // Arrange
            var store = new List<string>();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSink(new CustomSink(ThrowExceptionAt.Log, store));
            loggerFactory.AddSink(new CustomSink(ThrowExceptionAt.Log, store));
            var logger = loggerFactory.CreateLogger("Test");

            // Act
            var aggregateException = Assert.Throws<AggregateException>(() => logger.LogInformation("Hello!"));

            // Assert
            Assert.Empty(store);
            Assert.NotNull(aggregateException);
            Assert.StartsWith("An error occurred while writing to logger(s).", aggregateException.Message);
            var exceptions = aggregateException.InnerExceptions;
            Assert.Equal(2, exceptions.Count);
            Assert.Equal("provider1.Test-Error occurred while logging data.", exceptions[0].Message);
            Assert.Equal("provider2.Test-Error occurred while logging data.", exceptions[1].Message);
        }

        private class CustomSink : ILogSink
        {
            private readonly ThrowExceptionAt _throwExceptionAt;
            private readonly List<string> _store;

            public CustomSink(ThrowExceptionAt throwExceptionAt, List<string> store)
            {
                _throwExceptionAt = throwExceptionAt;
                _store = store;
            }

            public IDisposable BeginScope(string categoryName, object state)
            {
                if (_throwExceptionAt == ThrowExceptionAt.BeginScope)
                {
                    throw new InvalidOperationException($"{categoryName}-Error occurred while creating scope.");
                }
                _store.Add($"{categoryName}-{state}");

                return null;
            }

            public bool IsEnabled(string categoryName, LogLevel logLevel)
            {
                if (_throwExceptionAt == ThrowExceptionAt.IsEnabled)
                {
                    throw new InvalidOperationException($"{categoryName}-Error occurred while checking if logger is enabled.");
                }

                return true;
            }

            public void Log<TState>(
                string categoryName,
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception exception,
                Func<TState, Exception, string> formatter)
            {
                if (!IsEnabled(categoryName, logLevel))
                {
                    return;
                }

                if (_throwExceptionAt == ThrowExceptionAt.Log)
                {
                    throw new InvalidOperationException($"{categoryName}-Error occurred while logging data.");
                }
                _store.Add($"{categoryName}-{state}");
            }

            public void Dispose()
            {
            }
        }

        private enum ThrowExceptionAt
        {
            None,
            BeginScope,
            Log,
            IsEnabled
        }
    }
}
