// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
                "The string argument '{0}' cannot be empty",
                argumentName);
        }
    }
}