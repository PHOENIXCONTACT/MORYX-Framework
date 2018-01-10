﻿namespace Marvin.Runtime.Maintenance.Plugins.ModuleMaintenance.Wcf.Models
{
    /// <summary>
    /// Server module state
    /// </summary>
    public enum ModuleServerModuleState
    {
        /// <summary>
        /// Initial value
        /// </summary>
        Stopped,

        /// <summary>
        /// Module is initializing
        /// </summary>
        Initializing,

        /// <summary>
        /// Service is ready to be started
        /// </summary>
        Ready,

        /// <summary>
        /// Service is starting
        /// </summary>
        Starting,

        /// <summary>
        /// Service is running
        /// </summary>
        Running,

        /// <summary>
        /// Service is stopping
        /// </summary>
        Stopping,

        /// <summary>
        /// Service failed
        /// </summary>
        Failure,
    }
}