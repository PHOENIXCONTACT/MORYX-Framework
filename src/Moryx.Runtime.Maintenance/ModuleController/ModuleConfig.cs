// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Runtime.Maintenance.Contracts;
using Moryx.Runtime.Maintenance.Plugins;
using Moryx.Serialization;

namespace Moryx.Runtime.Maintenance
{
    /// <summary>
    /// Configuration of the maintenance module.
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <inheritdoc />
        protected override bool PersistDefaultConfig => true;

        /// <summary>
        /// List of configured maintenance modules.
        /// </summary>
        [DataMember]
        [PluginConfigs(typeof(IMaintenancePlugin), false)]
        [Description("List of configured maintenance modules.")]
        public List<MaintenancePluginConfig> Plugins { get; set; }

        /// <summary>
        /// If a logging appender exceeds this timeouts its config stream is closed
        /// </summary>
        [DataMember]
        [Description("If a logging appender exceeds this timeouts its config stream is closed")]
        [DefaultValue(30000)]
        public int AppenderTimeOut { get; set; }

        /// <summary>
        /// Folder where the setup data is stored.
        /// </summary>
        [DataMember]
        [RelativeDirectories]
        [DefaultValue(@".\Backups\")]
        public string SetupDataDir { get; set; }

        /// <summary>
        /// Initialize the maintenance module.
        /// </summary>
        protected override void Initialize()
        {
            Plugins = new List<MaintenancePluginConfig>
            {
                new DefaultMaintenanceConfig()
            };
        }
    }
}
