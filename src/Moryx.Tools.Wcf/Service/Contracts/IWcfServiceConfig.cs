// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Configuration;
using Moryx.Modules;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Basic configuration for WCF service plugins
    /// </summary>
    public interface IWcfServiceConfig : IUpdatableConfig, IPluginConfig
    {
        /// <summary>
        /// The WCF host configuration
        /// </summary>
        HostConfig ConnectorHost { get; }
    }
}
