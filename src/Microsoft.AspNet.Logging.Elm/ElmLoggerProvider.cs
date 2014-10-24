﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Logging.Elm
{
    public class ElmLoggerProvider : ILoggerProvider
    {
        private readonly IElmStore _store;
        private readonly IContextAccessor<HttpContext> _contextAccessor;
        private readonly object _requestIdentifierKey;
        private readonly object _logContextKey;

        public ElmLoggerProvider(IElmStore store, IContextAccessor<HttpContext> contextAccessor, 
                                 object requestIdentifierKey, object logContextKey)
        {
            _store = store;
            _contextAccessor = contextAccessor;
            _requestIdentifierKey = requestIdentifierKey;
            _logContextKey = logContextKey;
        }

        public ILogger Create(string name)
        {
            return new ElmLogger(name, this, _store, _contextAccessor, _requestIdentifierKey, _logContextKey);      
        }
    }
}
