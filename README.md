# .NET Core Overview

## Documentation
* [Official Documentation](https://docs.microsoft.com/en-us/aspnet/core/)


## Installation
* [Install Link](https://www.microsoft.com/net/core)
    * SDK vs Runtime
* Runtime info and help
    * `dotnet`
* CLI Version
    * `dotnet --version`
* Review templates
    * `dotnet new`


## Making a Minimal Console Project
* `mkdir CoreDemo`
* `cd CoreDemo`
* `dotnet new console`
* The csproj file
* VS Code auto restore


## Running the Minimal App
* Run app
    * `dotnet run` - compile and run
    * `dotnet build` - compile only
* Edit, run again
* Watcher
    ```xml
    <ItemGroup>
        <DotNetCliToolReference Include="Microsoft.DotNet.Watcher.Tools" Version="1.0.0" />
    </ItemGroup>
    ```


## Adding ASP.NET
* Run `dotnet add package Microsoft.AspNetCore`
    * Auto restores package
* Add `Startup.cs`
    ```cs
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    namespace CoreDemo
    {
        public class Startup
        {
            public void Configure(IApplicationBuilder app) 
            {
                app.Run(context =>
                {
                    return context.Response.WriteAsync("Hello, World from the web!");
                });
            }
        }
    }
    ```
    * Note: `Configure` is a convention like `Main`
* Modify `Program.cs`
    ```cs
    using Microsoft.AspNetCore.Hosting;

    new WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(Directory.GetCurrentDirectory()())
        .UseStartup<Startup>()
        .Build()
        .Run();
    ```
    * Could also add things like `.UseIISIntegration()`, `.UseApplicationInsights()`, etc.
* `dotnet run`
    * Stays active
* Routes don't do anything


## Environments

* [Different ways to set](https://andrewlock.net/how-to-set-the-hosting-environment-in-asp-net-core/)
* Set `.UseEnvironment("Development")` in `Program.cs`
* Add `IHostingEnvironment env` to the `Configure`
    * `env.IsDevelopment()`, `env.IsStaging()`, and `env.IsProduction()` are built in


<!-- TODO -->
## Configuration
* [Overview](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration)


## Middleware

### Pipeline
* [Middleware Documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware)
* Old Handler and Module Overview
    
    ![Old Handler and Module Overview](https://docs.microsoft.com/en-us/aspnet/core/migration/http-modules/_static/moduleshandlers.png)
* New Middleware Overview

    ![New Middleware Overview](https://docs.microsoft.com/en-us/aspnet/core/migration/http-modules/_static/middleware.png)
* [Middleware Differences](http://www.talkingdotnet.com/asp-net-core-middleware-is-different-from-httpmodule/)
* Have order control now
* Host independent, doesn't rely on System.Web
* Not based on events
* Go over `app.Use()`, `app.Run()`, `app.Map()`, `app.When()`, `app.MapWhen()`

### Static Files
* Add a `wwwroot` folder
* Add `index.html`
    ```html
    <html>
    <body>
        Hello world from index.html!
    </body>
    </html>
    ```
* Show doesn't work
* Run `dotnet add package Microsoft.AspNetCore.StaticFiles`
* Add `app.UseStaticFiles();` in `Startup.cs` ABOVE `app.Run()`
* Show works, but path doesn't work
* Add `app.UseDefaultFiles();` in `Startup.cs` ABOVE `app.UseStaticFiles();`
* Note: `UseFileServer` combines the functionality of `UseStaticFiles`, `UseDefaultFiles`, and `UseDirectoryBrowser`
* Add `app.UseFileServer(enableDirectoryBrowsing: true);` in `Startup.cs` instead of previous commands
* Navigate to `http://localhost:5000/foo/`

### Error Handling
* Add error handling in `Startup.cs`
    ```cs
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    ```
* Add `throw new Exception("Nah.");` in the `app.Run` of `Startup.cs`
* Show detailed `500` error
* Use `.UseEnvironment("Production")` in `Program.cs`
* Show browser-based `500` error

### Custom Error Handling
* Can by used to log stuff, emails, exceptionless, etc.
* Add `UnauthorizedException.cs` that extends `Exception`
* Add `ApplicationBuilderExtensions.cs` to a new `Middleware` folder
    ```cs
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
    ```
* Add `ErrorHandlingMiddleware.cs`
    ```cs
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    namespace CoreDemo.Middleware
    {
        public class ErrorHandlingMiddleware 
        {
            private readonly RequestDelegate _next;

            public ErrorHandlingMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext context)
            {
                try 
                {
                    await _next(context);
                }
                catch(Exception exception) 
                {
                    await HandleException(context, exception);
                }
            }

            private Task HandleException(HttpContext context, Exception exception)
            {
                var code = HttpStatusCode.InternalServerError;
                    
                if (exception is UnauthorizedException)    
                {
                    code = HttpStatusCode.Unauthorized;
                }

                var result = JsonConvert.SerializeObject(new { error = exception.Message });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)code;
                return context.Response.WriteAsync(result);
            }
        }
    }
    ```
* Add our exception to `app.Run()` in `Startup.cs`
* Run the app, and observe a `401` thrown
* Change our custom exception to a regular exception and notice a `500` is thrown

### URL Rewriting
* Run `dotnet add package Microsoft.AspNetCore.Rewrite`
* Add the following to `Startup.cs` after the developer exception page middleware:
    ```cs
    app.UseRewriter(new RewriteOptions()
        .AddRedirectToHttps());
    ```
* Show redirects to HTTPS
* Change code to:
    ```cs
    app.UseRewriter(new RewriteOptions()
        .AddRewrite("rewrite", "foo/", true)
        .AddRedirect("redirect", "index.html"));
    ```
* Show redirects and rewrites
* Show other methods

### CORS
* Run `dotnet add package Microsoft.AspNetCore.Cors`
* Add the following to `Startup.cs` after the developer exception page middleware:
    ```cs
    app.UseCors(options => 
        {
            options.AllowAnyHeader();
            options.AllowAnyMethod();
            options.AllowAnyOrigin();
            options.AllowCredentials();
        });
    ```
* Add the `ConfigureServices` method to `Startup.cs`:
    ```cs
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors();
    }
    ```
* [Startup documenation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup)


## ASP.NET MVC

### General
* MVC and Web API have been combined
    * Everything extends `Controller`
* Run `dotnet add package Microsoft.AspNetCore.Mvc`

### Logging
* Add `ILoggerFactory loggerFactory` to the `Configure` method's parameters
* Add `logger.AddConsole();`
* Show request output

### Filters
* [Documentation](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters)
* Filter pipeline overview:

    ![Filter Pipeline Overview](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters/_static/filter-pipeline-2.png)
* Cross-cutting concerns
* [Differences between Middleware and Filters](https://andrewlock.net/exploring-middleware-as-mvc-filters-in-asp-net-core-1-1/)

### Routing
* [Documentation](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing)
* Default routing
* Attribute routing
    * Ability to add route to attributes

### Responses
* [Documentation](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/formatting)
* `IActionResult`
    * More specific results such as `JsonResult` or `ContentResult`
* `[Produces]`
* Use `[FormatFilter]` to allow formats to be specified in the URL
    * `api/foo/1.json` or `api/foo/1.xml`