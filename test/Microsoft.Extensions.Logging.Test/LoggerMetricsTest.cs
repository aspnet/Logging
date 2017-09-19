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

        [Fact]
        public void RecordValue_DoesntWriteMetricToProviderWithoutMetricsSupport()
        {
            var testSink1 = new TestSink();
            var testSink2 = new TestSink();
            var loggerFactory = TestLoggerBuilder.Create(builder => builder
                .AddProvider(new TestLoggerProviderWithoutMetrics(testSink1, isEnabled: true))
                .AddProvider(new TestLoggerProvider(testSink2, isEnabled: true)));

            var logger = loggerFactory.CreateLogger("test");
            var metric = logger.DefineMetric("test");
            metric.RecordValue(42.0);

            Assert.Empty(testSink1.Metrics);

            Assert.Collection(testSink2.Metrics,
                item =>
                {
                    Assert.Equal("test", item.Name);
                    Assert.Equal(42.0, item.Value);
                });
        }

        [Fact]
        public void RecordValue_WritesValueToRegisteredMetricsProviders()
        {
            var testSink1 = new TestSink();
            var testSink2 = new TestSink();
            var loggerFactory = TestLoggerBuilder.Create(builder => builder
                .AddProvider(new TestLoggerProvider(testSink1, isEnabled: true))
                .AddProvider(new TestLoggerProvider(testSink2, isEnabled: true)));

            var logger = loggerFactory.CreateLogger("test");
            var metric = logger.DefineMetric("test");
            metric.RecordValue(42.0);

            Assert.Collection(testSink1.Metrics,
                item =>
                {
                    Assert.Equal("test", item.Name);
                    Assert.Equal(42.0, item.Value);
                });

            Assert.Collection(testSink2.Metrics,
                item =>
                {
                    Assert.Equal("test", item.Name);
                    Assert.Equal(42.0, item.Value);
                });
        }
    }
}
