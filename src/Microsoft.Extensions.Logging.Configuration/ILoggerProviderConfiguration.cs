using System.Text;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILoggerProviderConfiguration<T>
    {
        /// <summary>
        ///
        /// </summary>
        IConfiguration Configuration { get; }
    }
}
