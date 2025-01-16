// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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

            host.Services.UseMoryxFileSystem("fs");
            host.Services.UseMoryxConfigurations("Config");

            var moduleManager = host.Services.GetRequiredService<IModuleManager>();
            moduleManager.StartModules();

            host.Run();

            moduleManager.StopModules();
        }
    }
}
