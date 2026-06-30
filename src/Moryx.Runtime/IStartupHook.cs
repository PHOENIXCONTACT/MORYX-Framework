// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime;

/// <summary>
/// Base class for reusable hooks that should run during application startup.
/// See docs/articles/framework/startup-hooks.md
/// </summary>
public interface IStartupHook
{
    /// <summary>
    /// Lowest value gets executed first
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Execute the main functionality of the hook
    /// </summary>
    /// <returns></returns>
    Task RunAsync(CancellationToken cancellationToken);
}
