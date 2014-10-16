// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
        private IContextAccessor<HttpContext> _contextAccessor;
        private static readonly object _requestIdentifier = new object();

        public ElmMiddleware(
            RequestDelegate next, ILoggerFactory factory, IOptions<ElmOptions> options, 
            IElmStore store, IContextAccessor<HttpContext> contextAccessor)
        {
            _next = next;
            _options = options.Options;
            _store = store;
            _contextAccessor = contextAccessor;
            _provider = new ElmLoggerProvider(_store, contextAccessor, _requestIdentifier);
            factory.AddProvider(_provider);
        }

        public async Task Invoke(HttpContext context)
        {
            _contextAccessor.SetContextSource(() => context, null);
            context.Items[_requestIdentifier] = Guid.NewGuid();
            if (!context.Request.Path.Value.Equals(_options.Path.Value ?? "/Elm"))
            {
                await _next(context);
                return;
            }
            var model = new LogPageModel() { Logs = _store.GetLogs() };
            var logPage = new LogPage(model);
            await logPage.ExecuteAsync(context);
        }
    }
}