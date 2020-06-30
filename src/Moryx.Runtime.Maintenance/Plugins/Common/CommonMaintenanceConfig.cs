// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Runtime.Maintenance.Contracts;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Maintenance.Plugins.Common
{
    /// <summary>
    /// Configuration for the common maintenance plugin.
    /// </summary>
    [DataContract]
    public class CommonMaintenanceConfig : MaintenancePluginConfig
    {
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public override string PluginName => CommonMaintenancePlugin.PluginName;

        /// <summary>
        /// Constructor for the common maintenance config. Creates an "CommonMaintenance" endpoint with binding type "BasicHttp".
        /// </summary>
        public CommonMaintenanceConfig()
        {
            ProvidedEndpoint = new HostConfig()
            {
                Endpoint = "CommonMaintenance",
                BindingType = ServiceBindingType.WebHttp,
            };
        }
    }
}
