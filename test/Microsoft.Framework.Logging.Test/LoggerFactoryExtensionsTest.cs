// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if ASPNET50
using Moq;
#endif
using Xunit;

namespace Microsoft.Framework.Logging.Test
{
    public class LoggerFactoryExtensionsTest
    {
#if ASPNET50
        [Fact]
        public void LoggerFactoryCreateOfT_CallsCreateWithCorrectName()
        {
            // Arrange
            var expected = typeof(TestType).FullName;

            var factory = new Mock<ILoggerFactory>();
            factory.Setup(f => f.Create(
                It.IsAny<string>()))
            .Returns(new Mock<ILogger>().Object);

            // Act
            factory.Object.Create<TestType>();

            // Assert
            factory.Verify(f => f.Create(expected));
        }

        [Fact]
        public void LoggerFactoryCreateOfT_SingleGeneric_CallsCreateWithCorrectName()
        {
            // Arrange
            var factory = new Mock<ILoggerFactory>();
            factory.Setup(f => f.Create(It.Is<string>(
                x => x.Equals("Microsoft.Framework.Logging.Test.GenericClass<Microsoft.Framework.Logging.Test.TestType>"))))
            .Returns(new Mock<ILogger>().Object);

            var logger = factory.Object.Create<GenericClass<TestType>>();

            // Assert
            Assert.NotNull(logger);
        }

        [Fact]
        public void LoggerFactoryCreateOfT_TwoGenerics_CallsCreateWithCorrectName()
        {
            // Arrange
            var factory = new Mock<ILoggerFactory>();
            factory.Setup(f => f.Create(It.Is<string>(
                x => x.Equals("Microsoft.Framework.Logging.Test.GenericClass<Microsoft.Framework.Logging.Test.TestType, Microsoft.Framework.Logging.Test.SecondTestType>"))))
            .Returns(new Mock<ILogger>().Object);

            var logger = factory.Object.Create<GenericClass<TestType,SecondTestType>>();

            // Assert
            Assert.NotNull(logger);
        }
#endif
    }

    internal class TestType
    {
        // intentionally holds nothing
    }

    internal class SecondTestType
    {
        // intentionally holds nothing
    }

    internal class GenericClass<X, Y> where X : class where Y : class
    {
        // intentionally holds nothing
    }

    internal class GenericClass<X> where X : class
    {
        // intentionally holds nothing
    }
}