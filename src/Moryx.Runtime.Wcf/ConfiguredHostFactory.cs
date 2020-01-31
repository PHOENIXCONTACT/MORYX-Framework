// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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

        public IConfiguredServiceHost CreateHost<T>(HostConfig config)
        {
            return _hostFactory.CreateHost<T>(config, Factory, Logger);
        }
    }
}
