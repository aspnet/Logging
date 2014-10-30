using System;
using System.Globalization;

namespace Microsoft.Framework.Logging.Serilog
{
    internal static class Resources
    {
        public static class Validation
        {
            public const string Required = "This field is required";
            public const string MaxLength = "This field must be no more than {1} characters in length";
        }

        public static string FormatArgumentIsEmpty(object argumentName)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "The string argument '{argumentName}' cannot be empty",
                argumentName);
        }
    }
}