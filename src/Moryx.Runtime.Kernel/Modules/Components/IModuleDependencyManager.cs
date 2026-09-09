// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel;

internal interface IModuleDependencyManager
{
    /// <summary>
    /// All facades on modules
    /// </summary>
    List<object> Facades { get; }

    /// <summary>
    /// Build and fill the dependency tree
    /// </summary>
    IReadOnlyList<IServerModule> BuildDependencyTree(IReadOnlyList<IServerModule> allModules);

    /// <summary>
    /// Get the full dependency tree
    /// </summary>
    /// <returns></returns>
    IModuleDependencyTree GetDependencyTree();

    /// <summary>
    /// Get all start dependencies of this plugin
    /// </summary>
    IModuleDependency GetDependencyBranch(IServerModule module);
}