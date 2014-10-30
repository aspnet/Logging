using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Microsoft.Framework.Logging.Serilog
{
    [DebuggerStepThrough]
    internal static class Check
    {
        public static T NotNull<T>(T value, string parameterName)
        {
            NotEmpty(parameterName, "parameterName");

            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        public static string NotEmpty(string value, string parameterName)
        {
            if (ReferenceEquals(parameterName, null))
            {
                throw new ArgumentNullException("parameterName");
            }

            if (parameterName.Length == 0)
            {
                throw new ArgumentException(Resources.FormatArgumentIsEmpty("parameterName"));
            }

            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(parameterName);
            }

            if (value.Length == 0)
            {
                throw new ArgumentException(Resources.FormatArgumentIsEmpty(parameterName));
            }

            return value;
        }
    }
}