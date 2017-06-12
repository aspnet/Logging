using Microsoft.Extensions.Logging.AzureAppServices.Internal;
using Moq;
using Serilog.Events;
using Xunit;

namespace Microsoft.Extensions.Logging.AzureAppServices.Test
{
    public class WebConfigurationLevelSwitchTests
    {
        [Theory]
        public void InitializesWithCurrentLevelWhenCreated(string levelValue, string enabledValue)
        {
            var s = new ConfigurationBasedLevelSwitcher();
        }
    }
}
