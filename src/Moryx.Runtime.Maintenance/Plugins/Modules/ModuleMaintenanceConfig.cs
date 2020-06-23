// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Runtime.Maintenance.Contracts;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Maintenance.Plugins.Modules
{
    [DataContract]
    internal class ModuleMaintenanceConfig : MaintenancePluginConfig
    {
        public ModuleMaintenanceConfig()
        {
            ProvidedEndpoint = new HostConfig
            {
                BindingType = ServiceBindingType.WebHttp,
                Endpoint = "ModuleMaintenance",
                MetadataEnabled = true
            };
        }

        public override string PluginName => ModuleMaintenancePlugin.PluginName;
    }
}
