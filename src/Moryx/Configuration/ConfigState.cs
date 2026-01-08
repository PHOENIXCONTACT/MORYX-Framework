// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Configuration;

/// <summary>
/// Different states a <see cref="ConfigBase"/> object can have after deserialization
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