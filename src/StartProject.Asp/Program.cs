// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Kernel;
using Moryx.Tools;
using Moryx.Model;
using Moryx.Runtime.Modules;

namespace StartProject.Asp
{
    public class Program
    {
        public static async Task Main(string[] args)
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
            await moduleManager.StartModulesAsync();

            await host.RunAsync();

            await moduleManager.StopModulesAsync();
        }
    }
}
