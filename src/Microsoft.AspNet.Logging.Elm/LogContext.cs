﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Claims;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.Logging.Elm
{
    public class LogContext
    {
        public int ThreadID { get; set; }

        public Guid RequestID { get; set; }

        public HostString Host { get; set; }

        public PathString Path { get; set; }

        public string ContentType { get; set; }

        public string Scheme { get; set; }

        public int StatusCode { get; set; }
        
        public ClaimsPrincipal User { get; set; }

        public string Method { get; set; }

        public string Protocol { get; set; }

        public IHeaderDictionary Headers { get; set; }

        public QueryString Query { get; set; }

        public IReadableStringCollection Cookies { get; set; }
    }
}