// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moryx.Communication;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Runtime.Configuration;
using Moryx.Runtime.Maintenance.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Nancy;
using Nancy.Owin;
using Nancy.Responses.Negotiation;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Moryx.Runtime.Maintenance.Plugins
{
    public class Startup : IStartup
    {
        private readonly IContainer _container;
        public IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env, IContainer container)
        {
            _container = container;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services
            services.AddCors();

            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UsePathBase("/maintenance");
            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials()
            );

            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "maintenance");

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(filepath),
                RequestPath = ""
            });

            app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new MoryxNancyBootstrapper(_container)));
        }
    }

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

    public sealed class CustomJsonNetSerializer : JsonSerializer, ISerializer
    {
        public CustomJsonNetSerializer()
        {
            ContractResolver = new DefaultContractResolver();// new CamelCasePropertyNamesContractResolver();
            DateFormatHandling = DateFormatHandling.IsoDateFormat;
            Formatting = Formatting.None;
        }
        public bool CanSerialize(MediaRange mediaRange)
        {
            return mediaRange.ToString().Equals("application/json", StringComparison.OrdinalIgnoreCase);
        }

        public void Serialize<TModel>(MediaRange mediaRange, TModel model, Stream outputStream)
        {
            using (var streamWriter = new StreamWriter(outputStream))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                Serialize(jsonWriter, model);
            }
        }

        public IEnumerable<string> Extensions { get { yield return "json"; } }
    }
    internal class ContainerModuleCatalog : INancyModuleCatalog
    {
        private readonly IContainer _container;

        public ContainerModuleCatalog(IContainer container)
        {
            _container = container;
        }

        public IEnumerable<INancyModule> GetAllModules(NancyContext context)
        {
            var modules = _container.ResolveAll<INancyModule>().ToArray();
            return modules;
        }

        public INancyModule GetModule(Type moduleType, NancyContext context)
        {
            return (INancyModule)_container.Resolve(moduleType);
        }
    }

    internal class MoryxNancyBootstrapper : DefaultNancyBootstrapper
    {
        private readonly IContainer _container;

        public MoryxNancyBootstrapper(IContainer container)
        {
            _container = container;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register<INancyModuleCatalog>(new ContainerModuleCatalog(_container));
            container.Register<ISerializer, CustomJsonNetSerializer>();
        }
    }
}
