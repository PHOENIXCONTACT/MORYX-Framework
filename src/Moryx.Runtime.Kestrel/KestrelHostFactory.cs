using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moryx.Communication;
using Moryx.Communication.Endpoints;
using Moryx.Container;

namespace Moryx.Runtime.Kestrel
{
    internal class KestrelHostFactory : IEndpointHostFactory
    {
        private readonly IContainer _container;
        private readonly PortConfig _portConfig;
        private readonly EndpointCollector _collector;

        public KestrelHostFactory(IContainer container, EndpointCollector collector, PortConfig portConfig)
        {
            _container = container;
            _collector = collector;
            _portConfig = portConfig;
        }

        public IEndpointHost CreateHost(Type endpoint, object config)
        {
            var windsor = (_container as CastleContainer)?.Container;
            if (windsor == null)
                throw new InvalidOperationException("Kestrel hosting requires a windsor container based MORYX container!");

            var hostBuilder = Host.CreateDefaultBuilder()
                .UseWindsorContainerServiceProvider(windsor)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.Listen(new IPAddress(0), _portConfig.HttpPort);
                    }).ConfigureLogging(builder =>
                    {
                        builder.ClearProviders();
                    });
                    if (typeof(Controller).IsAssignableFrom(endpoint) && config == null)
                        // Default controller hosting
                        webBuilder.UseStartup(context => new DefaultStartup(context.Configuration, endpoint));
                    else if (typeof(Controller).IsAssignableFrom(endpoint) && config != null)
                        // Controller hosting with custom start-up
                        webBuilder.UseStartup(context => config);
                    else if(endpoint != null)
                        webBuilder.UseStartup(endpoint);
                });

            return new KestrelEndpointHost(hostBuilder);
        }
    }
}