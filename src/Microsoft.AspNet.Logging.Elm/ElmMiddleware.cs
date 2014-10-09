using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Elm;

namespace Microsoft.AspNet.Logging.Elm
{
    /// <summary>
    /// Enables the Elm logging service
    /// </summary>
    public class ElmMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ElmOptions _options;

        public ElmMiddleware(RequestDelegate next, ILoggerFactory factory, ElmOptions options)
        {
            _next = next;
            _options = options ?? new ElmOptions();
            factory.AddProvider(new ElmLoggerProvider());
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Value.Equals(_options.Path ?? "/Elm"))
            {
                await _next(context);
                return;
            }
            await context.Response.WriteAsync("------------Logs-----------\r\n");
            foreach (var log in ElmLog.Log)
            {
                await context.Response.WriteAsync(string.Format("{0}\r\n", log));
            }
            await context.Response.WriteAsync("---------------------------\r\n");
        }
    }
}