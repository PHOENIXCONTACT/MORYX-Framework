// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Configuration;

/// <summary>
/// Base interface for all config objects managed by the <see cref="IConfigManager"/>
/// </summary>
public interface IConfig : IInitializable
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
