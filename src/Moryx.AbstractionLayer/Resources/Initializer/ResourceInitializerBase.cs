// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Logging;
using Moryx.Modules;

namespace Moryx.AbstractionLayer.Resources
{
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
        public void Initialize(ResourceInitializerConfig config)
        {
            // Get child logger
            Logger = Logger.GetChild(Name, GetType());

            // Cast configuration
            Config = (TConfig)config;
        }

        /// <inheritdoc />
        void IPlugin.Start()
        {
        }

        /// <inheritdoc />
        void IPlugin.Stop()
        {
        }

        /// <inheritdoc />
        public abstract Task<ResourceInitializerResult> Execute(IResourceGraph graph, object parameters);

        /// <summary>
        /// Creates an <see cref="ResourceInitializerResult"/> within a completed task
        /// </summary>
        /// <param name="initializedResources">Initialized resources, only roots should be returned as resources are saved recursively</param>
        protected static Task<ResourceInitializerResult> InitializedAsync(IReadOnlyList<Resource> initializedResources) =>
            InitializedAsync(initializedResources, false);

        /// <summary>
        /// Creates an <see cref="ResourceInitializerResult"/> within a completed task
        /// </summary>
        /// <param name="initializedResources">Initialized resources, only roots should be returned as resources are saved recursively</param>
        /// <param name="saved">If true, the resources will not be saved, but are assumed to already have been saved within the initializer</param>
        protected static Task<ResourceInitializerResult> InitializedAsync(IReadOnlyList<Resource> initializedResources, bool saved) =>
            Task.FromResult(Initialized(initializedResources, saved));

        /// <summary>
        /// Creates an <see cref="ResourceInitializerResult"/>
        /// </summary>
        /// <param name="initializedResources">Initialized resources, only roots should be returned as resources are saved recursively</param>
        protected static ResourceInitializerResult Initialized(IReadOnlyList<Resource> initializedResources) =>
            Initialized(initializedResources, false);

        /// <summary>
        /// Creates an <see cref="ResourceInitializerResult"/>
        /// </summary>
        /// <param name="initializedResources">Initialized resources, only roots should be returned as resources are saved recursively</param>
        /// <param name="saved">If true, the resources will not be saved, but are assumed to already have been saved within the initializer</param>
        protected static ResourceInitializerResult Initialized(IReadOnlyList<Resource> initializedResources, bool saved)
        {
            return new ResourceInitializerResult
            {
                Saved = saved,
                InitializedResources = initializedResources
            };
        }
    }
}
