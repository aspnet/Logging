// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Moq;
using Xunit;

namespace Microsoft.Framework.Logging.Test
{
    public class LoggerExtensionsTests
    {
        [Fact]
        public void WriteStartDisposable_Calls_WriteStop()
        {
            // Arrange
            var eventType = TraceType.Information;
            var message = "test message";

            var mockLogger = new Mock<ILogger>();

            mockLogger
                .Setup(m => m.WriteCore(
                    eventType | TraceType.Start,
                    0,
                    message,
                    null,
                    It.IsAny<Func<object, Exception, string>>()))
                .Returns(true)
                .Verifiable();

            mockLogger
                .Setup(m => m.WriteCore(
                    eventType | TraceType.Stop,
                    0,
                    message,
                    null,
                    It.IsAny<Func<object, Exception, string>>()))
                .Returns(true)
                .Verifiable();

            var logger = mockLogger.Object;

            // Act
            var disposable = Assert.IsType<LogicalOperation>(logger.WriteStart(eventType, message));
            disposable.Dispose();

            // Assert
            mockLogger.VerifyAll();
        }
    }
}