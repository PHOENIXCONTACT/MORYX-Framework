// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Modules;

namespace Moryx.Runtime.Maintenance.Contracts
{
    /// <summary>
    /// Base configuration for a maintenance plugin.
    /// </summary>
    [DataContract]
    public abstract class MaintenancePluginConfig : UpdatableEntry, IPluginConfig
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public abstract string PluginName { get; }
    }
}
