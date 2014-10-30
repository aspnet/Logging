using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Framework.Logging.Serilog
{
    public abstract class LogData : ILoggerStructure
    {
        public virtual IEnumerable<KeyValuePair<string, object>> GetValues()
        {
            foreach (var propertyInfo in GetType().GetTypeInfo().DeclaredProperties)
            {
                yield return new KeyValuePair<string, object>(
                    propertyInfo.Name,
                    propertyInfo.GetValue(this));
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}