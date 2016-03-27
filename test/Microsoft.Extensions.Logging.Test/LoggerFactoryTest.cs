// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Moq;
using Xunit;

namespace Microsoft.Extensions.Logging.Test
{
    public class LoggerFactoryTest
    {
        [Fact]
        public void Dispose_ProvidersAreDisposed()
        {
            // Arrange
            var factory = new LoggerFactory();
            var disposableSink1 = CreateSink();
            var disposableSink2 = CreateSink();
            factory.AddSink(disposableSink1);
            factory.AddSink(disposableSink2);

            // Act
            factory.Dispose();

            // Assert
            Mock.Get<IDisposable>(disposableSink1)
                    .Verify(p => p.Dispose(), Times.Once());
            Mock.Get<IDisposable>(disposableSink2)
                     .Verify(p => p.Dispose(), Times.Once());
        }

        private static ILogSink CreateSink()
        {
            var disposableSink = new Mock<ILogSink>();
            disposableSink.As<IDisposable>()
                  .Setup(p => p.Dispose());
            return disposableSink.Object;
        }

        [Fact]
        public void Dispose_ThrowException_SwallowsException()
        {
            // Arrange
            var factory = new LoggerFactory();
            var throwingSink = new Mock<ILogSink>();
            throwingSink.As<IDisposable>()
                .Setup(p => p.Dispose())
                .Throws<Exception>();
            factory.AddSink(throwingSink.Object);

            // Act
            factory.Dispose();

            // Assert
            throwingSink.As<IDisposable>()
                .Verify(p => p.Dispose(), Times.Once());
        }
    }
}
