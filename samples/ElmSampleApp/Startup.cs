using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Logging.Elm;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;

namespace ElmSampleApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IElmStore, ElmStore>(); // registering the service so it can be injected into constructors
            services.Configure<ElmOptions>(options =>
            {
                options.Path = new PathString("/foo");
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory factory)
        {
            app.UseElm();

            app.Run(async context =>
            {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Hello world");

                var logger = factory.Create("HelloWorld");
                context.Response.StatusCode = 200;
                logger.WriteInformation("Hello");
                context.Response.StatusCode = 400;
                logger.WriteCritical("World", new Exception("Bad Request"));
                context.Response.StatusCode = 404;
                logger.WriteWarning("Not Found", new Exception("Existential Crisis"));
            });
        }
    }
}
