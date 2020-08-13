// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Communication;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Runtime.Configuration;
using Moryx.Runtime.Maintenance.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Moryx.Runtime.Maintenance.Plugins
{
    /// <summary>
    /// Default maintenance plugin hosting the default API endpoint
    /// </summary>
    [ExpectedConfig(typeof(DefaultMaintenanceConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IMaintenancePlugin), Name = nameof(DefaultMaintenance))]
    internal class DefaultMaintenance : MaintenancePluginBase<DefaultMaintenanceConfig>
    {
        private IWebHost _host;

        public IContainer Container { get; set; }

        public IRuntimeConfigManager ConfigManager { get; set; }

        public override void Start()
        {
            var portConfig = ConfigManager.GetConfiguration<PortConfig>();

            try
            {
                var myStartup = new Startup(new HostingEnvironment(), Container);
                var builder = new ConfigurationBuilder();
                myStartup.Configuration = builder.Build();

                _host = new WebHostBuilder()
                    .UseKestrel()
                    .UseUrls($"http://{portConfig.Host}:{portConfig.HttpPort}")
                    .ConfigureServices(services =>
                    {
                        services.AddSingleton<IStartup>(myStartup);
                    })
                    .Build();

                _host.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public override void Dispose()
        {
            _host.Dispose();
            _host = null;
        }

        public override void Stop()
        {
            _host.StopAsync().Wait();
        }
    }
}
