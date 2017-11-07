using System;
using Marvin.Modules.ModulePlugins;

namespace Marvin.Runtime.Maintenance.Contracts
{
    /// <summary>
    /// Interface for the maintenance plugin.
    /// </summary>
    public interface IMaintenancePlugin : IConfiguredPlugin<MaintenancePluginConfig>, IDisposable
    {
    }
}
