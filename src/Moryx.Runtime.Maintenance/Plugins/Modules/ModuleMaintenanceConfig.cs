// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

#if USE_WCF
using Moryx.Tools.Wcf;
#endif

namespace Moryx.Runtime.Maintenance.Plugins.Modules
{
    [DataContract]
    internal class ModuleMaintenanceConfig : MaintenancePluginConfig
    {
        public ModuleMaintenanceConfig()
        {
#if USE_WCF
            ProvidedEndpoint = new HostConfig
            {
                BindingType = ServiceBindingType.WebHttp,
                Endpoint = "modules",
                MetadataEnabled = true
            };
#endif
        }

        public override string PluginName => ModuleMaintenancePlugin.PluginName;
    }
}
