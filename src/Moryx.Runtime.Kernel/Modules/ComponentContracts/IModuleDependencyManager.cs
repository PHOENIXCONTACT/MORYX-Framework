// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    internal interface IModuleDependencyManager : IModuleManagerComponent
    {
        /// <summary>
        /// Build and fill the dependency tree
        /// </summary>
        void BuildDependencyTree();

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
