// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Container;
using Marvin.Modules;
using Marvin.Runtime.Maintenance.Contracts;

namespace Marvin.Runtime.Maintenance.Plugins.Databases
{
    /// <summary>
    /// Plugin which provides functions for database maintenance.
    /// </summary>
    [ExpectedConfig(typeof(DatabaseConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IMaintenancePlugin), Name = ComponentName)]
    public class DatabasePlugin : MaintenancePluginBase<DatabaseConfig, IDatabaseMaintenance>
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public const string ComponentName = "DatabaseConfig";

        /// <summary>
        /// Maintenacne model. Injected by castle.
        /// </summary>
        public IDatabaseMaintenance ModelMaintenance { get; set; }

        /// <summary>
        /// initialize the database plugin. 
        /// </summary>
        /// <param name="config">The database maintenance config.</param>
        public override void Initialize(MaintenancePluginConfig config)
        {
            base.Initialize(config);
            ((DatabaseMaintenance)ModelMaintenance).Config = Config;
        }
    }
}
