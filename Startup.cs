using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CoreDemo
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app) 
        {
            app.UseFileServer(enableDirectoryBrowsing: true);

            app.Run(context =>
            {
                return context.Response.WriteAsync("Hello World from the web!");
            });
        }
    }
}