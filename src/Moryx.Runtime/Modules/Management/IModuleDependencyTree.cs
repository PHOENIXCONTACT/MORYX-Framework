// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.Runtime.Modules
{
    /// <summary>
    /// Tree of module dependencies
    /// </summary>
    public interface IModuleDependencyTree
    {
        /// <summary>
        /// All server modules that do not require other modules to start
        /// </summary>
        IEnumerable<IModuleDependency> RootModules { get; } 
    }

    /// <summary>
    /// Branch within the dependency tree representing a single module
    /// </summary>
    public interface IModuleDependency
    {
        /// <summary>
        /// Module represented by this entry in the dependency tree
        /// </summary>
        IServerModule RepresentedModule { get; }

        /// <summary>
        /// All modules this module depends on
        /// </summary>
        IEnumerable<IModuleDependency> Dependencies { get; } 

        /// <summary>
        /// All modules that depend on this module
        /// </summary>
        IEnumerable<IModuleDependency> Dependends { get; } 
    }
}
