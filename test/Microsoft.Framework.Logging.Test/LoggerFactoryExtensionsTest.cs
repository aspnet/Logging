using System;
#if NET45
using Moq;
#endif
using Xunit;


namespace Microsoft.Framework.Logging.Test
{
    public class LoggerFactoryExtensionsTest
    {
#if NET45
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
#endif

        private class TestType
        {
            // intentionally holds nothing
        }
    }
}