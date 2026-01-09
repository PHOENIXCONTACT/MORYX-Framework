// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel;

/// <summary>
/// Component handling the start of modules
/// </summary>
internal interface IModuleStarter
{
    /// <summary>
    /// Call initialize on the module
    /// </summary>
    Task InitializeAsync(IServerModule module, CancellationToken cancellationToken);

    /// <summary>
    /// Starts a module and all dependencies if necessary
    /// </summary>
    Task StartAsync(IServerModule module, CancellationToken cancellationToken);

    /// <summary>
    /// Starts all modules
    /// </summary>
    Task StartAllAsync(CancellationToken cancellationToken);
}