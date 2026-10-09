---
uid: HowToUseStrategiesInResources
---
# How to use strategies in resources

This tutorial shows how a [Resource](../../src/Moryx.AbstractionLayer/Resources/Resource.cs) delegates part of its behavior to an exchangeable strategy, and how the strategy is selected in the resource UI using a [name based selector](../articles/framework/dependency-injection.md#name-based-component-selector). The same general structure can be applied to [configuration-based selectors](../articles/framework/dependency-injection.md#config-based-component-selector) as well.

>[!NOTE] This tutorial builds on [How to use strategies](how-to-use-strategies.md), which defines the strategy interface `IWhiteSpaceRemovingStrategy`, its implementations and the factory `IWhiteSpaceRemovingStrategyFactory`. Only the parts that differ for resources are described here.

>[!NOTE] If you have not created a resource before, start with [How to create a resource](how-to-create-resource.md).

The complete sample can be found in [StrategyUsingResource.cs](../../src/Moryx.Resources.Samples/StrategyUsingResource.cs).
It uses the strategies from [Moryx.Samples](../../src/Moryx.Samples/Strategies/), which are shared with the module sample.

## What differs for resources

| | Module | Resource |
|---|---|---|
| Container | Container of the module | Container of the *ResourceManagement* |
| Registration | Automatic for the module assembly, `LoadComponents` for other assemblies | `DependencyRegistrationAttribute` on the resource, `ISubInitializer` for other assemblies |
| Selection property | Part of the module config, changes require a restart of the module | Persisted property of the resource, can be changed while the resource is running |
| Resolution | A component or plugin of the module creates the strategy on start | The resource should create the strategy when needed and after the selection changed |
| Factory | Injected into the component or plugin | Injected into the resource |

## Register the strategies

Resources are not part of the *Moryx.Resources.Management* assembly, so their strategies are not registered automatically.
The *Resource Management* registers all resources with `Container.LoadComponents<IResource>()`.
While doing so, it evaluates the `DependencyRegistrationAttribute` of each resource and registers all components of the **resource's assembly** that export one of the listed types.
If strategies and factory are defined in the resource assembly, list the strategy interface **and** the factory:

```cs
[ResourceRegistration]
[DependencyRegistration(typeof(IWhiteSpaceRemovingStrategy), typeof(IWhiteSpaceRemovingStrategyFactory))]
public class StrategyUsingResource : Resource
{
    ...
}
```

Types from other assemblies are ignored by the attribute. As the sample strategies and factory are defined in *Moryx.Samples*, the resource registers them with an `ISubInitializer`, referenced by the `DependencyRegistrationAttribute`:

```cs
public class WhiteSpaceRemovingStrategyInitializer : ISubInitializer
{
    public void Initialize(IContainer services)
    {
        services.LoadComponents<IWhiteSpaceRemovingStrategy>();
        services.Register<IWhiteSpaceRemovingStrategyFactory>();
    }
}

[ResourceRegistration]
[DependencyRegistration(typeof(IWhiteSpaceRemovingStrategy), Initializer = typeof(WhiteSpaceRemovingStrategyInitializer))]
public class StrategyUsingResource : Resource
{
    ...
}
```

The initializer is registered together with the resource and executed by the *ResourceManagement* after its own initialization, before any resource is loaded.
The code is identical to what was in the `ModuleController` in the [module sample](how-to-use-strategies.md#register-the-strategies), so the same conditions for strategies from other assemblies apply.

## Add the selector property

The name of the selected strategy is a persisted property of the resource:

```cs
[PluginNameSelector(typeof(IWhiteSpaceRemovingStrategy))]
[DataMember, EntrySerialize]
public string Strategy { get; set; }
```

`DataMember` persists the selection, `EntrySerialize` shows the property in the resource UI.

## Resolve and use the strategy

The factory is injected into the resource as a property. The selection can be changed in the UI while the resource is running, therefore the sample resolves the strategy when it is needed and creates a new one if the name changed:

```cs
public IWhiteSpaceRemovingStrategyFactory StrategyFactory { get; set; }

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

protected override void OnDispose()
{
    ReleaseStrategy();
    base.OnDispose();
}
```

The check for an empty selection is necessary, because the factory returns the first registered strategy for a `null` name.

## Troubleshooting

In addition to the [general troubleshooting](how-to-use-strategies.md#troubleshooting):

| Symptom | Cause |
|---------|-------|
| The drop-down is empty or a strategy is missing | The strategy interface is not listed in `DependencyRegistration`, or the `Initializer` for strategies from other assemblies is missing. |
| `StrategyFactory` is `null` | The factory is not registered, either by `DependencyRegistration` or by the `Initializer`, or the resource is missing the `ResourceRegistrationAttribute`. |
