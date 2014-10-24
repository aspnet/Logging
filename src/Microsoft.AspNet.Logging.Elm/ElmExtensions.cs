// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Logging.Elm;

namespace Microsoft.AspNet.Builder
{
    public static class ElmExtensions
    {
        public static IApplicationBuilder UseElm(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ElmMiddleware>();
        }
    }
}