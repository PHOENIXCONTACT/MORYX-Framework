# MORYX 8.x to 10.x

## Async life-cycle

**Background**

Modern applications increasingly rely on asynchronous programming to improve responsiveness, scalability, and resource efficiency. Traditional synchronous lifecycle methods (`void`) in Modules and Plugins often block threads, especially during I/O-bound operations such as database access, file loading, or network calls. This can lead to performance bottlenecks and limited scalability.

To address these challenges, MORYX has been updated to support fully asynchronous lifecycle methods (`async Task`) across Modules and Plugins. This change enables:

- Non-blocking startup and shutdown processes.
- Safe asynchronous operations inside lifecycle methods, such as loading data, communicating with external services, or performing computations asynchronously.
- Improved scalability and responsiveness of MORYX, particularly in large or complex deployments.

By aligning with modern .NET asynchronous programming patterns, this migration ensures that Modules and Plugins can safely and efficiently leverage `async/await`, providing a foundation for more robust and maintainable applications.

Previously, lifecycle methods in Modules and Plugins were synchronous (`void`) and had no native support for asynchronous operations. This required all asynchronous calls to be manually synchronized to maintain consistent start/stop behavior.

With MORYX 10:

- Initialization, startup, and shutdown processes now support `async Task`.
- Modules and Plugins can perform non-blocking asynchronous operations safely during lifecycle events.
- The runtime ensures proper sequencing and state management even with asynchronous calls.

This change brings Modules and Plugins in line with modern .NET asynchronous programming patterns, improving responsiveness and scalability of the system.

### Changes in `StateBase`

We now provide a full async implementation of the `StateBase`. To keep consistent naming, the `StateBase` was split to `SyncStateBase` and `AsyncStateBase`.
Due to the reduction of unnecessary interfaces, `IState` was removed and `StateBase` will be used in e.g. `IStateContext` now.

The `StateMachine` class was extended by `WithAsync()` and `ForceAsync()`. The `AsyncStateBase` provides async all the way: `NextStateAsync()`, `OnEnterAsync()`, `OnExitAsync`.\
**Upgrade hint:** Replace `StateBase<TContext>` by `SyncStateBase<TContext>`.

The same convention was applied to `DriverState`. It was renamed to `SyncDriverState` and a new implementation `AsyncDriverState` was added with full async support.\
**Upgrade hint:** Replace `DriverState<TContext>` by `SyncDriverState<TContext>`.

### Server Module lifecycle refactored to async methods

The lifecycle methods of ServerModules have been migrated from void to `async Task` to enable modern asynchronous programming with async/await.

**Changes in ServerModuleBase:**

- `OnInitialize()` -> `OnInitializeAsync()`
- `OnStart()` -> `OnStartAsync()`
- `OnStop()` -> `OnStopAsync()`

````cs
protected override Task OnInitializeAsync()
{
    return Task.CompletedTask;
}

protected override async Task OnStartAsync()
{
    var asyncFoo = Container.Resolve<IAsyncFoo>();
    await asyncFoo.StartAsync();
}

protected override async Task OnStopAsync()
{
    var asyncFoo = Container.Resolve<IAsyncFoo>();
    await asyncFoo.StopAsync();
}
````

**Changes in IModuleManager:**

- `StartModules()` -> `StartModulesAsync()`
- `StopModules()` -> `StopModulesAsync()`
- `InitalizeModule(IServerModule module)` -> `InitalizeModuleAsync(IServerModule module)`
- `StartModule(IServerModule module)` -> `StartModuleAsync(IServerModule module)`
- `StopModule(IServerModule module)` -> `StopModuleAsync(IServerModule module)`
- `ReincarnateModule(IServerModule module)` -> `ReincarnateModuleAsync(IServerModule module)`

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

Note: For projects using top-level statements you can use the new async methods without any further actions.

### Async Lifecycle Support for ResourceManagement

The ResourceManagement has been updated to support **asynchronous lifecycle methods**, including initialization, startup, and shutdown processes.

**Changes in Resource**:

- `OnInitialize()` -> `OnInitializeAsync()`
- `OnStart()` -> `OnStartAsync()`
- `OnStop()` -> `OnStopAsync()`

````cs
protected override Task OnInitializeAsync()
{
    return base.OnStartAsync();
}

protected override Task OnStartAsync()
{
    return base.OnStartAsync();
}

protected override Task OnStopAsync()
{
    return base.OnStopAsync();
}
````

Additionally, the APIs of these components have been updated to return `Task` or `Task<T>` to reflect asynchronous behavior.

**`IResourceGraph`**

- `void Save` -> `Task Save`
- `bool Destroy` -> `Task<bool> Destroy`

**`IResourceManagement`-facade:**

- Modification methods are now using async Task.

### Async Lifecycle Support for OrderManagement

