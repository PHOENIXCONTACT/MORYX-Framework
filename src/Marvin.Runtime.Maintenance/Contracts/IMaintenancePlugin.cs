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
