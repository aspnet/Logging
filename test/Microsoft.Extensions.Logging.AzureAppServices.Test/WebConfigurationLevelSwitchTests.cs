//using System.Collections.Generic;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging.AzureAppServices;
//using Moq;
//using Xunit;

//namespace Microsoft.Extensions.Logging.AzureAppServices.Test
//{
//    public class WebConfigurationLevelSwitchTests
//    {
//        [Theory]
//        public void InitializesWithCurrentLevelWhenCreated(string levelValue, string enableValue, LogLevel expectedLevel)
//        {
//            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new []
//            {
//                new KeyValuePair<string, string>("levelKey", levelValue),
//                new KeyValuePair<string, string>("enableKey", enableValue),
//            });
//            var s = new ConfigurationBasedLevelSwitcher(configuration, typeof(TestFileLoggerProvider), "levelKey", "levelValue");

//            var filterConfiguration = new LogerFilterConfiguration();
//            s.Configure(filterConfiguration);

//            Assert.Equal(1, filterConfiguration.Rules.Count);

//            var rule = filterConfiguration.Rules[0];
//            Assert.Equal(typeof(TestFileLoggerProvider).FullName, rule.Provider);
//            Assert.Equal(expectedLevel, rule.MinLevel);
//        }
//    }
//}
