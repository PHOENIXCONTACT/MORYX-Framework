// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Logging;
using Moryx.Modules;

namespace Moryx.AbstractionLayer.Resources;

/// <summary>
/// Base class for resource initializers without a custom config
/// </summary>
public abstract class ResourceInitializerBase : ResourceInitializerBase<ResourceInitializerConfig>
{
}

/// <summary>
/// Base class for resource initializers with a custom config
/// </summary>
public abstract class ResourceInitializerBase<TConfig> : IResourceInitializer, ILoggingComponent
    where TConfig : ResourceInitializerConfig
{
    /// <summary>
    /// Configuration of the resourceInitializer
    /// </summary>
    public TConfig Config { get; private set; }

    /// <summary>
    /// Yes, this is a logger!
    /// </summary>
    public IModuleLogger Logger { get; set; }

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public abstract string Description { get; }

    /// <inheritdoc />
    public Task InitializeAsync(ResourceInitializerConfig config, CancellationToken cancellationToken = default)
    {
        // Get child logger
        Logger = Logger.GetChild(Name, GetType());

        // Cast configuration
        Config = (TConfig)config;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task StartAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <inheritdoc />
    public virtual Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public abstract Task<ResourceInitializerResult> ExecuteAsync(IResourceGraph graph, object parameters, CancellationToken cancellationToken);

    /// <summary>
    /// Creates an <see cref="ResourceInitializerResult"/> within a completed task
    /// </summary>
    /// <param name="initializedResources">Initialized resources, only roots should be returned as resources are saved recursively</param>
    [Obsolete("Will be removed in the next major. Replace with direct 'new ResourceInitializerResult()' instantiation.")]
    protected static Task<ResourceInitializerResult> InitializedAsync(IReadOnlyList<Resource> initializedResources) =>
        InitializedAsync(initializedResources, false);

    /// <summary>
    /// Creates an <see cref="ResourceInitializerResult"/> within a completed task
    /// </summary>
    /// <param name="initializedResources">Initialized resources, only roots should be returned as resources are saved recursively</param>
    /// <param name="saved">If true, the resources will not be saved, but are assumed to already have been saved within the initializer</param>
    [Obsolete("Will be removed in the next major. Replace with direct 'new ResourceInitializerResult()' instantiation.")]
    protected static Task<ResourceInitializerResult> InitializedAsync(IReadOnlyList<Resource> initializedResources, bool saved) =>
        Task.FromResult(Initialized(initializedResources, saved));

    /// <summary>
    /// Creates an <see cref="ResourceInitializerResult"/>
    /// </summary>
    /// <param name="initializedResources">Initialized resources, only roots should be returned as resources are saved recursively</param>
    [Obsolete("Will be removed in the next major. Replace with direct 'new ResourceInitializerResult()' instantiation.")]
    protected static ResourceInitializerResult Initialized(IReadOnlyList<Resource> initializedResources) =>
        Initialized(initializedResources, false);

    /// <summary>
    /// Creates an <see cref="ResourceInitializerResult"/>
    /// </summary>
    /// <param name="initializedResources">Initialized resources, only roots should be returned as resources are saved recursively</param>
    /// <param name="saved">If true, the resources will not be saved, but are assumed to already have been saved within the initializer</param>
    [Obsolete("Will be removed in the next major. Replace with direct 'new ResourceInitializerResult()' instantiation.")]
    protected static ResourceInitializerResult Initialized(IReadOnlyList<Resource> initializedResources, bool saved)
    {
        return new ResourceInitializerResult
        {
            Saved = saved,
            InitializedResources = initializedResources
        };
    }
}
