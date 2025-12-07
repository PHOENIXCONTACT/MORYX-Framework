// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    internal interface IModuleStopper
    {
        /// <summary>
        /// Stop this plugin and all required dependencies
        /// </summary>
        /// <param name="module"></param>
        Task StopAsync(IServerModule module);

        /// <summary>
        /// Stop all services
        /// </summary>
        Task StopAllAsync();
    }
}
