// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Products.Management
{
    public class StrategyBase<TConfig, TConfigBase> : IAsyncConfiguredInitializable<TConfigBase>
        where TConfigBase : IProductStrategyConfiguration, IPluginConfig
        where TConfig : TConfigBase
    {
        /// <summary>
        /// Target type handled by this strategy
        /// </summary>
        public Type TargetType { get; protected set; }

        /// <summary>
        /// Configuration of this strategy
        /// </summary>
        protected TConfig Config { get; private set; }

        /// <summary>
        /// Initialize the strategy
        /// </summary>
        public virtual Task InitializeAsync(TConfigBase config, CancellationToken cancellationToken = default)
        {
            Config = (TConfig)config;
            return Task.CompletedTask;
        }
    }
}
