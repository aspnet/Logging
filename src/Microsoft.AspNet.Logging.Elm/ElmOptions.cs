// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Http;

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
        public PathString Path { get; set; }
    }
}