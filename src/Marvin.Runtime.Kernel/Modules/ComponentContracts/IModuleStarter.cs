// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Component handling the start of modules
    /// </summary>
    internal interface IModuleStarter : IModuleManagerComponent
    {
        /// <summary>
        /// Starts a module and all dependencies if necessary
        /// </summary>
        void Start(IServerModule module);

        /// <summary>
        /// Starts all modules
        /// </summary>
        void StartAll();
    }
}
