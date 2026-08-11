// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Tools;

/// <summary>
/// Interface for hooks that participate in a lifecycle.
/// </summary>
public interface ILifecycleHook
{
    /// <summary>
    /// Lowest value gets executed first
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Execute the main functionality of the hook
    /// </summary>
    /// <returns></returns>
    Task RunAsync(CancellationToken cancellationToken = default);
}
