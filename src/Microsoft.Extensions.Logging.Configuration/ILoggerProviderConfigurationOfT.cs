using System;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    ///
    /// </summary>
    public interface ILoggerProviderConfiguration
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        IConfiguration GetConfiguration(Type providerType);
    }
}