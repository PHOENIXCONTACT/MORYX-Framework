// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Modules;

namespace Moryx.Container
{
    /// <summary>
    /// Factory create <see cref="IModule"/> specific container
    /// </summary>
    public interface IModuleContainerFactory
    {
        /// <summary>
        /// Creates a new container based on strategies and the module assembly
        /// </summary>
        IContainer Create(IDictionary<Type, string> strategies, Assembly moduleAssembly);
    }
}
