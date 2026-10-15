---
uid: HowToUseStrategies
---
# How to use strategies

>[!NOTE] The [strategy pattern](https://en.wikipedia.org/wiki/Strategy_pattern) has a dedicated place in the MORYX Framework and is directly supported by its [dependency injection](/docs/articles/framework/dependency-injection.md).

This tutorial shows how a server module delegates part of its behavior to an exchangeable strategy, which is selected in the module configuration. It covers where the strategies can come from, how they are created on demand with a factory and what to consider when using strategies in plugins.
This tutorial uses [name-based strategy selection](/docs/articles/framework/dependency-injection.md#name-based-component-selector), you can apply the same general structure for configured strategies.
Check out [configuration-based strategy selection](/docs/articles/framework/dependency-injection.md#config-based-component-selector) for more information.

## Define the strategy

The strategy interface defines the exchangeable behavior:

```cs
public interface IWhiteSpaceRemovingStrategy
{
    string DropSpaces(string s);
}
```

If strategies shall be implemented in other projects, place the interface in an assembly these projects can reference, i.e. not your module itself.

Each strategy is a plugin with a unique name. The name is what is stored in the configuration and shown in the UI:

```cs
[Plugin(LifeCycle.Singleton, typeof(IWhiteSpaceRemovingStrategy), Name = StrategyName)]
public class RegexWhiteSpaceRemovingStrategy : IWhiteSpaceRemovingStrategy
{
    public const string StrategyName = "Regex Remover";

    public string DropSpaces(string s) => Regex.Replace(s, @"\s+", "");
}

[Plugin(LifeCycle.Singleton, typeof(IWhiteSpaceRemovingStrategy), Name = StrategyName)]
public class LinqWhiteSpaceRemovingStrategy : IWhiteSpaceRemovingStrategy
{
    public const string StrategyName = "Linq Remover";

    public string DropSpaces(string s) => string.Concat(s.Where(c => !char.IsWhiteSpace(c)));
}
```

Use `LifeCycle.Singleton` for stateless strategies, all users share the same instance. Use `LifeCycle.Transient` if a strategy holds state or more than one instance is necessary, e.g. when using configured strategies with different configurations.

## Select the strategy in the configuration

The selected strategy is a property of the module configuration:

```cs
[DataMember]
[ModuleStrategy(typeof(IWhiteSpaceRemovingStrategy))]
[PluginNameSelector(typeof(IWhiteSpaceRemovingStrategy))]
[DefaultValue(RegexWhiteSpaceRemovingStrategy.StrategyName)]
public string WhiteSpaceRemovingStrategy { get; set; }
```

- `ModuleStrategyAttribute` tells the container to inject the strategy with the configured name wherever `IWhiteSpaceRemovingStrategy` is requested in the module.
- `PluginNameSelectorAttribute` offers the names of all strategies registered in the container of the module as possible values, see [Possible Values](/docs/articles/framework/Serialization/possible-values.md#pluginnameselectorattribute).
- `DefaultValue` makes sure a strategy is selected. If the name is empty or unknown, nothing is injected and the property stays `null`.

## Use the strategy

Components and plugins of the module simply depend on the strategy interface:

```cs
[Component(LifeCycle.Singleton, typeof(ITextProcessor))]
internal class TextProcessor : ITextProcessor
{
    public IWhiteSpaceRemovingStrategy WhiteSpaceRemoving { get; set; }

    public string Process(string text) => WhiteSpaceRemoving.DropSpaces(text);
}
```

The configuration is evaluated when the module is initialized. Changing the selection therefore requires a restart of the module.

Strategies that need their own configuration use an `IPluginConfig` instead of a `string` property. The container selects the strategy by its `PluginName`, the dependent component passes the configuration on, e.g. in its `Start` method:

```cs
[DataMember]
[ModuleStrategy(typeof(IJobScheduler)), PluginConfigs(typeof(IJobScheduler), false)]
public JobSchedulerConfig JobSchedulerConfig { get; set; }

// In the component that uses the strategy
JobScheduler.Initialize(Config.JobSchedulerConfig);
```

## Register the strategies

Every module has its own container. A strategy that is not registered there can neither be selected in the UI nor injected. Depending on where the strategies are defined, they are registered differently.

### Strategies in the module assembly

All types of the module assembly with a registration attribute are registered automatically. Nothing else is necessary, the strategies can even be `internal`.

### Strategies in another assembly

Strategies from other assemblies, e.g. project-specific strategies for a generic module, are registered by the `ModuleController`:

```cs
protected override Task OnInitializeAsync(CancellationToken cancellationToken)
{
    Container.LoadComponents<IWhiteSpaceRemovingStrategy>();
    return Task.CompletedTask;
}
```

`LoadComponents` registers all implementations of the interface in the application. For a strategy from another assembly to be found:

- the strategy class is `public`,
- the assembly is deployed to the application directory, i.e. the project is referenced by the application,
- the assembly is loaded into the application domain. Call `AppDomainBuilder.LoadAssemblies()` at the beginning of your `Program.cs`, as in [StartProject.Asp](/src/StartProject.Asp/Program.cs).

### Strategies brought by a plugin from another assembly

If the strategy is used by a plugin that is defined outside of the module assembly and loaded with `LoadComponents`, the plugin can register its strategies itself with the `DependencyRegistrationAttribute`. The module does not need to know about them.

If the strategies are defined in the **same assembly as the plugin**, listing the strategy interface is enough:

```cs
[Plugin(LifeCycle.Singleton, typeof(ITextProcessor), Name = PluginName)]
[DependencyRegistration(typeof(IWhiteSpaceRemovingStrategy))]
public class TextProcessor : ITextProcessor
{
    public const string PluginName = nameof(TextProcessor);
}
```

If the strategies are defined in **yet another assembly**, the plugin adds an `ISubInitializer`, which is executed after `OnInitializeAsync` of the module:

```cs
public class WhiteSpaceRemovingStrategyInitializer : ISubInitializer
{
    public void Initialize(IContainer container)
    {
        container.LoadComponents<IWhiteSpaceRemovingStrategy>();
    }
}

[Plugin(LifeCycle.Singleton, typeof(ITextProcessor), Name = PluginName)]
[DependencyRegistration(typeof(IWhiteSpaceRemovingStrategy), Initializer = typeof(WhiteSpaceRemovingStrategyInitializer))]
public class TextProcessor : ITextProcessor
{
    public const string PluginName = nameof(TextProcessor);
}
```

The same conditions as for [strategies in another assembly](#strategies-in-another-assembly) apply to the strategies loaded by the initializer.

>[!NOTE]
> The `DependencyRegistrationAttribute` is only evaluated for types registered with `LoadComponents`. It has no effect on types of the module assembly, so the `Initializer` of the attribute is ignored there as well.

## Create strategies with a factory

`ModuleStrategyAttribute` provides one strategy per module, fixed until the next restart. Use a factory instead if

- several instances with different selections are needed, e.g. one per configured plugin, or
- the selection can change at runtime.  
*Both cases are prominent in [resources](#an-example-using-resources), for example.*

The factory turns the name into an instance, its implementation is generated by the container:

```cs
[PluginFactory(typeof(INameBasedComponentSelector))]
public interface IWhiteSpaceRemovingStrategyFactory
{
    IWhiteSpaceRemovingStrategy Create(string name);

    void Destroy(IWhiteSpaceRemovingStrategy instance);
}
```

The factory is registered like the strategies: automatically in the module assembly, with `Container.Register<IWhiteSpaceRemovingStrategyFactory>()` otherwise, or by listing it in the `DependencyRegistrationAttribute`. The selection property only needs the `PluginNameSelectorAttribute`:

```cs
public IWhiteSpaceRemovingStrategyFactory StrategyFactory { get; set; }

private IWhiteSpaceRemovingStrategy _strategy;

public void Start()
{
    _strategy = StrategyFactory.Create(Config.WhiteSpaceRemovingStrategy);
}

public void Stop()
{
    if (_strategy != null)
        StrategyFactory.Destroy(_strategy);
    _strategy = null;
}
```

Keep the following behavior of the factory in mind:

- An unknown name throws a `ComponentNotFoundException`.
- A `null` name does **not** fail, it returns the first registered strategy.
- If you declare the factory interface `internal`, the assembly needs `[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]`.

## An example using resources

Resources are a typical example of [strategies brought by a plugin from another assembly](#strategies-brought-by-a-plugin-from-another-assembly): The *Resource Management Module* registers all resources with `LoadComponents<IResource>()` and evaluates their `DependencyRegistrationAttribute`. Compared to other modules, there are three special features here:

- The selection is a persisted property of the resource, not part of the module configuration. `ModuleStrategyAttribute` does not apply, so the resource always uses a factory.
- The selection can be changed in the resource UI while the resource is running, so the resource resolves the strategy again after the selection changed.
- The `ResourceRegistrationAttribute` is necessary to specifically enable the resource for dependency injection

```cs
[ResourceRegistration]
[DependencyRegistration(typeof(IWhiteSpaceRemovingStrategy), typeof(IWhiteSpaceRemovingStrategyFactory))]
public class StrategyUsingResource : Resource
{
    private IWhiteSpaceRemovingStrategy _strategy;
    private string _strategyName;

    public IWhiteSpaceRemovingStrategyFactory StrategyFactory { get; set; }

    [PluginNameSelector(typeof(IWhiteSpaceRemovingStrategy))]
    [DataMember, EntrySerialize]
    public string Strategy { get; set; }

    [EntrySerialize]
    public string DropSpaces(string text) => GetStrategy().DropSpaces(text);

    private IWhiteSpaceRemovingStrategy GetStrategy()
    {
        if (_strategy != null && _strategyName == Strategy)
            return _strategy;

        ReleaseStrategy();

        // The factory would return the first strategy for an empty name
        if (string.IsNullOrEmpty(Strategy))
            throw new InvalidOperationException($"No strategy selected for resource {Name}.");

        _strategy = StrategyFactory.Create(Strategy);
        _strategyName = Strategy;
        return _strategy;
    }

    private void ReleaseStrategy()
    {
        if (_strategy != null)
            StrategyFactory.Destroy(_strategy);
        _strategy = null;
        _strategyName = null;
    }

    protected override void OnDispose()
    {
        ReleaseStrategy();
        base.OnDispose();
    }
}
```

If strategies and factory are defined in another assembly than the resource, register both in an `ISubInitializer` as shown above. The initializer is executed by the *ResourceManagement* before any resource is loaded.

## Troubleshooting

| Symptom | Cause |
|---------|-------|
| The drop-down is empty or a strategy is missing | The strategy is not registered in the container of the module, see [Register the strategies](#register-the-strategies). |
| A strategy from another assembly is missing | The class is `internal`, the DLL is not in the application directory, or `AppDomainBuilder.LoadAssemblies()` is not called. |
| The injected strategy is `null` | The configured name is empty or does not match a registered strategy, e.g. after a strategy was renamed or its assembly removed. |
| `ComponentNotFoundException` when creating the strategy | The name passed to the factory does not match a registered strategy. |
| The factory creates the wrong strategy | The name is empty, so the first registered strategy is returned. |
| Two strategies, but only one is listed | Both use the same `Name`. Only the first registration is kept. |
| `DependencyRegistration` has no effect | The type is part of the module assembly and therefore not registered with `LoadComponents`. |
| `StrategyFactory` of a resource is `null` | The factory is not listed in `DependencyRegistration` or not registered by the `Initializer`, or the resource is missing the `ResourceRegistrationAttribute`. |
