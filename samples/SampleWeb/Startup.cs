// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async context =>
            {
                var logger = context.RequestServices.GetService<ILogger<Startup>>();
                logger.LogInformation("Starting");

                var startTime = DateTimeOffset.Now;
                logger.LogInformation(1, "Started at '{StartTime}' and 0x{Hello:X} is hex of 42", startTime, 42);
                // or
                logger.ProgramStarting(startTime, 42);

                using (logger.PurchaseOrderScope("00655321"))
                {
                    try
                    {
                        throw new Exception("Boom");
                    }
                    catch (Exception ex)
                    {
                        logger.LogCritical(1, ex, "Unexpected critical error starting application");
                        logger.LogError(1, ex, "Unexpected error");
                        logger.LogWarning(1, ex, "Unexpected warning");
                    }

                    using (logger.BeginScope("Main"))
                    {

                        logger.LogInformation("Waiting for user input");

                        var input = context.Request.QueryString.ToString();

                        logger.LogInformation("User typed '{input}' on the command line", input);
                        logger.LogWarning("The time is now {Time}, it's getting late!", DateTimeOffset.Now);
                    }
                }

                var endTime = DateTimeOffset.Now;
                logger.LogInformation(2, "Stopping at '{StopTime}'", endTime);
                // or
                logger.ProgramStopping(endTime);

                logger.LogInformation("Stopping");
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
