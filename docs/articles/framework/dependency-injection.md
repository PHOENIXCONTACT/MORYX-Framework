---
uid: DependencyInjection
---
# Dependency Injection

Every server module in MORYX has its own dependency injection container. Components and plugins of a module are registered in this container and get their dependencies injected from it. Modules do not share containers, they only communicate through their [facades](/docs/tutorials/server-module/facades.md).

The container is based on [Castle Windsor](https://github.com/castleproject/Windsor) and encapsulated in [`CastleContainer`](/src/Moryx.Container/CastleContainer.cs). Modules, components and plugins only depend on the interface [`IContainer`](/src/Moryx/Container/IContainer.cs) and its extension methods, so they never reference Castle Windsor directly.

> [!TIP]
> For a step-by-step guide to set up the container of a module, see [How to create a server module](/docs/tutorials/server-module/server-module.md).

> [!TIP]
> For a step-by-step guide on exchangeable behavior in a module's components and plugins as well as helpful troubleshooting advice, see [How to use strategies](/docs/tutorials/how-to-use-strategies.md).

## How it works

### The container lifecycle of a module

The container of a module is created and destroyed by [`ServerModuleBase`](/src/Moryx.Runtime/Modules/ServerModuleBase.cs) together with the module:

1. **Parse the configuration**: The module configuration is loaded and searched for properties with the `ModuleStrategyAttribute`, see [Module strategies](#module-strategies).
2. **Create the container**: The [`ModuleContainerFactory`](/src/Moryx.Runtime.Kernel/Container/ModuleContainerFactory.cs) creates a new `CastleContainer` and registers **all** types of the module assembly that carry a `ComponentAttribute` (or a derived attribute like `PluginAttribute`) or a `PluginFactoryAttribute`.  
*Note: This includes `internal` types, which allows you to keep your module interior `internal`.*
3. **Register the module instances**: The module configuration, the `ILoggerFactory` and the module logger are registered as instances.
4. **`OnInitializeAsync`**: The module registers everything that is not part of its own assembly, e.g. plugins from other assemblies or facades of other modules.
5. **Execute the sub initializers**: All `ISubInitializer` registered in the container are resolved and executed, see [Register dependencies of a component](#register-dependencies-of-a-component).
6. **Start, Stop**: Components are resolved from the container as needed.
7. **Destroy**: When the module is stopped, the container is destroyed, together with all instances it created. The next start creates a new container.  
*Note: The facade of your own module lives outside of the container's lifecycle, it is not going to be recreated.*

### Resolving dependencies

Dependencies are injected via constructor parameters or public properties with a setter. Constructor parameters are mandatory, properties are optional and stay `null` if the dependency cannot be resolved.

For every dependency, the container first looks at its own registrations. In addition, the [`MoryxFacility`](/src/Moryx.Container/MoryxFacility.cs) adds the following sub resolvers, which are consulted in this order:

| Sub resolver | Triggered by | Resolves |
|---|---|---|
| `CollectionResolver` | Array or collection of a service | All registered implementations of the service |
| [`NamedDependencyResolver`](/src/Moryx.Container/NamedDependencyResolver.cs) | `NamedAttribute` on the property or parameter | The component registered with this name |
| [`StrategySubResolver`](/src/Moryx.Container/StrategySubResolver.cs) | Service type configured with the `ModuleStrategyAttribute` | The component with the configured name |
| [`ChildContainerSubResolver`](/src/Moryx.Container/ChildContainerSubResolver.cs) | `UseChildAttribute` on the property or parameter | A child of a registered `INamedChildContainer`, e.g. a named child logger |
| [`ServiceProviderSubResolver`](/src/Moryx.Container/ServiceProviderSubResolver.cs) | Service is not registered in the container | The service from the host `IServiceProvider`, see [ServiceProvider Bridge](#serviceprovider-bridge) |

```cs
[Plugin(LifeCycle.Singleton, typeof(IMyPlugin), Name = PluginName)]
public class MyPlugin : IMyPlugin
{
    public const string PluginName = nameof(MyPlugin);

    // Child logger with the name of this plugin
    [UseChild(PluginName)]
    public IModuleLogger Logger { get; set; }

    // Specific implementation of IMyComponent
    [Named(MyComponentB.ComponentName)]
    public IMyComponent Component { get; set; }

    // All implementations of IMyHook
    public IMyHook[] Hooks { get; set; }
}
```

Further sample implementations for plugins can be found in the [TestModule](/src/Moryx.TestModule/Plugin/).

### Manual resolution

Components should get their dependencies injected instead of resolving them. Manual resolution is meant for the composition root, i.e. the `ModuleController`, which resolves the entry components of the module on start and stop:

```cs
// First registered implementation, or the one configured with ModuleStrategyAttribute
var plugin = Container.Resolve<IMyPlugin>();

// Implementation registered with this name
var pluginB = Container.Resolve<IMyPlugin>(MyPluginB.PluginName);

// All registered implementations
IMyHook[] hooks = Container.ResolveAll<IMyHook>();
```

Keep the following behavior in mind:

- `Resolve` returns `null` if the service is not registered.
- An unknown name does **not** fail, `Resolve` falls back to the first registered implementation of the service.
- Sub resolvers only apply to the dependencies of the resolved component, not to the requested service itself. A service of the host is therefore not resolved through the [ServiceProvider Bridge](#serviceprovider-bridge).

[How to create a server module](/docs/tutorials/server-module/server-module.md#start-the-module) shows the typical usage.

## Registration

### Registration attributes

Types are only registered if they are marked with one of the registration attributes. A type without them is neither registered automatically nor by `LoadComponents`.

| Attribute | Purpose |
|---|---|
| `ComponentAttribute` | Marks a fixed part of the module structure. Defines the `LifeCycle`, the services and an optional `Name`. |
| `PluginAttribute` | Same as `ComponentAttribute`, but marks an exchangeable part, e.g. a strategy. Plugins with a `Name` can be selected in the configuration. |
| `PluginFactoryAttribute` | Marks an interface as factory. Its implementation is generated by the container, see [Factories](#factories). |
| `DependencyRegistrationAttribute` | Registers additional types when the annotated type is loaded with `LoadComponents`, see [Register dependencies of a component](#register-dependencies-of-a-component). |
| `ComponentLoaderIgnoreAttribute` | Assembly attribute that excludes all types of the assembly from `LoadComponents`. |

```cs
[Component(LifeCycle.Singleton, typeof(IMyComponent))]
internal class MyComponent : IMyComponent
{
}

[Plugin(LifeCycle.Transient, typeof(IMyPlugin), Name = PluginName)]
public class MyPlugin : IMyPlugin
{
    public const string PluginName = nameof(MyPlugin);
}
```

- `LifeCycle.Singleton`: All dependents share one instance per container.
- `LifeCycle.Transient`: Every resolution creates a new instance. Release instances created by a factory with its `Destroy` method.

> [!CAUTION]
> Names must be unique within the container. If two types are registered with the same name, only the first one is kept. Types without a name are registered with their full type name.

### Registration methods

Types of the module assembly are registered automatically. Everything else is registered in `OnInitializeAsync` of the `ModuleController` with the following methods:

```cs
protected override Task OnInitializeAsync(CancellationToken cancellationToken)
{
    // All public implementations of IMyPlugin from all loaded assemblies
    Container.LoadComponents<IMyPlugin>();

    // Only implementations that match a condition
    Container.LoadComponents<IMyPlugin>(type => type.GetCustomAttribute<MyAttribute>() != null);

    // A single type or a factory interface
    Container.Register<IMyService, MyService>();
    Container.Register<IMyPluginFactory>();

    // An existing instance, optionally with a name
    Container.SetInstance(ResourceManagement)
        .SetInstance<IMyService>(myService, "MyService");

    return Task.CompletedTask;
}
```

`LoadComponents<T>` searches the types known to the [`ReflectionTool`](/src/Moryx/Tools/ReflectionTool.cs) for public classes that implement `T` and carry a `ComponentAttribute`. A type is only found if:

- the class is `public`,
- its assembly is deployed to the application directory, i.e. the project is referenced by the application,
- its assembly is loaded into the application domain. Call `AppDomainBuilder.LoadAssemblies()` at the beginning of your `Program.cs`, as in [StartProject.Asp](/src/StartProject.Asp/Program.cs).

Types that are already registered are skipped, so it is safe to load the same service more than once.

### Register dependencies of a component

A component from another assembly often needs its own plugins. Instead of letting every module register them, the component declares them with the `DependencyRegistrationAttribute`. The attribute is evaluated when the component is registered with `LoadComponents` (see [`ContainerLoadComponentsExtension`](/src/Moryx/Container/ContainerLoadComponentsExtension.cs)):

- All types from the **assembly of the annotated component** that export one of the listed services are registered. With `InstallerMode.All`, all types of the assembly are registered.
- If an `Initializer` is set, it is registered as `ISubInitializer`. The module executes it after `OnInitializeAsync`. Use it to register types from other assemblies than the component (which itself is already not part of the module's assembly).

```cs
[Plugin(LifeCycle.Singleton, typeof(IMyPlugin), Name = PluginName)]
[DependencyRegistration(typeof(IMyHook), Initializer = typeof(MyHookInitializer))]
public class MyPlugin : IMyPlugin
{
    public const string PluginName = nameof(MyPlugin);

    public IMyHook[] Hooks { get; set; }
}

public class MyHookInitializer : ISubInitializer
{
    public void Initialize(IContainer container)
    {
        container.LoadComponents<IMyHook>();
    }
}
```

> [!NOTE]
> The attribute has no effect on types of the module assembly, because they are registered without `LoadComponents`.

## Module strategies

The `ModuleStrategyAttribute` binds a service type to the name of an implementation, which is selected in the module configuration. The property can either be a `string` with the name or an `IPluginConfig`, in which case its `PluginName` is used:

```cs
[DataMember]
[ModuleStrategy(typeof(IMyStrategy)), PluginNameSelector(typeof(IMyStrategy))]
public string MyStrategy { get; set; }
```

Before the container is created, the module configuration is parsed for these attributes, including nested configurations and collections (see [`ConfigParser`](/src/Moryx.Runtime/Modules/ConfigParser.cs)). Whenever a component of the module depends on `IMyStrategy`, the container injects the implementation with the configured name. As the configuration is only parsed during initialization, a different strategy is used after the module is restarted.

## Factories

An interface marked with the `PluginFactoryAttribute` is implemented by the container using the [typed factory facility](https://github.com/castleproject/Windsor/blob/master/docs/typed-factory-facility-interface-based.md) of Castle Windsor. A `Create` method resolves a component, a `Destroy` method releases it. The component selector of the attribute decides which implementation is created.

### Name based component selector

The `INameBasedComponentSelector` takes the name of a plugin, like [the once sketched out previously](#registration-attributes), and creates the instances accordingly using:

```cs
[PluginFactory(typeof(INameBasedComponentSelector))]
public interface IMyPluginFactory
{
    IMyPlugin Create(string name);

    void Destroy(IMyPlugin instance);
}
```

- An unknown name throws a `ComponentNotFoundException`.
- A `null` or empty name throws an `ArgumentException`.

### Config based component selector

The `IConfigBasedComponentSelector` creates the implementation named by `IPluginConfig.PluginName` and initializes it with the configuration. The plugins implement `IConfiguredPlugin<TConfig>`:

```cs
[DataContract]
public class MyPluginConfig : IPluginConfig
{
    [DataMember]
    public virtual string PluginName { get; set; }
}

[Plugin(LifeCycle.Transient, typeof(IMyPlugin), Name = nameof(MyPluginA))]
public class MyPluginA : IMyPlugin, IConfiguredPlugin<MyPluginConfig>
{
    public void Initialize(MyPluginConfig config) { }
}

[Plugin(LifeCycle.Transient, typeof(IMyPlugin), Name = nameof(MyPluginB))]
public class MyPluginB : IMyPlugin, IConfiguredPlugin<MyPluginConfig>
{
    public void Initialize(MyPluginConfig config) { }
}

[PluginFactory(typeof(IConfigBasedComponentSelector))]
public interface IMyPluginFactory
{
    IMyPlugin Create(MyPluginConfig config);

    void Destroy(IMyPlugin instance);
}
```

A sample implementation of configured plugins can be found in the [TestModule](/src/Moryx.TestModule/Plugin/TestPlugin.cs), which utilizes the `ExpectedConfigAttribute`.

If the component implements `IAsyncConfiguredInitializable`, its `InitializeAsync` method is called during selection. The signature of the `Create` method decides how it is called:

```cs
public interface ISample : IAsyncConfiguredInitializable<SampleConfig>;

[PluginFactory(typeof(IConfigBasedComponentSelector))]
internal interface ISampleFactory
{
    // InitializeAsync will be called synchronously after creation using CancellationToken.None
    ISample Create(SampleConfig config);

    // InitializeAsync will be called synchronously after creation with passing cancellationToken
    ISample Create(SampleConfig config, CancellationToken cancellationToken);

    // InitializeAsync will be called asynchronously after creation using CancellationToken.None
    Task<ISample> Create(SampleConfig config);

    // InitializeAsync will be called asynchronously after creation with passing cancellationToken
    Task<ISample> Create(SampleConfig config, CancellationToken cancellationToken);
}
```

Use the name based selector if the implementations do not need a configuration. A configuration class that only holds the `PluginName` adds no value.

If you declare the factory interface `internal`, the assembly needs `[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]`.

## ServiceProvider Bridge

> See also [ADR-006: ServiceProvider Bridge for Module Containers](/docs/adr/006-serviceprovider-bridge.md)

By default, services registered in the host `IServiceProvider` (e.g. `IHttpClientFactory`, `TimeProvider`) are not available inside module containers. The ServiceProvider Bridge solves this by automatically forwarding unresolved dependencies from the Castle Windsor container to the host `IServiceProvider`.

### How the bridge works

When a module component requests a dependency that is not registered in its Castle Windsor container, the [`ServiceProviderSubResolver`](/src/Moryx.Container/ServiceProviderSubResolver.cs) checks if the host `IServiceProvider` can provide it. If so, the service is injected transparently.

1. Module component requests IHttpClientFactory
2. Castle Windsor checks its own registrations (not found)
3. ServiceProviderSubResolver checks IServiceProvider
4. Returns IHttpClientFactory from host DI

Castle Windsor registrations always take precedence. The bridge only activates for types that are not registered in the module container.

### When to use

Explicit registration in the module container (via `SetInstance`, `LoadComponents` or `RegistrationAttribute`) must always be preferred when the dependency is known to the module. The ServiceProvider Bridge is intended for plugins or components that require infrastructure services from the host (e.g. `IHttpClientFactory`, `TimeProvider`) which are not part of the module's own domain.

### Usage

Any service registered in the host `IServiceProvider` can be injected into module components — no additional registration needed:

```cs
[Component(LifeCycle.Singleton, typeof(IMyService))]
public class MyService : IMyService
{
    public MyService(IHttpClientFactory httpClientFactory, TimeProvider timeProvider)
    {
        // Both are resolved from the host IServiceProvider automatically
        var client = httpClientFactory.CreateClient();
    }
}
```

To make a service available through the bridge, register it in the host `IServiceCollection` at startup:

```cs
builder.Services.AddHttpClient();
```

### Limitations

- If a type is registered in both Castle Windsor and the `IServiceProvider`, Castle Windsor always wins.
- The bridge works for constructor and property injection. Direct `Container.Resolve<T>()` calls are not affected and return `null` if the type is not registered — sub-resolvers are only consulted when resolving dependencies of registered components.
- The bridge does not guarantee that a service is available. It is the application developer's responsibility to register the required services in the `IServiceCollection` at startup (e.g. `builder.Services.AddHttpClient()`). There is no compile-time check — if a service is missing from the `IServiceProvider`, the injection will fail at runtime when the module component is resolved.

For excluded types and further exceptions see [ADR-006](/docs/adr/006-serviceprovider-bridge.md#exceptions).
