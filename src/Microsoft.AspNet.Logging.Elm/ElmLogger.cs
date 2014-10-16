// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Logging.Elm
{
    public class ElmLogger : ILogger
    {
        private readonly string _name;
        private readonly ElmLoggerProvider _provider;
        private IElmStore _store;
        private readonly IContextAccessor<HttpContext> _contextAccessor;
        private readonly object _requestIdentifierKey;

        public ElmLogger(string name, ElmLoggerProvider provider, IElmStore store, IContextAccessor<HttpContext> contextAccessor, object requestIdentifierKey)
        {
            _name = name;
            _provider = provider;
            _store = store;
            _contextAccessor = contextAccessor;
            _requestIdentifierKey = requestIdentifierKey;
        }

        public void Write(TraceType traceType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            var message = string.Empty;
            if (formatter != null)
            {
                message = formatter(state, exception);
            }
            else
            {
                if (state != null)
                {
                    message += state;
                }
                if (exception != null)
                {
                    message += Environment.NewLine + exception;
                }
            }
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            LogInfo info = new LogInfo()
            {
                Context = GetLogContext(),
                Name = _name,
                EventID = eventId,
                Severity = traceType,
                Exception = exception,
                State = state,
                Time = DateTime.Now
            };
            _store.Write(info);
        }

        public bool IsEnabled(TraceType traceType)
        {
            return true;
        }

        public IDisposable BeginScope(object state)
        {
            // TODO: use NullDisposable once it's moved to this repo #33
            return null;
        }

        private LogContext GetLogContext()
        {
            var context = _contextAccessor.Value;
            if (context == null)
            {
                return new LogContext();
            }
            else
            {
                return new LogContext()
                {
                    RequestID = (Guid)context.Items[_requestIdentifierKey],
                    Host = context.Request.Host,
                    ContentType = context.Request.ContentType,
                    Path = context.Request.Path,
                    Scheme = context.Request.Scheme,
                    StatusCode = context.Response.StatusCode,
                    User = context.User.Identity.Name
                };
            }
        }
    }
}