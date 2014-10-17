// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Http;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Logging.Elm
{
    /// <summary>
    /// Options for ElmMiddleware
    /// </summary>
    public class ElmOptions
    {
        public ElmOptions()
        {
            Path = new PathString("/Elm");
        }

        /// <summary>
        /// Specifies the path to view the logs
        /// </summary>
        public PathString Path { get; set; }

        /// <summary>
        /// The minimum severity level shown
        /// </summary>
        public TraceType MinLevel { get; set; }

        /// <summary>
        /// prefix filter for the loggers shown
        /// </summary>
        public string NamePrefix { get; set; }
    }
}