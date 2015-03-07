// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.EventLog;

namespace EventLoggingASPNet
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
        }
        
        // This method gets called by a runtime.
        // Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor = null)
        {
            loggerFactory.AddWindowsEventLogIfHasPermission(httpContextAccessor, (ex, httpContext, stringBuilder) => {
                var isSSL = (httpContext.Request.Scheme == "https");
                stringBuilder.Append("Is SSL/TLS: ");
                stringBuilder.AppendLine((isSSL ? "Yes" : "No"));
            });
            
            services.AddMvc();
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
            // Add MVC to the request pipeline.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
