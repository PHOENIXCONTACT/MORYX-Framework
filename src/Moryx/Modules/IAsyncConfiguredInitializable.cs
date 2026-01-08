// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Modules;

/// <summary>
/// This generic interface is intended for all components that require a configuration for their initialization to work properly.
/// This configuration is passed to the plugin via the Initialize(TConf config) method.
/// </summary>
public interface IAsyncConfiguredInitializable<in T>
    where T : IPluginConfig
{
    /// <summary>
    /// Initialize this component with its config
    /// </summary>
    /// <param name="config">Config of this component</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    Task InitializeAsync(T config, CancellationToken cancellationToken = default);
}
