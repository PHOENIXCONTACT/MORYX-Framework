---
uid: GettingStarted.ServerModule
---
# How to create a server module

A server module is a self-contained unit of the application with its own [dependency injection container](/docs/articles/framework/dependency-injection.md), configuration and lifecycle. This tutorial shows how to create a module and how its `ModuleController` composes the components of the module.

Create a new project, named after the module, e.g. *Moryx.Something.Execution*, and reference *Moryx* and *Moryx.Runtime*. A module consists of at least the following files:

```fs
-Moryx.Something.Execution
|-ModuleController
| |-ModuleController.cs
| |-ModuleConfig.cs
| |-ModuleConsole.cs
```

## The ModuleController

The `ModuleController` is the composition root of the module. It derives from `ServerModuleBase<TConfig>` and is responsible to

1. import the dependencies of the module,
2. register external dependencies in the container of the module,
3. resolve and start the components of the module,
4. stop them again, and
5. export the facades of the module, see [Facades](/docs/tutorials/server-module/facades.md).

```cs
[Description("Executes something of the production")]
public class ModuleController : ServerModuleBase<ModuleConfig>, IFacadeContainer<IExecution>
{
    public const string ModuleName = "Something Execution";

    public override string Name => ModuleName;
}
```

### Import dependencies

The `ModuleController` itself is created by the host. It has two kinds of dependencies:

- **Services of the host** are injected via the constructor, e.g. the `IDbContextManager`. The base class requires the `IModuleContainerFactory`, the `IConfigManager` and the `ILoggerFactory`.
- **Facades of other modules** are injected into public properties with the `RequiredModuleApiAttribute`. The attribute also defines whether the module depends on the start of the other module, see [Importing a facade](facades.md#importing-a-facade).

```cs
[RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
public IResourceManagement ResourceManagement { get; set; }

private readonly IDbContextManager _dbContextManager;

public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory,
    IDbContextManager dbContextManager)
    : base(containerFactory, configManager, loggerFactory)
{
    _dbContextManager = dbContextManager;
}
```

### Register external dependencies

Before `OnInitializeAsync` is called, the container is created and all components, plugins and factories of the module assembly are registered automatically, as well as the module configuration and logger. Everything else is registered in `OnInitializeAsync`, so the components of the module can get it injected:

```cs
protected override Task OnInitializeAsync(CancellationToken cancellationToken)
{
    // Imported facades
    Container.SetInstance(ResourceManagement)
        .SetInstance(ProductManagement);

    // Data model of the module
    Container.ActivateDbContexts(_dbContextManager);

    // Plugins from other assemblies
    Container.LoadComponents<ISomePlugin>();

    return Task.CompletedTask;
}
```

See [Registration methods](/docs/articles/framework/dependency-injection.md#registration-methods) for all options and [How to use strategies](/docs/tutorials/how-to-use-strategies.md#register-the-strategies) for registering exchangeable plugins.

### Start the module

Components do not resolve their dependencies themselves, they get them injected. Only the `ModuleController` resolves components manually, see [Manual resolution](/docs/articles/framework/dependency-injection.md#manual-resolution).

```cs
protected override async Task OnStartAsync(CancellationToken cancellationToken)
{
    // Start Plugin
    var plugin = Container.Resolve<ISomePlugin>();
    await plugin.StartAsync();
    
    // Activate facades
    ActivateFacade(_somethingExecution);
}
```

Activate the facades after the components are started, so other modules only access the module when it is ready.

### Stop the module

`OnStopAsync` reverses the start: Deactivate the facades first, then stop the components. As the entry component is a singleton, resolving it again returns the started instance:

```cs
protected override async Task OnStopAsync(CancellationToken cancellationToken)
{
    DeactivateFacade(_executionFacade);

    
    // Stop Plugin
    var plugin = Container.Resolve<ISomePlugin>();
    await plugin.StopAsync();
}
```

After `OnStopAsync`, the container is destroyed together with all its instances. The next start of the module initializes a new container, so do not keep references to components in the `ModuleController`.

## The ModuleConfig

The `ModuleConfig` holds the configuration of the module. It is loaded before `OnInitializeAsync`, registered in the container and available in the `ModuleController` as `Config`. It can be edited in the configuration UI, changes take effect after a restart of the module.

```cs
[DataContract]
public class ModuleConfig : ConfigBase
{
    [DataMember]
    [DefaultValue(42)]
    public int InstanceCount { get; set; }

    [DataMember]
    [DefaultValue("Hello World")]
    public string SomeString { get; set; }
}
```

- The class derives from `ConfigBase` and has the `DataContractAttribute`.
- Only properties with the `DataMemberAttribute` are saved.
- The `DefaultValueAttribute` defines the value of a new configuration.

See [Configuration](/docs/articles/framework/configuration.md) for all conventions and [How to use strategies](../how-to-use-strategies.md#select-the-strategy-in-the-configuration) for selecting plugins in the configuration.

## The ModuleConsole

The module console offers methods in the UI of the module, e.g. for testing, debugging or maintenance. It implements `IServerModuleConsole` and is marked with the `ServerModuleConsoleAttribute`, which registers it in the container of the module. Methods with the `EntrySerializeAttribute` are shown in the UI. As the console is part of the container, it can get the components of the module injected.

```cs
[ServerModuleConsole]
internal class ModuleConsole : IServerModuleConsole
{
    public ISomePlugin SomePlugin { get; set; }

    [EntrySerialize]
    public void DoSomething(int input)
    {
        ...
    }
}
```

## Next steps

- [Facades](facades.md): Export and import the API of a module.
- [How to use strategies](../how-to-use-strategies.md): Make parts of the module exchangeable.
- [How to test a module](how-to-test-a-module.md): Test the module in isolation.
