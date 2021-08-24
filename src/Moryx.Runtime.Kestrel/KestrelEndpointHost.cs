using System;
using Microsoft.Extensions.Hosting;
using Moryx.Communication.Endpoints;

namespace Moryx.Runtime.Kestrel
{
    internal class KestrelEndpointHost : IEndpointHost
    {
        private IHost _host;

        public IHostBuilder HostBuilder { get; }

        public KestrelEndpointHost(IHostBuilder hostBuilder)
        {
            HostBuilder = hostBuilder;
        }

        public void Start()
        {
            if (_host == null)
            {
                _host = HostBuilder.Build();
                _host.Start();
            }
            else
            {
                throw new InvalidOperationException("Host already running!");
            }
        }

        public void Stop()
        {
            _host?.StopAsync();
        }
    }
}