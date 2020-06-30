// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules
{
    /// <summary>
    /// Module interface for all platform extension modules
    /// </summary>
    public interface IPlatformModule : IServerModule
    {
        /// <summary>
        /// Module manager reference filled by the module manager himself
        /// </summary>
        void SetModuleManager(IModuleManager moduleManager);
    }
}
