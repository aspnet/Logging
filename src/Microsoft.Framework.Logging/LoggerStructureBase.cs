using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Framework.Logging
{
    public abstract class LoggerStructureBase : ILoggerStructure
    {
        public virtual IEnumerable<KeyValuePair<string, object>> GetValues()
        {
            var values = new List<KeyValuePair<string, object>>();
#if ASPNET50 || ASPNETCORE50 || NET45
            foreach (var propertyInfo in GetType().GetTypeInfo().DeclaredProperties)
            {
                values.Add(new KeyValuePair<string, object>(
                    propertyInfo.Name,
                    propertyInfo.GetValue(this)));
            }
#endif
            return values;
        }

        public abstract override string ToString();
    }
}