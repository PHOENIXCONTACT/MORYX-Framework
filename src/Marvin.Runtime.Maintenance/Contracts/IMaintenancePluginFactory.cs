// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Container;

namespace Marvin.Runtime.Maintenance.Contracts
{
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    internal interface IMaintenancePluginFactory
    {
        /// <summary>
        /// Create the maintenance module with this name
        /// </summary>
        IMaintenancePlugin Create(MaintenancePluginConfig config);

        /// <summary>
        /// Destroy a module instance
        /// </summary>
        void Destroy(IMaintenancePlugin instance);
    }
}
