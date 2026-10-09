// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Samples.Strategies;
using Moryx.Serialization;

namespace Moryx.Resources.Samples;

/// <summary>
/// Resource that delegates part of its behavior to an exchangeable strategy.
/// Strategies from this assembly would be registered by the <see cref="DependencyRegistrationAttribute"/> itself,
/// the strategies and the factory from other assemblies are registered by the <see cref="WhiteSpaceRemovingStrategyInitializer"/>.
/// </summary>
[ResourceRegistration]
[DependencyRegistration(typeof(IWhiteSpaceRemovingStrategy), Initializer = typeof(WhiteSpaceRemovingStrategyInitializer))]
public class StrategyUsingResource : Resource
{
    private IWhiteSpaceRemovingStrategy _strategy;
    private string _strategyName;

    /// <summary>
    /// Factory to create the selected strategy, injected by the container of the resource management
    /// </summary>
    public IWhiteSpaceRemovingStrategyFactory StrategyFactory { get; set; }

    /// <summary>
    /// Name of the selected strategy. The possible values are the names of all registered strategies.
    /// </summary>
    [PluginNameSelector(typeof(IWhiteSpaceRemovingStrategy))]
    [DataMember, EntrySerialize]
    public string Strategy { get; set; }

    /// <summary>
    /// Removes all white spaces from the given text using the selected strategy
    /// </summary>
    [EntrySerialize]
    public string DropSpaces(string text) => GetStrategy().DropSpaces(text);

    private IWhiteSpaceRemovingStrategy GetStrategy()
    {
        // The selection can be changed at runtime, so resolve again if necessary
        if (_strategy != null && _strategyName == Strategy)
            return _strategy;

        ReleaseStrategy();

        if (string.IsNullOrEmpty(Strategy))
            throw new InvalidOperationException($"No strategy selected for resource {Name}.");

        _strategy = StrategyFactory.Create(Strategy);
        _strategyName = Strategy;
        return _strategy;
    }

    private void ReleaseStrategy()
    {
        if (_strategy == null)
            return;

        StrategyFactory.Destroy(_strategy);
        _strategy = null;
        _strategyName = null;
    }

    /// <inheritdoc />
    protected override void OnDispose()
    {
        ReleaseStrategy();
        base.OnDispose();
    }
}

/// <summary>
/// Registers all public <see cref="IWhiteSpaceRemovingStrategy"/> implementations of the application
/// and the <see cref="IWhiteSpaceRemovingStrategyFactory"/>, which are defined in other assemblies.
/// </summary>
public class WhiteSpaceRemovingStrategyInitializer : ISubInitializer
{
    /// <inheritdoc />
    public void Initialize(IContainer services)
    {
        services.LoadComponents<IWhiteSpaceRemovingStrategy>();
        services.Register<IWhiteSpaceRemovingStrategyFactory>();
    }
}
