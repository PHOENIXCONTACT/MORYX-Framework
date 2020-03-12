// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Container;
using Marvin.Modules;
using Marvin.Runtime.Maintenance.Contracts;
using Marvin.Runtime.Maintenance.Plugins;

namespace Marvin.Runtime.Maintenance.Web
{
    [ExpectedConfig(typeof(MaintenanceWebConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IMaintenancePlugin), Name = PluginName)]
    [DependencyRegistration(typeof(IWebServerFileSystem))]
    internal class MaintenanceWeb : MaintenancePluginBase<MaintenanceWebConfig, IWebServerFileSystem>
    {
        public const string PluginName = "MaintenanceWeb";
    }
}
