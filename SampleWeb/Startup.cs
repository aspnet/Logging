using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleApp;

namespace SampleWeb
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder => builder
                .AddAzureWebAppDiagnostics()
                .AddConsole());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                var _logger = context.RequestServices.GetService<ILogger<Startup>>();
                _logger.LogInformation("Starting");

                var startTime = DateTimeOffset.Now;
                _logger.LogInformation(1, "Started at '{StartTime}' and 0x{Hello:X} is hex of 42", startTime, 42);
                // or
                _logger.ProgramStarting(startTime, 42);

                using (_logger.PurchaseOrderScope("00655321"))
                {
                    try
                    {
                        throw new Exception("Boom");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(1, ex, "Unexpected critical error starting application");
                        _logger.LogError(1, ex, "Unexpected error");
                        _logger.LogWarning(1, ex, "Unexpected warning");
                    }

                    using (_logger.BeginScope("Main"))
                    {

                        _logger.LogInformation("Waiting for user input");

                        var input = context.Request.QueryString.ToString();

                        _logger.LogInformation("User typed '{input}' on the command line", input);
                        _logger.LogWarning("The time is now {Time}, it's getting late!", DateTimeOffset.Now);
                    }
                }

                var endTime = DateTimeOffset.Now;
                _logger.LogInformation(2, "Stopping at '{StopTime}'", endTime);
                // or
                _logger.ProgramStopping(endTime);

                _logger.LogInformation("Stopping");
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
