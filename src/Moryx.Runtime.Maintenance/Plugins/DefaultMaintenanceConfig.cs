// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Runtime.Maintenance.Contracts;

namespace Moryx.Runtime.Maintenance.Plugins
{
    /// <summary>
    /// Configuration for the default maintenance plugin.
    /// </summary>
    [DataContract]
    public class DefaultMaintenanceConfig : MaintenancePluginConfig
    {
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public override string PluginName => nameof(DefaultMaintenance);
    }
}
