using Moryx.Runtime.Kernel;
using Moryx.Tools;
using Moryx.Model;
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
                    serviceCollection.AddMoryxModels();
                    serviceCollection.AddMoryxModules();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).Build();

            host.Services.UseMoryxConfigurations("Config");

            var moduleManager = host.Services.GetRequiredService<IModuleManager>();
            moduleManager.StartModules();

            host.Run();

            moduleManager.StopModules();
        }
    }
}
