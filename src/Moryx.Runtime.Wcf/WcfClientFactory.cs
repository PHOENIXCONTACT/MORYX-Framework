// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Communication;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Wcf
{
    /// <summary>
    /// Factory for the clients.
    /// </summary>
    [InitializableKernelComponent(typeof(IWcfClientFactory))]
    internal class WcfClientFactory : BaseWcfClientFactory, IInitializable, ILoggingHost
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
        public string Name => "ClientFactory";

        /// <summary>
        /// Initialize the client factory.
        /// </summary>
        public void Initialize()
        {
            Logging.ActivateLogging(this);

            var factoryConfig = ConfigManager.GetConfiguration<ClientFactoryConfig>();
            var proxyConfig = ConfigManager.GetConfiguration<ProxyConfig>();

            Initialize(factoryConfig, proxyConfig, new SimpleThreadContext());
        }
    }
}
