using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace CoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            new WebHostBuilder()
                .UseEnvironment("Development")
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
