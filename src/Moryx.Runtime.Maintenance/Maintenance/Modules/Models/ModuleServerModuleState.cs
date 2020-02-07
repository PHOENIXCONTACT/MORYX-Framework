// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Maintenance.Modules
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
