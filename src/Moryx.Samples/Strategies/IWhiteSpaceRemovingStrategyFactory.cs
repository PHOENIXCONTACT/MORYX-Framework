// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Samples.Strategies;

/// <summary>
/// Factory to create <see cref="IWhiteSpaceRemovingStrategy"/> instances by their plugin name
/// </summary>
[PluginFactory(typeof(INameBasedComponentSelector))]
public interface IWhiteSpaceRemovingStrategyFactory
{
    /// <summary>
    /// Creates the strategy with the given plugin name
    /// </summary>
    IWhiteSpaceRemovingStrategy Create(string name);

    /// <summary>
    /// Releases a strategy created by this factory
    /// </summary>
    void Destroy(IWhiteSpaceRemovingStrategy instance);
}
