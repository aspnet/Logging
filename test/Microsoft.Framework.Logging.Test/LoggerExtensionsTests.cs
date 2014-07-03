// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
#if NET45
using Moq;
#endif
using Xunit;
using Microsoft.Framework.Logging.Infrastructure;

namespace Microsoft.Framework.Logging.Test
{
    public class LoggerExtensionsTests
    {
#if NET45
        [Fact]
        public void WriteStartDisposable_Calls_WriteStop_WhenEnabled()
        {
            // Arrange
            var message = "1337m4sterchief117n0sc0p3";

            var mockLogger = new Mock<ILogger>();

            mockLogger
                .Setup(m => m.WriteCore(
                    TraceType.Start,
                    0,
                    message,
                    null,
                    It.IsAny<Func<object, Exception, string>>()))
                .Returns(true)
                .Verifiable();

            mockLogger
                .Setup(m => m.WriteCore(
                    TraceType.Stop,
                    0,
                    message,
                    null,
                    It.IsAny<Func<object, Exception, string>>()))
                .Returns(true)
                .Verifiable();

            var logger = mockLogger.Object;

            // Act
            var disposable = Assert.IsType<LogicalOperation>(logger.WriteStart(message));
            disposable.Dispose();

            // Assert
            mockLogger.VerifyAll();
        }

        [Fact]
        public void WriteStart_Returns_NullLogicalOperation_WhenDisabled()
        {
            // Arrange
            var message = "1337m4sterchief117n0sc0p3";

            var mockLogger = new Mock<ILogger>();

            mockLogger
                .Setup(m => m.WriteCore(
                    TraceType.Start,
                    0,
                    message,
                    null,
                    It.IsAny<Func<object, Exception, string>>()))
                .Returns(false)
                .Verifiable();

            var logger = mockLogger.Object;

            // Act
            var disposable = logger.WriteStart(message);

            // Assert
            Assert.Same(NullLogicalOperation.Instance, disposable);

            mockLogger
                .Verify(
                    m => m.WriteCore(
                        It.IsAny<TraceType>(),
                        It.IsAny<int>(),
                        It.IsAny<string>(),
                        It.IsAny<Exception>(),
                        It.IsAny<Func<object, Exception, string>>()),
                    Times.Once()
                );
        }
#endif
    }
}