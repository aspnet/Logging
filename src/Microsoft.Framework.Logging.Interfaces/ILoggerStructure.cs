using System;
using System.Collections.Generic;

namespace Microsoft.Framework.Logging
{
#if ASPNET50 || ASPNETCORE50
    [Runtime.AssemblyNeutral]
#endif
    public interface ILoggerStructure
    {
        IReadOnlyDictionary<string, object> GetValues();

        /// <summary>
        /// Returns a human-readable string of the structured data
        /// </summary>
        string Format();
    }
}
