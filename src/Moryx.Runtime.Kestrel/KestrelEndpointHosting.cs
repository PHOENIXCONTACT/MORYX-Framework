// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moryx.Communication;
using Moryx.Communication.Endpoints;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Modules;

namespace Moryx.Runtime.Kestrel
{
    /// <summary>
    /// Kestrel Http host factory to start up a kestrel server
    /// </summary>
    [InitializableKernelComponent(typeof(IEndpointHosting), Name = "KestrelHosting")]
    internal class KestrelEndpointHosting : IInitializable, IEndpointHosting, IDisposable
    {
        #region Dependencies

        /// <summary>
        /// Config manager to receive the port config
        /// </summary>
        public IConfigManager ConfigManager { get; set; }

        #endregion

        #region Fields and Properties

        private PortConfig _portConfig;
        private WindsorContainer _hostingContainer;

        private IHost _host;
        private readonly List<ControllerProxySubResolver> _linkedControllers = new List<ControllerProxySubResolver>();

        internal static Type Startup { get; set; } = typeof(Startup);

        #endregion

        /// <inheritdoc />
        public void Initialize()
        {
            _portConfig = ConfigManager.GetConfiguration<PortConfig>();

            _hostingContainer = new WindsorContainer();

            var hostBuilder = Host.CreateDefaultBuilder()
                .UseWindsorContainerServiceProvider(_hostingContainer)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.Listen(new IPAddress(0), _portConfig.HttpPort);
                    }).ConfigureLogging(builder =>
                    {
                        builder.ClearProviders();
                    }).UseStartup(Startup);
                });
            _host = hostBuilder.Build();

            // Missing registrations
            _hostingContainer.Register(Component.For<EndpointCollector>());

            _host.Start();
        }

        public void ActivateHosting(IContainer container)
        {
            container.SetInstance(this);
            container.Register<IEndpointHost, KestrelEndpointHost>("KestrelHost", LifeCycle.Transient);
            // Let castle create the factory
            container.Register<IEndpointHostFactory>();
        }

        internal void LinkController(Type controller, IContainer moduleContainer)
        {
            var proxyResolver = new ControllerProxySubResolver(controller, moduleContainer);
            lock (_linkedControllers)
                _linkedControllers.Add(proxyResolver);
            _hostingContainer.Kernel.Resolver.AddSubResolver(proxyResolver);

            var routeAtt = controller.GetCustomAttribute<RouteAttribute>();
            var endpointAtt = controller.GetCustomAttribute<EndpointAttribute>();
            var route = "/";
            if (routeAtt != null)
                route += routeAtt.Template + "/";
            var address = $"http://{_portConfig.Host}:{_portConfig.HttpPort}{route}";
            _hostingContainer.Resolve<EndpointCollector>().AddEndpoint(address, new Endpoint
            {
                Address = address,
                Path = route,
                Service = endpointAtt?.Name ?? controller.Name,
                Version = endpointAtt?.Version ?? "1.0.0"
            });
        }

        internal void UnlinkController(Type controller, IContainer moduleContainer)
        {
            ControllerProxySubResolver proxyResolver;
            lock (_linkedControllers)
                proxyResolver = _linkedControllers.FirstOrDefault(p => p.Controller == controller && p.Container == moduleContainer);
            if (proxyResolver != null)
                _hostingContainer.Kernel.Resolver.RemoveSubResolver(proxyResolver);
        }

        public void Dispose()
        {
            _host.Dispose();
        }
    }
}
