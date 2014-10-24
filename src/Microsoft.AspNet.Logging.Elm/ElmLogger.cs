// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Elm;

namespace Microsoft.AspNet.Logging.Elm
{
    public class ElmLogger : ILogger
    {
        private readonly string _name;
        private readonly ElmLoggerProvider _provider;
        private IElmStore _store;
        private readonly IContextAccessor<HttpContext> _contextAccessor;
        private readonly object _requestIdentifierKey;
        private readonly object _logContextKey;

        public ElmLogger(string name, ElmLoggerProvider provider, IElmStore store,
                         IContextAccessor<HttpContext> contextAccessor, 
                         object requestIdentifierKey, object logContextKey)
        {
            _name = name;
            _provider = provider;
            _store = store;
            _contextAccessor = contextAccessor;
            _requestIdentifierKey = requestIdentifierKey;
            _logContextKey = logContextKey;
        }

        public void Write(TraceType traceType, int eventId, object state, Exception exception, 
                          Func<object, Exception, string> formatter)
        {
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
            if (ElmScope.Counts.ContainsKey(GetLogContext().RequestID))
            {
                // TODO: display nested scopes nicely
                for (var i = 0; i < ElmScope.Counts[GetLogContext().RequestID].Count; i++)
                {
                    state = "-----" + state;
                }
                info.State = state;
                info.Scopes = new List<Guid>(ElmScope.Counts[GetLogContext().RequestID]);
            }
            _store.Add(info);
        }

        public bool IsEnabled(TraceType traceType)
        {
            return true;
        }

        public IDisposable BeginScope(object state)
        {
            return new ElmScope(this, state, GetLogContext().RequestID);
        }

        private LogContext GetLogContext()
        {
            var context = _contextAccessor.Value;
            if (context == null)
            {
                // TODO: group non-request logs by Thread ID
                return new LogContext()
                {
                    ThreadID = Thread.CurrentThread.ManagedThreadId
                };
            }

            var logContext = context.Items[_logContextKey] as LogContext;
            if (logContext == null)
            {
                logContext = new LogContext()
                {
                    RequestID = (Guid)context.Items[_requestIdentifierKey],
                    Host = context.Request.Host,
                    ContentType = context.Request.ContentType,
                    Path = context.Request.Path,
                    Scheme = context.Request.Scheme,
                    StatusCode = context.Response.StatusCode,
                    User = context.User,
                    Method = context.Request.Method,
                    Protocol = context.Request.Protocol,
                    Headers = context.Request.Headers,
                    Query = context.Request.QueryString,
                    Cookies = context.Request.Cookies
                };
                context.Items[_logContextKey] = logContext;
            }

            return logContext;
        }
    }
}