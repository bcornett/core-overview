using Microsoft.AspNetCore.Builder;

namespace CoreDemo.Middleware 
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseErrorHandling(this IApplicationBuilder app) 
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}