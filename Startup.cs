using CoreDemo.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;

namespace CoreDemo
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) 
        {
            app.UseErrorHandling();

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