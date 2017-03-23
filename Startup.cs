using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace CoreDemo
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) 
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseFileServer(enableDirectoryBrowsing: true);

            app.Run(context =>
            {
                throw new Exception("Nah.");

                return context.Response.WriteAsync("Hello World from the web!");
            });
        }
    }
}