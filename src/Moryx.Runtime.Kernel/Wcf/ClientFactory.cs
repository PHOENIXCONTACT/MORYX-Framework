// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Kernel
{
    /// <summary>
    /// Factory for the clients.
    /// </summary>
    [InitializableKernelComponent(typeof(IWcfClientFactory))]
    public class ClientFactory : BaseWcfClientFactory, IInitializable, ILoggingHost
    {
        /// <summary>
        /// Injected by global container
        /// </summary>
        public IConfigManager ConfigManager { get; set; }

        /// <summary>
        /// Injected by global container
        /// </summary>
        public ILoggerManagement Logging { get; set; }

        /// <summary>
        /// Name of this host. Used for logger name structure
        /// </summary>
        public string Name { get { return "ClientFactory"; } }

        /// <summary>
        /// Initialize the client factory.
        /// </summary>
        public void Initialize()
        {
            Logging.ActivateLogging(this);

            ClientFactoryConfig factoryConfig = ConfigManager.GetConfiguration<ClientFactoryConfig>();
            ProxyConfig proxyConfig = ConfigManager.GetConfiguration<ProxyConfig>();

            Initialize(factoryConfig, proxyConfig, new SimpleThreadContext());
        }

    }
}
