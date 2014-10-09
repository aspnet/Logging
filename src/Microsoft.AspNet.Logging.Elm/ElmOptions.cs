using System;

namespace Microsoft.AspNet.Logging.Elm
{
    /// <summary>
    /// Options for ElmMiddleware
    /// </summary>
    public class ElmOptions
    {
        /// <summary>
        /// Specifies the path to view the logs
        /// </summary>
        public string Path { get; set; }
    }
}