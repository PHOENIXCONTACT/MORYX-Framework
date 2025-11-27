# MORYX 8.x to 10.x

## Async life-cycle

### Server Module lifecycle refactored to async methods

The lifecycle methods of ServerModules have been migrated from void to async Task to enable modern asynchronous programming with async/await.
Since there was no native async context for lifecycle methods, asynchronous calls must be synchronized to preserve the existing start/stop behavior.
The runtime now supports asynchronous initialization, startup, and shutdown processes (e.g., loading entities from a database during startup).

**Changes in ServerModuleBase:**

- `OnInitialize()` -> `OnInitializeAsync()`
- `OnStart()` -> `OnStartAsync()`
- `OnStop()` -> `OnStopAsync()`

**Changes in IModuleManager:**

- `StartModules()` -> `StartModulesAsync()`
- `StopModules()` -> `StopModulesAsync()`

The `Main` method in `Program.cs` must be updated to a Task-based signature.

````cs
public static async Task Main(string[] args)
{
    AppDomainBuilder.LoadAssemblies();

    [...]

    var moduleManager = host.Services.GetRequiredService<IModuleManager>();
    await moduleManager.StartModulesAsync();

    await host.RunAsync();

    await moduleManager.StopModulesAsync();
}
````

### Changes in StateBase

We now provide a full async implementation of the `StateBase`. To keep constistent naming, the `StateBase` was splitted to `SyncStateBase` and `AsyncStateBase`.
Due to the reduction of unneccessary interfaces, `IState` was removed and `StateBase` will be used in e.g. `IStateContext` now.

The `StateMachine` class was extended by `WithAsync()` and `ForceAsync()`. The `AsyncStateBase` provides async all the way: `NextStateAsync()`, `OnEnterAsync()`, `OnExitAsync`.\
**Upgrade hint:** Replace `StateBase<TContext>` by `SyncStateBase<TContext>`.

The same convention was applied to `DriverState`. It was renamed to `SyncDriverState` and a new implementation `AsyncDriverState` was added with full async support.\
**Upgrade hint:** Replace `DriverState<TContext>` by `SyncDriverState<TContext>`.

## Replaced result of visual instructions with dedicated result object

In *Moryx.Factory* **6.3** and **8.1** we introduced the new result object and optional extended APIs. The result object solved issues caused by localization of the different results. With **Moryx 10** we remove all old APIs based on strings.

## Replaced `IVisualInstructions` with `VisualInstructionParameters`

The interface was only used in `VisualInstructionParameters` which can and is being used as a base class in most cases anyway.
Hence, `IVisualInstructions` is removed in favor of a more extendable base class.

## Integration of Moryx.Simulation into Moryx.ControlSystem

To reduce the number of API packages and simplify the overall architecture, **Moryx.Simulation** has been integrated into **Moryx.ControlSystem** starting with Moryx 10. All simulation-related APIs and functionality are now part of the Moryx.ControlSystem package. This change streamlines dependency management and makes it easier to maintain and extend simulation features within the control system context.

The simulator module has also been renamed, and its namespace and package id have changed accordingly to reflect its new location within Moryx.ControlSystem.


## Renamings and Typo-Fixes

- TcpClientConfig.IpAdress -> TcpClientConfig.IpAddress
- TcpListenerConfig.IpAdress -> TcpListenerConfig.IpAddress
- ResourceRelationType.CurrentExchangablePart -> ResourceRelationType.CurrentExchangeablePart
- ResourceRelationType.PossibleExchangablePart -> ResourceRelationType.PossibleExchangeablePart
- MqttDriver.BrokerURL -> MqttDriver.BrokerUrl
- IResourceManagement.GetAllResources -> IResourceManagement.GetResourcesUnsafe
- IResourceManagement.Create -> IResourceManagement.CreateUnsafe
- IResourceManagement.Read -> IResourceManagement.ReadUnsafe
- IResourceManagement.Modify -> IResourceManagement.ModifyUnsafe
- ProcessContext -> ProcessWorkplanContext
- OperationClassification -> OperationStateClassification
- OperationClassification.Loading -> OperationStateClassification.Assigning
- IAsyncInitializable.Initialize -> IAsyncInitializable.InitializeAsync
- IAsyncPlugin.Start -> IAsyncPlugin.StartAsync
- IAsyncPlugin.Stop -> IAsyncPlugin.StopAsync
- DriverState -> SyncDriverState

## Reduction of interfaces

Several interfaces have been removed to streamline the codebase and reduce complexity. The following interfaces are no longer available:

- `IProductType`: Replaced with base-class `ProductType`
- `IProductInstance`: Replaced with base-class `ProductInstance`
- `IConfig`: Replaced with base-class `ConfigBase`
- `IDatabaseConfig`: Replaced with base-class `DatabaseConfig`
- `IState`: Replace with base-class `StateBase`