The OrderManagement has been updated to support **asynchronous lifecycle methods**, including initialization, startup, and shutdown processes.

The following plugins have been migrated to the **async lifecycle**:

- `IAdviceExecutor`
- `IDocumentLoader`
- `IPartsAssignment`
- `IProductAssignment`
- `IRecipeAssignment`
- `IOperationValidation`
- `IOperationDispatcher`

Additionally, the APIs of these components have been updated to return `Task` or `Task<T>` to reflect asynchronous behavior.

**`IOrderManagement`-facade:**

- `Operation GetOperation` -> `Task<Operation> GetOperation`
- `Operation AddOperation` -> `Task<Operation> AddOperation`
- `void BeginOperation` -> `Task BeginOperation`
- `void AbortOperation` -> `Task AbortOperation`
- `void SetOperationSortOrder` -> `Task SetOperationSortOrder`
- `void UpdateSource` -> `Task UpdateSource`
- `void ReportOperation` -> `Task ReportOperation`
- `void InterruptOperation` -> `Task InterruptOperation`
- `void Reload` -> `Task Reload`

**`IOperationPool`:**

- `Operation Get` -> `Task<Operation> Get`

**`IAdviceExecutor`**

`bool ValidateCreationContext` -> `Task<bool> ValidateCreationContext`

**`IOperationDispatcher`**

- `void Dispatch` -> `Task Dispatch`
- `void Complete` -> `Task Complete`
- `void JobProgressChanged` -> `Task JobProgressChanged`
- `void JobStateChanged` -> `Task JobStateChanged`

### Async Lifecycle Support for ProcessEngine

The ProcessEngine has been updated to support **asynchronous lifecycle methods**, including initialization, startup, and shutdown processes.

The following plugins have been migrated to the **async lifecycle**:

- `ICellSelector`

Additionally, the APIs of these components have been updated to return `Task` or `Task<T>` to reflect asynchronous behavior.

**`IJobManagement`-facade:**

- `void Add` -> `Task Add`

**`IProcessControl`-facade:**

- `IReadOnlyList<IProcess> RunningProcesses` -> `IReadOnlyList<IProcess> GetRunningProcesses`
- `IReadOnlyList<IProcess> GetProcesses` -> `Task<IReadOnlyList<IProcess>> GetArchivedProcesses`

## WorkerSupport / VisualInstructions

The WorkerSupport Module `Moryx.ControlSystem.WorkerSupport` was renamed to `Moryx.VisualInstructions.Controller` to match all namespaces. Also the Resource project was renamed from `Moryx.Resources.AssemblyInstruction` to `Moryx.Resources.VisualInstructions`.

VisualInstructions has an own separate namespace now.

| Project                             | Description                                               | MORYX 8                                     |
|-------------------------------------|-----------------------------------------------------------|---------------------------------------------|
| Moryx.VisualInstructions            | API for usage of Visual Instructions                      | Moryx.ControlSystem.VisualInstructions      |
| Moryx.VisualInstructions.Controller | Module for managing Visual Instructions                   | Moryx.ControlSystem.WorkerSupport           |
| Moryx.VisualInstructions.Web        | Web Module to display Visual Instructions                 | Moryx.ControlSystem.WorkerSupport.Web       |
| Moryx.VisualInstructions.Endpoints  | ASP.NET Controller for hosting API of Visual Instructions | Moryx.ControlSystem.WorkerSupport.Endpoints |
| Moryx.Resources.VisualInstructions  | Digital Twin of a visual instructions                     | Moryx.Resources.AssemblyInstructions        |

### Replaced result of visual instructions with dedicated result object

In *Moryx.Factory* **6.3** and **8.1** we introduced the new result object and optional extended APIs. The result object solved issues caused by localization of the different results. With **Moryx 10** we remove all old APIs based on strings.

### Replaced `IVisualInstructions` with `VisualInstructionParameters`

The interface was only used in `VisualInstructionParameters` which can and is being used as a base class in most cases anyway.
Hence, `IVisualInstructions` is removed in favor of a more extendable base class.

## Integration of Moryx.Simulation into Moryx.ControlSystem

To reduce the number of API packages and simplify the overall architecture, **Moryx.Simulation** has been integrated into **Moryx.ControlSystem** starting with MORYX 10. All simulation-related APIs and functionality are now part of the Moryx.ControlSystem package. This change streamlines dependency management and makes it easier to maintain and extend simulation features within the control system context.

The simulator module has also been renamed, and its namespace and package id have changed accordingly to reflect its new location within Moryx.ControlSystem.

## ProcessEngineContext and ControlSystemAttached/Detached

The methods `ControlSystemAttached` and `ControlSystemDetached` were renamed to `ProcessEngineAttached` and `ProcessEngineDetached` to match the naming of the framework. ControlSystem is a term for multiple modules and components used within the framework (ProcessEngine, SetupProvider, MaterialManager, ...).

