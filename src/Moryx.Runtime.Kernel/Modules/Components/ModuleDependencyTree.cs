// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    internal class PluginDependencyTree : IModuleDependencyTree
    {
        public PluginDependencyTree(IReadOnlyList<IModuleDependency> roots)
        {
            RootModules = roots;
        }

        public IReadOnlyList<IModuleDependency> RootModules { get; }
    }

    internal class ModuleDependencyBranch : IModuleDependency
    {
        public ModuleDependencyBranch(IServerModule representedModule)
        {
            RepresentedModule = representedModule;
        }

        /// <summary>
        /// Plugin represented by this entry in the dependency tree
        /// </summary>
        public IServerModule RepresentedModule { get; }

        public List<IModuleDependency> Dependencies { get; set; } = new List<IModuleDependency>();
        /// <summary>
        /// All modules this module depends on
        /// </summary>
        IReadOnlyList<IModuleDependency> IModuleDependency.Dependencies => Dependencies;

        /// <summary>
        /// All modules that depend on this module
        /// </summary>
        public List<IModuleDependency> Dependends { get; set; } = new List<IModuleDependency>();
        /// <summary>
        /// All modules that depend on this module
        /// </summary>
        IReadOnlyList<IModuleDependency> IModuleDependency.Dependents => Dependends;
    }
}