## Method Signature Changes

- `PossibleValuesAttribute`: `virtual IEnumerable<string> GetValues(IContainer localContainer, IServiceProvider serviceProvider)` -> `abstract IEnumerable<string> GetValues(IContainer localContainer, IServiceProvider serviceProvider)`

## Data Model Changes

With MORYX 10, several changes have been made to the data model to improve performance and maintainability. Notable changes include:

- Naming conventions across the data model have been standardized to ensure consistency and clarity. We use pluralised names for DbSet properties and singular names for entity classes. Sample: `public DbSet<ProductEntity> Products { get; set; }` instead of `public DbSet<ProductEntity> ProductEntities { get; set; }`.
- Derived data models are now fully supported. This needs some changes how the model is defined in code first scenarios. Please refer to the [Data Model Tutorial](/docs/tutorials/data-model/CodeFirst.md) for more information.
- Removed support of Dump and Restore operations in the data model. These operations were rarely used and added unnecessary complexity to the data model management. Use your database admin tools to perform backup and restore operations instead.

### Removal of ProductFile

- The `ProductFileEntity` has been removed from the data model as it was not utilized effectively. This change helps streamline the data structure and reduce unnecessary complexity.
- If you were using `ProductFileEntity`, consider using alternative storage solutions such as file systems or dedicated file storage services to manage product-related files.
- The `ProductFile` was removed completely.

## Launcher

- The `SortIndex` configuration was moved to the `Moryx.Launcher.LauncherConfig.json` configuration file. Refer to the [Launcher](/docs/articles/launcher/Launcher.md) documentation for more information.

## Removal of Modules-Analytics

The analytics module was doing nothing and the web module was replaced by supporting external modules in `Launcher`. Its now supported to embed external web-pages into the shell. Refer to the [Launcher](/docs/articles/launcher/Launcher.md) documentation for more information.

## Rarely used features removed

These feature were infrequently used and has been removed to simplify the codebase.

- ExceptionPrinter: Used to print exceptions to different outputs. Use Exception.ToString() instead.
- CrashHandler: Used to handle application crashes. This feature was used were the runtime was a console application.
- Caller: Used to get information about the calling method. Use System.Diagnostics.StackTrace instead.
- Moryx.Endpoints: This namespace contained all base classes for the time when an endpoint was hosted inside a module. Since we use controllers it is deprecated.
- Moryx.Identity: This namespace contained base classes and services for the old WPF client to provide the authorization context. Since MORYX support web uis, this is deprecated
- PortConfig: Used for old wcf services. Deprecated since ASP.NET Core.
- ProxyConfig.Port: Use the full address instead. It contains also http/https, domain and port

## Moved classes and namespaces

- Moryx.Tools.FunctionResult: Moved to Moryx.Tools
- Moryx.AbstractionLayer namespace: All classes have been moved to more specific domain namespaces e.g. Moryx.AbstractionLayer.Resources, Moryx.AbstractionLayer.Products, Moryx.AbstractionLayer.Processes etc.

## Modules-Orders

- Removed report from interrupt of an operation. Reporting during an interruption doesn't add any value. The quantity for the report can only be predicted and will be inaccurate if something goes wrong or is reworked during the interruption.

## Reworked driver APIs

All driver APIs have been reworked to use TPL async/await instead of callbacks for the following reasons:

- The same logic looks almost synchronous.
- Cleaner and integrates seamlessly with .NET’s exception system.
- You can chain tasks with LINQ-like methods or await syntax.
- Async stack-traces in IDE show the actual logical call flow — even across await boundaries.
- The APIs for `IMessageDriver` and `IInOutDriver` with their generics and different variants was too complicated and all known usages simply used objects and root members instead different argument types. So we simplified the APIs, which also improves exhangeability of different drivers and simplifies Simulator implementations. To adjust your usages, simply remove all generic arguments.
- `IRfidDriver`, `IScannerDriver`, `IPickByLightDriver` and `IWeightScaleDriver` were extended with commonly used methods, extendable by options and result objects
- Added generic `ISingleInput{TOptions, TResult}` and `IContinuousInput{TOptions, TResult}` for general pattern of input devices


## Resource initialization

The API of `IResourceInitializer` was adjusted

- `Initialize` is now returning async task
- Introduced `ResourceInitializerResult` object for extensibility and option to save
- Its now possible to execute initializers from the facade
- The initializers are registered transient by default.

## Product importer

- Introduced `ProductImporterAttribute` for harmonized registration of importers.
- The importers are registered transient by default.

## ConstraintContext during activity-handling

The `IConstraintContext` interface was removed from `IProcess`. Instead a new wrapper was introduced `ActivityConstraintContext` which provides the Activity and the Process for better handling in `IConstraint` implementations.
