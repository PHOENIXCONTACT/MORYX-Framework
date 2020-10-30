// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Modules;

namespace Moryx.Runtime.Maintenance.Plugins.Databases
{
    /// <summary>
    /// Plugin which provides functions for database maintenance.
    /// </summary>
    [ExpectedConfig(typeof(DatabaseConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IMaintenancePlugin), Name = ComponentName)]
    internal class DatabasePlugin : MaintenancePluginBase<DatabaseConfig, IDatabaseMaintenance>
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public const string ComponentName = "DatabaseConfig";

        /// <summary>
        /// Maintenance model. Injected by castle.
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
