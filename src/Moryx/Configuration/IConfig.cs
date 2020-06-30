// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Configuration
{
    /// <summary>
    /// Different states a <see cref="IConfig"/> object can have after deserialization
    /// by the <see cref="IConfigManager"/>.
    /// </summary>
    public enum ConfigState
    {
        /// <summary>
        /// The config file was not found and was therefor generated.
        /// </summary>
        Generated,
        /// <summary>
        /// The config file could be loaded and the state was set to valid by a maintainer.
        /// </summary>
        Valid,
        /// <summary>
        /// The config file was invalid and could not be deserialized and was replaced by a
        /// generated config object.
        /// </summary>
        Error
    }

    /// <summary>
    /// Base interface for all config objects managed by the <see cref="IConfigManager"/> 
    /// and lower bound for the <see cref="IConfigManager.GetConfiguration{T}(bool)"/> method.
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Current state of the config object. This should be decorated with the data member in order to save
        /// the valid state after finalized configuration.
        /// </summary>
        ConfigState ConfigState { get; set; }

        /// <summary>
        /// Exception message if load failed. This must not be decorated with a data member attribute.
        /// </summary>
        string LoadError { get; set; }
    }
}
