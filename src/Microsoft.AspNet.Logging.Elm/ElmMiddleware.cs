// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Logging.Elm.Views;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.Logging.Elm
{
    /// <summary>
    /// Enables the Elm logging service
    /// </summary>
    public class ElmMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ElmOptions _options;
        private readonly ElmLoggerProvider _provider;
        private readonly IElmStore _store;
        private readonly ILogger _logger;
        private IContextAccessor<HttpContext> _contextAccessor;
        private static readonly object _requestIdentifierKey = new object();
        private static readonly object _logContextKey = new object();

        public ElmMiddleware(
            RequestDelegate next, ILoggerFactory factory, IOptions<ElmOptions> options, 
            IElmStore store, IContextAccessor<HttpContext> contextAccessor)
        {
            _next = next;
            _options = options.Options;
            _store = store;
            _logger = factory.Create<ElmMiddleware>();
            _contextAccessor = contextAccessor;
            _provider = new ElmLoggerProvider(_store, contextAccessor, _requestIdentifierKey, _logContextKey);
            factory.AddProvider(_provider);
            // non-request logs
            _logger.WriteWarning("hello world");
            _logger.WriteCritical("critical: aliens approaching");
        }

        public async Task Invoke(HttpContext context)
        {
            context.Items[_requestIdentifierKey] = Guid.NewGuid();
            if (context.Request.Path != _options.Path && !context.Request.Path.StartsWithSegments(_options.Path))
            {
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    _contextAccessor.SetContextSource(() => context, null);
                    _logger.WriteError("An unhandled exception has occurred: " + ex.Message, ex);
                    throw;
                }
            }
            
            // parse params
            var logs = (IEnumerable<LogInfo>)null;
            if (context.Request.Query.ContainsKey("level"))
            {
                var minLevel = (TraceType)int.Parse(context.Request.Query.GetValues("level")[0]);
                logs = _store.GetLogs(minLevel);
                _options.MinLevel = minLevel;
            }
            else
            {
                logs = _store.GetLogs();
                _options.MinLevel = TraceType.Verbose;
            }
            if (context.Request.Query.ContainsKey("name"))
            {
                var namePrefix = context.Request.Query.GetValues("name")[0];
                logs = logs.Where(l => l.Name.StartsWith(namePrefix));
                _options.NamePrefix = namePrefix;
            }

            // main log page
            if (context.Request.Path == _options.Path)
            {
                var model = new LogPageModel()
                {
                    // sort so most recent logs are first
                    Logs = logs.OrderBy(l => l.Time).Reverse(),
                    Options = _options
                };
                var logPage = new LogPage(model);
                await logPage.ExecuteAsync(context);
            }
            // request details page
            else
            {
                try
                {
                    var parts = context.Request.Path.Value.Split('/');
                    var id = Guid.Parse(parts[parts.Length - 1]);
                    var requestLogs = logs.Where(l => l.Context.RequestID == id);
                    var model = new RequestPageModel()
                    {
                        RequestID = id,
                        Logs = requestLogs,
                        Options = _options
                    };
                    var requestPage = new RequestPage(model);
                    await requestPage.ExecuteAsync(context);
                }
                catch (Exception)
                {
                    // TODO: bad url
                }
            }
        }
    }
}