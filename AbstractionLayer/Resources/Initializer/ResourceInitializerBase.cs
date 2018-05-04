using System;
using System.Collections.Generic;
using Marvin.Logging;
using Marvin.Modules.ModulePlugins;

namespace Marvin.AbstractionLayer.Resources
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
        void IModulePlugin.Start()
        {
        }

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
        }
        
        /// <inheritdoc />
        public abstract IReadOnlyList<Resource> Execute(IResourceCreator creator);
    }
}