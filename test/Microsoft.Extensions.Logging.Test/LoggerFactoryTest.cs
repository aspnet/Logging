// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace Microsoft.Extensions.Logging.Test
{
    public class LoggerFactoryTest
    {
        [Fact]
        public void AddProvider_ThrowsAfterDisposed()
        {
            var factory = new LoggerFactory();
            factory.Dispose();
            Assert.Throws<ObjectDisposedException>(() => factory.AddProvider(CreateProvider()));
        }

        [Fact]
        public void CreateLogger_ThrowsAfterDisposed()
        {
            var factory = new LoggerFactory();
            factory.Dispose();
            Assert.Throws<ObjectDisposedException>(() => factory.CreateLogger("d"));
        }

        private class TestLoggerFactory : LoggerFactory
        {
            public bool Disposed => CheckDisposed();
        }

        [Fact]
        public void Dispose_MultipleCallsNoop()
        {
            var factory = new TestLoggerFactory();
            factory.Dispose();
            Assert.True(factory.Disposed);
            factory.Dispose();
        }

        [Fact]
        public void Dispose_ProvidersAreDisposed()
        {
            // Arrange
            var factory = new LoggerFactory();
            var disposableProvider1 = CreateProvider();
            var disposableProvider2 = CreateProvider();
            factory.AddProvider(disposableProvider1);
            factory.AddProvider(disposableProvider2);

            // Act
            factory.Dispose();

            // Assert
            Mock.Get<IDisposable>(disposableProvider1)
                    .Verify(p => p.Dispose(), Times.Once());
            Mock.Get<IDisposable>(disposableProvider2)
                     .Verify(p => p.Dispose(), Times.Once());
        }

        private static ILoggerProvider CreateProvider()
        {
            var disposableProvider = new Mock<ILoggerProvider>();
            disposableProvider.As<IDisposable>()
                  .Setup(p => p.Dispose());
            return disposableProvider.Object;
        }

        [Fact]
        public void Dispose_ThrowException_SwallowsException()
        {
            // Arrange
            var factory = new LoggerFactory();
            var throwingProvider = new Mock<ILoggerProvider>();
            throwingProvider.As<IDisposable>()
                .Setup(p => p.Dispose())
                .Throws<Exception>();
            factory.AddProvider(throwingProvider.Object);

            // Act
            factory.Dispose();

            // Assert
            throwingProvider.As<IDisposable>()
                .Verify(p => p.Dispose(), Times.Once());
        }

        [Fact]
        public void UseConfiguration_RegistersChangeCallback()
        {
            // Arrange
            var factory = new LoggerFactory();
            var changeToken = new Mock<IChangeToken>();
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c.GetReloadToken()).Returns(changeToken.Object);

            // Act
            factory.UseConfiguration(configuration.Object);

            // Assert
            changeToken.Verify(c => c.RegisterChangeCallback(It.IsAny<Action<object>>(), It.IsAny<Object>()), Times.Once);
        }
    }
}
