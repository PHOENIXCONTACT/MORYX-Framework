// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    internal interface IModuleDependencyManager
    {
        /// <summary>
        /// Build and fill the dependency tree
        /// </summary>
        IReadOnlyList<IServerModule> BuildDependencyTree(IReadOnlyList<IServerModule> allModules);

        /// <summary>
        /// Get the full dependency tree
        /// </summary>
        /// <returns></returns>
        IDependencyEvaluation GetDependencyEvalutaion();

        /// <summary>
        /// Get all start dependencies of this plugin
        /// </summary>
        IModuleDependency GetDependencyBranch(IServerModule module);
    }
}