The `ProcessEngineContext` was added to the `ProcessEngineAttached` to provide the `Cell` a possibility to gather information from the process engine. The class is empty in 10.0 because it defines only the API. Features are implemented in the next feature-releases of MORYX 10.x.

## Renaming and Typo-Fixes

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
- IControlSystemBound.ControlSystemAttached -> ICell.ProcessEngineAttached
- IControlSystemBound.ControlSystemDetached -> ICell.ProcessEngineDetached
- ProductQuery.Type -> ProductQuery.TypeName
- IProcessControl.GetProcesses -> IProcessControl.GetArchivedProcesses

## Reduction of interfaces

Several interfaces have been removed to streamline the codebase and reduce complexity. The following interfaces are no longer available:

- `IProductType`: Replaced with base-class `ProductType`
- `IProductInstance`: Replaced with base-class `ProductInstance`
- `IProductPartLink`: Replaced with base-class `ProductPartLink`
- `IConfig`: Replaced with base-class `ConfigBase`
- `IDatabaseConfig`: Replaced with base-class `DatabaseConfig`
- `IControlSystemBound`: Merged with `ICell`
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

## Other model adjustments

- Removed `TypeName` from ProcessEntity. It was not used.
- Combined `Classname`, `Namespace` in `TypeName` of `WorkplanStepEntity` and removed `Assembly`
- Renamed `Type` to `TypeName` in `RecipeEntity

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
- EntryToModelConverter: This component was used in WPF UIs to map an entry to a view model and vice versa. This is not used anymore and brings no benefit to the platform.

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

## Modules-ProcessEngine

- Removed API from IJobManagement: `JobEvaluation Evaluate(IProductRecipe recipe, int amount, IResourceManagement resourceManagement)`
- Added `IAsyncEnumerable<IProcessChunk> GetArchivedProcesses(ProcessRequestFilter filterType, DateTime start, DateTime end, long[] jobIds)` to `IProcessControl`

## Modules-Products

- Removed `productId` from `SaveRecipes` of `IProductStorage` and changed argument to `IReadOnlyList`
- `ProductState` and `ProductInstanceState` are now Flags-Enum.
- Introduced `ProductState.Generated` to classify a product as generated for internal use
- Added support to query by required product state

**`IProductManagement`**

- APIs (LoadType, Duplicate) are using `IIdentity` instead of `ProductIdentity` but in MORYX 10.0, only `ProductIdentity` is supported.

**`IProductStorage`**

- API `LoadType` is using `IIdentity` instead of `ProductIdentity` but in MORYX 10.0, only `ProductIdentity` is supported.

### Product importer

- Introduced `ProductImporterAttribute` for harmonized registration of importers.
- The importers are registered transient by default.

## ConstraintContext during activity-handling

The `IConstraintContext` interface was removed from `IProcess`. Instead a new wrapper was introduced `ActivityConstraintContext` which provides the Activity and the Process for better handling in `IConstraint` implementations.

## Renamed Moryx.Asp.Extensions to Moryx.AspNetCore

Renamed the package Moryx.Asp.Extensions to Moryx.AspNetCore and moved the classes to the respective namespace. This change was applied to match the Microsoft namespaces. In the past the project was used in MORYX <6 for C#-extensions on ASP.NET Components to initialize the Runtime environment and register endpoints inside the ServerModules. Since we use controllers based on the facades, these stuff was already removed.

## Changes to the MQTT Driver package

Bugfixes:

* The change in ConnectingToBrokerState prevents an application crash that were not uncommon during debugging

Cleanup:

* Mostly typos in the string resources or non equal punctuation
* Don't use the obsolete Payload Method and use ReadOnlySequence instead. To avoid unnecessary array copies methods deserializing the data have breaking signature changes
* Remove Newtonoft.Json in favor of System.Text.Json

Features:

* Add the option to add custom topics as the User by removing internal access modifiers from MqttTopic Serialize and Deserialize
* Add Retain information for Publishing and Receiving messages
* Support Mqtt5 response topics
* Support unsubscribing from Topics, by removing or changing the resource
* Support changing the broker without restarting the Resource Management, by a) providing a Reconnect method and b) handling changes to the relevant properties
* Support diagnostic tracing of message contents, before and after deserialization

## EntryConvert

- Supports async invocation of methods now by `InvokeMethodAsync`. Synchronous methods are executed synchronously.
- The synchronous `InvokeMethod` does now support async methods too. They are executed synchronously.


## Merged `IProcessControlReporting` into `IProcessControl`

The `IProcessControlReporting` interface has been merged into `IProcessControl`. All reporting-related methods and the `ReportAction` enum are now part of `IProcessControl`. Remove usages of `IProcessControlReporting` and update your code to use the unified `IProcessControl` interface.

