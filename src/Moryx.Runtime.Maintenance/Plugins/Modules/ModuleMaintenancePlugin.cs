// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Modules;
using Moryx.Runtime.Maintenance.Contracts;

namespace Moryx.Runtime.Maintenance.Plugins.Modules
{
    [ExpectedConfig(typeof(ModuleMaintenanceConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IMaintenancePlugin), Name = PluginName)]
    internal class ModuleMaintenancePlugin : MaintenancePluginBase<ModuleMaintenanceConfig, IModuleMaintenance>
    {
        public const string PluginName = "ModuleMaintenance";
    }
}
