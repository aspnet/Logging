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
            services.AddSingleton<IElmStore, ElmStore>(); // registering the service so it can be injected into constructors
            services.Configure<ElmOptions>(options =>
            {
                options.Path = new PathString("/foo");
            });
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory factory)
        {
            app.UseErrorPage();
            app.UseElm();
            app.UseMvc();
#pragma warning disable CS1998
            app.Run(async context =>
#pragma warning restore CS1998
            {
                throw new InvalidOperationException();
            });
        }
    }

    public class HomeController
    {
        public string Index()
        {
            return "Hello World";
        }
    }
}
