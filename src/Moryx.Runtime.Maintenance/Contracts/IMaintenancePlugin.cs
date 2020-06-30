// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Marvin.Modules;

namespace Marvin.Runtime.Maintenance.Contracts
{
    /// <summary>
    /// Interface for the maintenance plugin.
    /// </summary>
    public interface IMaintenancePlugin : IConfiguredPlugin<MaintenancePluginConfig>, IDisposable
    {
    }
}
