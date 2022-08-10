using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Moryx.Runtime.Kernel;
using Moryx.Tools;
using Moryx.Runtime.Modules;

namespace StartProject.Asp
{
    public class Program
    {
        public static void Main(string[] args)
        {            
            AppDomainBuilder.LoadAssemblies();

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddMoryxKernel();
                    serviceCollection.AddMoryxModules();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).Build();

            host.Services.UseMoryxConfigurations("Config");
            host.Services.StartMoryxModules();

            host.Run();
        }
    }
}
