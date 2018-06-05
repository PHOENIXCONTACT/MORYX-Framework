using System;
using System.Collections.Generic;
using System.Reflection;
using Marvin.Modules;

namespace Marvin.Container
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