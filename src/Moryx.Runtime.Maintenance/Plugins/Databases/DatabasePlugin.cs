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
    }
}
