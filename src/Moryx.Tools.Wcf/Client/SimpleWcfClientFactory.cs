// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Communication;
using Moryx.Configuration;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Simple implementation of a WCF client factory for console applications or system tests.
    /// </summary>
    public class SimpleWcfClientFactory : BaseWcfClientFactory
    {
        /// <summary>
        /// Initializes this factory without proxy configuration.
        /// </summary>
        /// <param name="config">The configuration of this factory.</param>
        public void Initialize(IWcfClientFactoryConfig config)
        {
            Initialize(config, null);
        }

        /// <summary>
        /// Initializes this factory.
        /// </summary>
        /// <param name="config">The configuration of this factory.</param>
        /// <param name="proxyConfig">An optional proxy configuration.</param>
        public void Initialize(IWcfClientFactoryConfig config, IProxyConfig proxyConfig)
        {
            Initialize(config, proxyConfig, new SimpleThreadContext());
        }
    }
}
