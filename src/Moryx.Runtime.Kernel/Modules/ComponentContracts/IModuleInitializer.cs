// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    /// <summary>
    /// Component handling the initialize of a module
    /// </summary>
    internal interface IModuleInitializer
    {
        /// <summary>
        /// Initializes a module
        /// </summary>
        void Initialize(IServerModule module);
    }
}
