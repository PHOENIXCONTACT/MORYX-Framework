// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Marvin.Runtime.Maintenance.Contracts;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Maintenance.Web
{
    [DataContract]
    public class MaintenanceWebConfig : MaintenancePluginConfig
    {
        /// <summary>
        /// Constructor for web server config. Creates an endpoint with name "MaintenanceWeb".
        /// </summary>
        public MaintenanceWebConfig()
        {
            ProvidedEndpoint = new HostConfig
            {
                Endpoint = "MaintenanceWeb",
            };
        }

        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public override string PluginName => MaintenanceWeb.PluginName;
    }
}
