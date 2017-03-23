using CoreDemo.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreDemo
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger) 
        {
            app.UseErrorHandling();

            app.UseCors(options => 
            {
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.AllowAnyOrigin();
                options.AllowCredentials();
            });

            logger.AddConsole();

            app.UseRewriter(new RewriteOptions()
                .AddRewrite("rewrite", "foo/", true)
                .AddRedirect("redirect", "index.html"));

            app.UseFileServer(enableDirectoryBrowsing: true);

            app.Run(async context => 
            {
                await context.Response.WriteAsync("Hello World from the web!");
            });
        }
    }
}