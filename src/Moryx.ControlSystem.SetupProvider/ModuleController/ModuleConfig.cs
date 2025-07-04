// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.ControlSystem.Setups;
using Moryx.Serialization;

namespace Moryx.ControlSystem.SetupProvider
{
    /// <summary>
    /// Module configuration of the ProcessEngine <see cref="ModuleController"/>
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <summary>
        /// All configured setup triggers for this application
        /// </summary>
        [DataMember, PluginConfigs(typeof(ISetupTrigger))]
        public List<SetupTriggerConfig> SetupTriggers { get; set; }

    }
}
