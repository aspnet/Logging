using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Logging.Elm;
using Microsoft.Framework.Logging;

namespace ElmSampleApp
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, ILoggerFactory factory)
        {
            app.UseElm(new ElmOptions());

            app.Run(async context =>
            {                
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Hello world");

                var logger = factory.Create("HelloWorld");
                logger.WriteCritical("Hello");
                logger.WriteInformation("World");
            });
        }
    }
}
