// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Modules;

namespace Moryx.Runtime.Maintenance.Plugins.Common
{
    /// <summary>
    /// Common maintenace plugin.
    /// </summary>
    [ExpectedConfig(typeof(CommonMaintenanceConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IMaintenancePlugin), Name = PluginName)]
    internal class CommonMaintenancePlugin : MaintenancePluginBase<CommonMaintenanceConfig, ICommonMaintenance>
    {
        internal const string PluginName = "CommonMaintenance";
    }
}
