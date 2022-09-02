using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Moryx.Asp.Integration;
using Moryx.Runtime.Kernel;
using System.IO;

namespace StartProject.Asp
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var directory = Directory.GetCurrentDirectory();

            // MORYX modifies current directory
            var moryxRuntime = new HeartOfGold(args);
            moryxRuntime.Load();

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddMoryxKernel(moryxRuntime);
                    serviceCollection.AddMoryxFacades(moryxRuntime);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseContentRoot(directory);
                    webBuilder.UseStartup<Startup>();
                }).Build();

            host.Start();
            var result = moryxRuntime.Execute();
            host.Dispose();

            return (int)result;
        }
    }
}
