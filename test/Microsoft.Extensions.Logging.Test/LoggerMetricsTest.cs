using System.Collections.Generic;
using Microsoft.Extensions.Logging.Testing;
using Xunit;

namespace Microsoft.Extensions.Logging.Test
{
    public class LoggerMetricsTest
    {
        [Fact]
        public void DefineMetric_ReturnsSameInstanceForSameName()
        {
            var testSink = new TestSink();
            var loggerFactory = TestLoggerBuilder.Create(builder => builder
                .AddProvider(new TestLoggerProvider(testSink, isEnabled: true)));

            var logger = loggerFactory.CreateLogger("test");

            Assert.Same(logger.DefineMetric("foo"), logger.DefineMetric("foo"));
        }
    }
}
