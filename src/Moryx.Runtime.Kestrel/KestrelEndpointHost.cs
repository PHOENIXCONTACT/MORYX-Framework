using System;
using Microsoft.Extensions.Hosting;
using Moryx.Communication.Endpoints;
using Moryx.Container;

namespace Moryx.Runtime.Kestrel
{
    internal class KestrelEndpointHost : IEndpointHost, IDisposable
    {
        private readonly Type _endpoint;
        private readonly object _config;

        /// <summary>
        /// Injected container of the module
        /// </summary>
        public IContainer ModuleContainer { get; set; }
        /// <summary>
        /// Kestrel hosting responsible for the controller
        /// </summary>
        public KestrelEndpointHosting Hosting { get; set; }

        public KestrelEndpointHost(Type endpoint) : this(endpoint, null)
        {
        }

        public KestrelEndpointHost(Type endpoint, object config)
        {
            _endpoint = endpoint;
            _config = config;
        }

        public void Start()
        {
            Hosting.LinkController(_endpoint, ModuleContainer);
        }

        public void Stop()
        {
            Hosting.UnlinkController(_endpoint, ModuleContainer);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}