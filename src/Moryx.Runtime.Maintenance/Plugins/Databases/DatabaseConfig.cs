// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Serialization;

#if USE_WCF
using Moryx.Tools.Wcf;
#endif

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
#if USE_WCF
            ProvidedEndpoint = new HostConfig
            {
                Endpoint = DatabaseMaintenance.Endpoint,
                MetadataEnabled = true
            };
#endif
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
