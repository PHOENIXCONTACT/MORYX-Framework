// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
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
        void Stop(IServerModule module);

        /// <summary>
        /// Stop all services
        /// </summary>
        void StopAll();
    }
}
