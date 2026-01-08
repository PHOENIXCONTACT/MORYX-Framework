// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Configuration;

/// <summary>
/// Central component for all access to configuration objects. This should be implemented on every environment the
/// way it works best - file, database or remote.
/// </summary>
public interface IConfigManager
{
    /// <summary>
    /// Get configuration for a type computed at runtime
    /// </summary>
    /// <param name="confType">Config type</param>
    /// <param name="getCopy">Return currently active config or a temporary copy</param>
    /// <param name="name">Name of the config</param>
    /// <returns>Config object</returns>
    ConfigBase GetConfiguration(Type confType, string name, bool getCopy);

    /// <summary>
    /// Save configuration using its type
    /// </summary>
    /// <param name="configuration">Configuration object</param>
    /// <param name="name">Name of the configuration</param>
    /// <param name="liveUpdate">Update currently active config live</param>
    void SaveConfiguration(ConfigBase configuration, string name, bool liveUpdate);

    /// <summary>
    /// Save any shared configs included in this partial config
    /// </summary>
    void SaveSharedConfigs(object partialConfig, bool liveUpdate);
}