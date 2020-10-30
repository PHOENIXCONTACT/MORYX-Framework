// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Serialization;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Maintenance.Plugins.Databases
{
    /// <summary>
    /// Configuration for the database maintenance plugin.
    /// </summary>
    [DataContract]
    internal class DatabaseConfig : MaintenancePluginConfig
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public override string PluginName => DatabasePlugin.ComponentName;

        /// <summary>
        /// Provide an endpoint named "DatabaseMaintenance" with binding type "BasicHttp".
        /// </summary>
        public DatabaseConfig()
        {
            ProvidedEndpoint = new HostConfig
            {
                BindingType = ServiceBindingType.WebHttp,
                Endpoint = "databases",
                MetadataEnabled = true
            };
        }

        /// <summary>
        /// Folder where the setup data is stored.
        /// </summary>
        [DataMember]
        [RelativeDirectories]
        [DefaultValue(@".\Backups\")]
        public string SetupDataDir { get; set; }
    }
}
