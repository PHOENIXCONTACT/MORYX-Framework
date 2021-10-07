// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Communication.Endpoints;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Wcf
{
    internal class ConfiguredHostFactory : IConfiguredHostFactory
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
    }
}
