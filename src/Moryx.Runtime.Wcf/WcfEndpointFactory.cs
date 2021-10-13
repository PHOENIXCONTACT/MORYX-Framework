using System;
using Moryx.Communication.Endpoints;
using Moryx.Logging;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Wcf
{
    internal class WcfEndpointFactory : IEndpointHostFactory
    {
        public IModuleLogger Logger { get; set; }

        public ITypedHostFactory Factory { get; set; }

        private readonly IWcfHostFactory _hostFactory;
        public WcfEndpointFactory(IWcfHostFactory hostFactory)
        {
            _hostFactory = hostFactory;
        }

        public IEndpointHost CreateHost(Type endpoint, object config)
        {
            var hostConfig = config as HostConfig;
            if (hostConfig == null)
                throw new ArgumentException("Wcf hosting requires config of type HostConfig");

            return (IEndpointHost)_hostFactory.CreateHost(endpoint, hostConfig, Factory, Logger);
        }
    }
}