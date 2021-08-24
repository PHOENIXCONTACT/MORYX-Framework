// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Communication.Endpoints;
using Moryx.Logging;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Wcf
{
    internal class ConfiguredHostFactory : IConfiguredHostFactory, IEndpointHostFactory
    {
        public IModuleLogger Logger { get; set; }

        public ITypedHostFactory Factory { get; set; }

        private readonly IWcfHostFactory _hostFactory;
        public ConfiguredHostFactory(IWcfHostFactory hostFactory)
        {
            _hostFactory = hostFactory;
        }

        public IConfiguredServiceHost CreateHost<TContract>(HostConfig config)
        {
            return _hostFactory.CreateHost<TContract>(config, Factory, Logger);
        }

        public IConfiguredServiceHost CreateHost(Type contract, HostConfig config)
        {
            return _hostFactory.CreateHost(contract, config, Factory, Logger);
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
