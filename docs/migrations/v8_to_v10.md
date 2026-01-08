# MORYX 8.x to 10.x

## What and why did we want to change aspects of MORYX in release 10?

### Async life-cycle

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

#### Changes in `StateBase`

We now provide a full async implementation of the `StateBase`. To keep consistent naming, the `StateBase` was split to `SyncStateBase` and `AsyncStateBase`.
Due to the reduction of unnecessary interfaces, `IState` was removed and `StateBase` will be used in e.g. `IStateContext` now.

The `StateMachine` class was extended by `WithAsync()` and `ForceAsync()`. The `AsyncStateBase` provides async all the way: `NextStateAsync()`, `OnEnterAsync()`, `OnExitAsync`.
The same convention was applied to `DriverState`. It was renamed to `SyncDriverState` and a new implementation `AsyncDriverState` was added with full async support.

The `StateMachine.Initialize` method was renamed to `StateMachine.ForContext` to better reflect its purpose. Additionally, a new method `ForAsyncContext` was introduced to create an asynchronous state machines.

The `StateMachine.Reload` methods were merged into `StateMachine.With` which makes it possible to provide the initial state during initialization. Additionally, a new method `WithAsync` was introduced to create an asynchronous state machines including overload for providing an initial state.

<details>
  <summary> Code Replacement Snippets </summary>

  ```csharp
  StateBase<TContext> // replace with
  SyncStateBase<TContext>

  DriverState<TContext> // replace with
  SyncDriverState<TContext>

  StateMachine.Initialize // replace with
  StateMachine.ForContext
  ```
</details>

#### Server Module lifecycle refactored to async methods

The lifecycle methods of ServerModules have been migrated from void to `async Task` to enable modern asynchronous programming with async/await.
- Changes to server modules through `ServerModuleBase`
  <details>
    <summary> Code Replacement Snippets </summary>

    ```csharp
    protected override void OnInitialize() // replace with
    protected override async Task OnInitializeAsync(CancellationToken cancellationToken)

    protected override void OnStart() // replace with
    protected override async Task OnStartAsync(CancellationToken cancellationToken)

    protected override void OnStop() // replace with
    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    ```
  </details>
- API changes to `IModuleManager`
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
  _Note: For projects using top-level statements you can use the new async methods without any further actions._

#### ConfigBasedComponentSelector async support

The `ConfigBasedComponentSelector` was extended to support async initialization of the selected component. If the component implements `IAsyncConfiguredInitializable`, its `InitializeAsync` method will be called during selection. It is not required that the `Create` method returns a task, the `InitializeAsync` will run synchronously after creation. If it returns a Task, it will await its completion.

The following sample shows the new possibilities of the `ConfigBasedComponentSelector`:

````cs
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
````

#### Other Async Related changes

- All public or protected APIs which are Task-based are renamed to use `Async` suffix. (Internal APIs are excluded from this rule but will be adjusted over time)
- All public APIs which are Task base provide a cancellation token parameter to support cancellation of long-running operations.
  - Plugins of modules use an none optional `CancellationToken` parameter.
  - Facade methods use an optional `CancellationToken` parameter with default value.
  - Module internal plugins which are exposed to plugins like `IProductStorage`or `IResourceGraph` use an optional `CancellationToken` parameter with default value.
- If cancellation is not supported by the component, no `CancellationToken` parameter is provided.

### Renaming and Typo-Fixes

- TcpClientConfig.IpAdress -> TcpClientConfig.IpAddress
- TcpListenerConfig.IpAdress -> TcpListenerConfig.IpAddress
- ResourceRelationType.CurrentExchangablePart -> ResourceRelationType.CurrentExchangeablePart
- ResourceRelationType.PossibleExchangablePart -> ResourceRelationType.PossibleExchangeablePart
- MqttDriver.BrokerURL -> MqttDriver.BrokerUrl
- IResourceManagement.GetAllResources -> IResourceManagement.GetResourcesUnsafe
- IResourceManagement.Create -> IResourceManagement.CreateUnsafeAsync
- IResourceManagement.Read -> IResourceManagement.ReadUnsafe
- IResourceManagement.Modify -> IResourceManagement.ModifyUnsafeAsync
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
- IProcessControl.GetProcesses -> IProcessControl.GetArchivedProcessesAsync

### Reduction of interfaces

Several interfaces have been removed to streamline the codebase and reduce complexity. The following interfaces are no longer available:

- `IDatabaseConfig`: Replaced with base-class `DatabaseConfig`
- `IControlSystemBound`: Merged with `ICell`
- `INamedTask`: Merged into `ITask`
- `IProductionRecipe`: Replaced with class `ProductionRecipe`
- `ISetupRecipe`: Replaced with class `SetupRecipe`
- `IState`: Replace with base-class `StateBase`

The following interfaces are still existent for api extensions but the base class is used in whole code base:

- `IActivity` Replaced with class `Activity`
    <details>
    <summary> Code Replacement Snippets </summary>

    ```csharp
    // ICellSelector
    public override IReadOnlyList<ICell> SelectCells(IActivity activity, IReadOnlyList<ICell> availableCells) // replace with
    public override Task<IReadOnlyList<ICell>> SelectCellsAsync(Activity activity, IReadOnlyList<ICell> availableCells, CancellationToken cancellationToken)

    // ICell
    public override void ProcessAborting(IActivity affectedActivity) // replace with
    public override void ProcessAborting(Activity affectedActivity)
    ```
    </details>


- `IProcess`: Replaced with class `Process`
    <details>
    <summary> Code Replacement Snippets </summary>

    ```csharp
    // Parameters
    protected override void Populate(IProcess process, Parameters instance) // replace with
    protected override void Populate(Process process, Parameters instance)

    // IProcessReporter
    public event EventHandler<IProcess> ProcessBroken; // replace with
    public event EventHandler<Process> ProcessBroken;
    
    public event EventHandler<IProcess> ProcessRemoved; // replace with
    public event EventHandler<Process> ProcessRemoved;
    ```
    </details>
- `IProductType`: Replaced with class `ProductType`
- `IProductInstance`: Replaced with class `ProductInstance`
- `IProductPartLink`: Replaced with class `ProductPartLink`
- `IConfig`: Replaced with class `ConfigBase`


### Data Model Changes

With MORYX 10, several changes have been made to the data model to improve performance and maintainability. Notable changes include:

- Naming conventions across the data model have been standardized to ensure consistency and clarity. We use pluralised names for DbSet properties and singular names for entity classes. Sample: `public DbSet<ProductEntity> Products { get; set; }` instead of `public DbSet<ProductEntity> ProductEntities { get; set; }`.
- Derived data models are now fully supported. This needs some changes how the model is defined in code first scenarios. Please refer to the [Data Model Tutorial](/docs/tutorials/data-model/CodeFirst.md) for more information.
- Removed support of Dump and Restore operations in the data model. These operations were rarely used and added unnecessary complexity to the data model management. Use your database admin tools to perform backup and restore operations instead.
- Data model configs are reduced to the targeting model configurator and connection string.

**Changes of DatabaseConfig**

Old version:
````json
{
  "ConnectionSettings": {
    "$type": "Moryx.Model.Sqlite.SqliteDatabaseConnectionSettings, Moryx.Model.Sqlite",
    "Database": "ProcessContext",
    "ConnectionString": "Data Source=./db/ProcessContext.db;Mode=ReadWrite;"
  },
  "ConfiguratorTypename": "Moryx.Model.Sqlite.SqliteModelConfigurator, Moryx.Model.Sqlite, Version=8.0.0.0, Culture=neutral, PublicKeyToken=null",
  "ConfigState": "Valid"
}
````

New version:

````json
{
  "ConfiguratorType": "Moryx.Model.Sqlite.SqliteModelConfigurator, Moryx.Model.Sqlite, Version=10.0.0.0, Culture=neutral, PublicKeyToken=null",
  "ConnectionString": "Data Source=./db/ProcessContext.db;Mode=ReadWrite;",
  "ConfigState": "Valid"
}
````

#### Configurations
Data model configs are reduced to the targeting model configurator and connection string, reducing complexity while still providing a richer UX in the frontend.
<details>
  <summary> Code Replacement Snippets </summary>

  ```json
  // *.DbConfig.json
  {
    "ConnectionSettings": {
      "$type": "Moryx.Model.Sqlite.SqliteDatabaseConnectionSettings, Moryx.Model.Sqlite",
      "Database": "ProcessContext",
      "ConnectionString": "Data Source=./db/ProcessContext.db;Mode=ReadWrite;"
    },
    "ConfiguratorTypename": "Moryx.Model.Sqlite.SqliteModelConfigurator, Moryx.Model.Sqlite, Version=8.0.0.0, Culture=neutral, PublicKeyToken=null",
    "ConfigState": "Valid"
  } 

  // replace with
  {
    "ConfiguratorType": "Moryx.Model.Sqlite.SqliteModelConfigurator, Moryx.Model.Sqlite, Version=10.0.0.0, Culture=neutral, PublicKeyToken=null",
    "ConnectionString": "Data Source=./db/ProcessContext.db;Mode=ReadWrite;",
    "ConfigState": "Valid"
  }
  ```
</details>

#### Attributes

- `Moryx.Model.*.Attributes` have been merged into `Moryx.Model.*` 
- Context attribute renaming
  <details>
    <summary> Code Replacement Snippets </summary>

    ```csharp
    [SqliteContext] // replace with
    [SqliteDbContext]
    
    [NpgsqlDatabaseContext] // replace with
    [NpgsqlDbContext]
    ```
  </details>

### Rarely used features removed

These features were infrequently used and has been removed to simplify the codebase.

- ExceptionPrinter: Used to print exceptions to different outputs. Use Exception.ToString() instead.
- CrashHandler: Used to handle application crashes. This feature was used were the runtime was a console application.
- Caller: Used to get information about the calling method. Use System.Diagnostics.StackTrace instead.
- Moryx.Endpoints: This namespace contained all base classes for the time when an endpoint was hosted inside a module. Since we use controllers it is deprecated.
- Moryx.Identity: This namespace contained base classes and services for the old WPF client to provide the authorization context. Since MORYX support web uis, this is deprecated
- PortConfig: Used for old wcf services. Deprecated since ASP.NET Core.
- ProxyConfig.Port: Use the full address instead. It contains also http/https, domain and port
- EntryToModelConverter: This component was used in WPF UIs to map an entry to a view model and vice versa. This is not used anymore and brings no benefit to the platform.
- `HandlerMap` was removed from code. I was a good helper for .NET Framework. Since `switch` supports [pattern matching in C#7.0](https://devblogs.microsoft.com/dotnet/new-features-in-c-7-0/#switch-statements-with-patterns) this is not required anymore.

### Reworked driver APIs

#### Async driver APIs
All driver APIs have been reworked to use TPL async/await instead of callbacks for the following reasons:

- The same logic looked almost synchronous with the callbacks before, even though it wasn't in most cases.
- The async approach is cleaner and integrates seamlessly with .NET’s exception system.
- You can chain tasks with LINQ-like methods or await syntax.
- Async stack-traces in IDE show the actual logical call flow — even across await boundaries.

#### Reworked driver API structure
- The APIs for `IMessageDriver` and `IInOutDriver` with their generics and different variants was too complicated and all known usages simply used objects and root members instead different argument types. So we simplified the APIs, which also improves exhangeability of different drivers and simplifies Simulator implementations. To adjust your usages, simply remove all generic arguments.
- `IRfidDriver`, `IScannerDriver`, `IPickByLightDriver` and `IWeightScaleDriver` were extended with commonly used methods, extendable by options and result objects
- Added generic `ISingleInput{TOptions, TResult}` and `IContinuousInput{TOptions, TResult}` for general pattern of input devices

#### Breaking changes
- Remove `TransmissionResult` without replacement (Use subclasses for async API responses directly, if applicable)
- Replaced `DriverNotRunningException` with `DriverStateException` to generalize it for other invalid state calls.
  <details>
    <summary> Code Replacement Snippets </summary>

    ```csharp
    new DriverNotRunningException() // replace with
    new DriverStateException(StateClassification.Running)
    ```
  </details>
- Removed `DriverResponse` use similar but more general `FunctionResult` instead.

## Breaking changes in the different MORYX components

### The Framework and Abstractions

#### EntryConvert

- Supports async invocation of methods now by `InvokeMethodAsync`. Synchronous methods are executed synchronously.
- The synchronous `InvokeMethod` now supports async methods too. They are executed synchronously.
- Added support for `AllowedValuesAttribute` and `DeniedValuesAttribute`. Refer to EntryConvert [PossibleValues-docs](/docs/articles/framework/Serialization/PossibleValues.md).
- EntryConvert now uses `EntryPossible[]` instead of `string[]` for `EntryValue.Possible`; each item contains `Key`, `DisplayName` and `Description`.
- Added support for additional ValidationAttributes in EntryConvert
  - LengthAttribute: Sets the `EntryValidation.Minimum` and `EntryValidation.Maximum`
  - DataTypeAttribute: Sets the `EntryValidation.DataType`
  - Base64StringAttribute: Sets the new `EntryUnitType.Base64`
- Removed `PrimitiveValuesAttribute`, use `AllowedValuesAttribute` of .NET instead.
- Changed `PossibleValuesAttribute.GetValues(IContainer localContainer, IServiceProvider serviceProvider)` from virtual to abstract making overrides mandatory

#### API, Package and Namespace changes

- Renamed `EntryUnitType.File` to `EntryUnitType.FilePath`
- Renamed `EntryUnitType.Directory` to `EntryUnitType.DirectoryPath`
- Renamed the package and namespaces `Moryx.Asp.Extensions` to `Moryx.AspNetCore`  
  Renamed  Moryx.Asp.Extensions to Moryx.AspNetCore and moved the classes to the respective namespace. This change was applied to match the Microsoft namespaces. In the past the project was used in MORYX <6 for C#-extensions on ASP.NET Components to initialize the Runtime environment and register endpoints inside the ServerModules. Since we use controllers based on the facades, these components were already removed..
- Moved namespace`Moryx.Tools.FunctionResult`to `Moryx.Tools`
- Moved all classes in the `Moryx.AbstractionLayer` namespace to more specific domain namespaces e.g. `Moryx.AbstractionLayer.Resources`, `Moryx.AbstractionLayer.Products`, `Moryx.AbstractionLayer.Processes` etc.

### Module WorkerSupport / VisualInstructions

The WorkerSupport Module `Moryx.ControlSystem.WorkerSupport` was renamed to `Moryx.VisualInstructions.Controller` to match all namespaces. Also the Resource project was renamed from `Moryx.Resources.AssemblyInstruction` to `Moryx.Resources.VisualInstructions`.

VisualInstructions has an own separate namespace now.

| Project                             | Description                                               | MORYX 8                                     |
|-------------------------------------|-----------------------------------------------------------|---------------------------------------------|
| Moryx.VisualInstructions            | API for usage of Visual Instructions                      | Moryx.ControlSystem.VisualInstructions      |
| Moryx.VisualInstructions.Controller | Module for managing Visual Instructions                   | Moryx.ControlSystem.WorkerSupport           |
| Moryx.VisualInstructions.Web        | Web Module to display Visual Instructions                 | Moryx.ControlSystem.WorkerSupport.Web       |
| Moryx.VisualInstructions.Endpoints  | ASP.NET Controller for hosting API of Visual Instructions | Moryx.ControlSystem.WorkerSupport.Endpoints |
| Moryx.Resources.VisualInstructions  | Digital Twin of a visual instructor                       | Moryx.Resources.AssemblyInstructions        |

Required SQL Update:

````sql
// SQLite
UPDATE Resources
SET "Type" = 'Moryx.Resources.VisualInstructions.VisualInstructor'
WHERE "Type" = 'Moryx.Resources.AssemblyInstruction.VisualInstructor';

// PostgreSQL
UPDATE public."Resources"
SET "Type" = 'Moryx.Resources.VisualInstructions.VisualInstructor'
WHERE "Type" = 'Moryx.Resources.AssemblyInstruction.VisualInstructor';
````

#### Replaced result of visual instructions with dedicated result object

In *Moryx.Factory* **6.3** and **8.1** we introduced the new result object and optional extended APIs. The result object solved issues caused by localization of the different results. With **Moryx 10** we remove all old APIs based on strings.

#### Replaced `IVisualInstructions` with `VisualInstructionParameters`

The interface was only used in `VisualInstructionParameters` which can and is being used as a base class in most cases anyway.
Hence, `IVisualInstructions` is removed in favor of a more extendable base class.

### Module Simulation 

#### Integration of Moryx.Simulation into Moryx.ControlSystem

To reduce the number of API packages and simplify the overall architecture, **Moryx.Simulation** has been integrated into **Moryx.ControlSystem** starting with MORYX 10. All simulation-related APIs and functionality are now part of the Moryx.ControlSystem package. This change streamlines dependency management and makes it easier to maintain and extend simulation features within the control system context.

The simulator module has also been renamed, and its namespace and package id have changed accordingly to reflect its new location within Moryx.ControlSystem.
- Rename the module configuration file from `Moryx.Simulation.Simulator.ModuleConfig.json` to `Moryx.ControlSystem.Simulator.ModuleConfig.json`.
- Replace package reference `Moryx.Simulation.Simulator` with `Moryx.ControlSystem.Simulator`
- Remove package reference `Moryx.Simulation`

### Moryx Launcher

- The `SortIndex` configuration was moved from the `appsettings.json` to the `Moryx.Launcher.LauncherConfig.json` configuration file.  
Refer to the [Launcher](/docs/articles/launcher/Launcher.md) documentation for more information.

### Module Analytics

#### Removal of the module in favor of external web-pages

The analytics module was doing nothing and the web module was replaced by supporting external modules in `Launcher`. It is now supported to embed external web-pages into the shell. Refer to the [Launcher](/docs/articles/launcher/Launcher.md) documentation for more information.

- Remove package references of `Moryx.Analytics.Server` and `Moryx.Analytics.Web`
- (Optional) Remove module configuration file `Moryx.Analytics.Server.ModuleController.ModuleConfig.json`

### Module Orders

- Removed report from interrupt of an operation. Reporting during an interruption doesn't add any value. The quantity for the report can only be predicted and will be inaccurate if something goes wrong or is reworked during the interruption.
- Facade Renaming:
  - `GetOperationAsync` -> `LoadOperationAsync`
  - `SetOperationSortOrder` and `UpdateSource` were combined to `UpdateOperationAsync`. It supports both functionalities and included changing `PlannedStart` and `PlannedEnd` as well.

#### Async Lifecycle Support for OrderManagement

The OrderManagement has been updated to support **asynchronous lifecycle methods**, including initialization, startup, and shutdown processes.

The following plugins have been migrated to the **async lifecycle**:

- `IAdviceExecutor`
- `IDocumentLoader`
  <details>
    <summary> Code Replacement Snippets </summary>

    ```csharp
    public void Initialize(DocumentLoaderConfig config) // replace with
    public async Task InitializeAsync(DocumentLoaderConfig config, CancellationToken cancellationToken = default)

    public async Task<IReadOnlyList<Document>> Load(Operation operation) // replace with
    public async Task<IReadOnlyList<Document>> LoadAsync(Operation operation, CancellationToken cancellationToken)
    ```
  </details>
- `IPartsAssignment`
- `IProductAssignment`
- `IRecipeAssignment`  
  <details>
    <summary> Code Replacement Snippets </summary>

    ```csharp
    public override bool ProcessRecipe(IProductRecipe clone, Operation operation, IOperationLogger operationLogger) // replace with
    public override Task<bool> ProcessRecipeAsync(IProductRecipe clone, Operation operation, IOperationLogger operationLogger, CancellationToken cancellationToken)

    public override IReadOnlyList<IProductRecipe> SelectRecipes(Operation operation, IOperationLogger operationLogger) // replace with
    public override async Task<IReadOnlyList<IProductRecipe>> SelectRecipesAsync(Operation operation, IOperationLogger operationLogger, CancellationToken cancellationToken)
    ```
  </details>
- `IOperationValidation`
- `IOperationDispatcher`

Additionally, the APIs of these components have been updated to return `Task` or `Task<T>` to reflect asynchronous behavior.

**`IOrderManagement`-facade:**

- `Operation GetOperation` -> `Task<Operation> GetOperationAsync`
- `Operation AddOperation` -> `Task<Operation> AddOperationAsync`
- `void BeginOperation` -> `Task BeginOperationAsync`
- `void AbortOperation` -> `Task AbortOperationAsync`
- `void SetOperationSortOrder` -> `Task SetOperationSortOrderAsync`
- `void UpdateSource` -> `Task UpdateSourceAsync`
- `void ReportOperation` -> `Task ReportOperationAsync`
- `void InterruptOperation` -> `Task InterruptOperationAsync`
- `void Reload` -> `Task ReloadAsync`

**`IOperationPool`:**

- `Operation Get` -> `Task<Operation> GetAsync`

**`IAdviceExecutor`**

`bool ValidateCreationContext` -> `Task<bool> ValidateCreationContextAsync`

**`IOperationDispatcher`**

- `void Dispatch` -> `Task DispatchAsync`
- `void Complete` -> `Task CompleteAsync`
- `void JobProgressChanged` -> `Task JobProgressChangedAsync`
- `void JobStateChanged` -> `Task JobStateChangedAsync`

### Module Resources
#### Async Lifecycle Support for ResourceManagement

The ResourceManagement has been updated to support **asynchronous lifecycle methods**, including initialization, startup, and shutdown processes.
<details>
  <summary> Code Replacement Snippets </summary>

  ```csharp
  protected override void OnInitialize() // replace with
  protected override async Task OnInitializeAsync(CancellationToken cancellationToken)

  protected override void OnStart() // replace with
  protected override async Task OnStartAsync(CancellationToken cancellationToken)

  protected override void OnStop() // replace with
  protected override async Task OnStopAsync(CancellationToken cancellationToken)
  ```
</details>

#### API Changes
Additionally, the APIs of these components have been updated to return `Task` or `Task<T>` to reflect asynchronous behavior.
- `IResourceGraph`
  - `void Save` -> `Task SaveAsync`
  - `bool Destroy` -> `Task<bool> DestroyAsync`
- `IResourceManagement`
  - Modification methods are now using async Task.
- `IResourceInitializer`
  - It is now possible to execute initializers from the facade
  - The initializers are registered transient by default.
  - It is subject to the [#async-life-cycle](#async-life-cycle) changes.
  - Introduced `ResourceInitializerResult` object for extensibility and option to save

  <details>
    <summary> Code Replacement Snippets </summary>

    ```csharp
    public override IReadOnlyList<Resource> Execute(IResourceGraph graph) // replace with
    public override Task<ResourceInitializerResult> ExecuteAsync(IResourceGraph graph, object parameters, CancellationToken cancellationToken)
    ```
  </details>

#### Data Model Changes
- Database configuration name changed `Moryx.Resources.Model.ResourcesContext.DbConfig.json` -> `Moryx.Resources.Management.Model.ResourcesContext.DbConfig.json`  

#### Resource initialization
The API of 

### Module Process Engine

#### Async Lifecycle Support for ProcessEngine

The ProcessEngine has been updated to support **asynchronous lifecycle methods**, including initialization, startup, and shutdown processes.

The following plugins have been migrated to the **async lifecycle**:

- `ICellSelector`

Additionally, the APIs of these components have been updated to return `Task` or `Task<T>` to reflect asynchronous behavior.

**`IJobManagement`-facade:**

- `void Add` -> `Task AddAsync`

**`IProcessControl`-facade:**

- `IReadOnlyList<IProcess> RunningProcesses` -> `IReadOnlyList<IProcess> GetRunningProcesses`
- `IReadOnlyList<IProcess> GetProcesses` -> `Task<IReadOnlyList<IProcess>> GetArchivedProcessesAsync`

#### API Changes
- Removed API from IJobManagement: `JobEvaluation Evaluate(IProductRecipe recipe, int amount, IResourceManagement resourceManagement)`
- Added `IAsyncEnumerable<IProcessChunk> LoadArchivedProcessesAsync(ProcessRequestFilter filterType, DateTime start, DateTime end, long[] jobIds)` to `IProcessControl`
- The `IProcessControlReporting` interface has been merged into `IProcessControl`. All reporting-related methods and the `ReportAction` enum are now part of `IProcessControl`.   
  <details>
    <summary> Code Replacement Snippets </summary>

    ```csharp
    IProcessControlReporting // replace with
    IProcessControl
    ```
  </details>

#### Clean-ups and unifications on `JobCreationContext`
  - Removed unused constructor `public JobCreationContext(IProductRecipe recipe)`
  - Removed unused method `public JobCreationContext Add(ProductionRecipe recipe)`
  - Replaced uses of `IProductRecipe` with `ProductionRecipe`
    _Note: ProcessEngine Module did expect IWorkplanRecipe anyways, so the use of IProductRecipe would have lead to exceptions in the past already._
  - Transformed `public struct JobTemplate` to `public record JobTemplate` to allow extensions in the future.

#### ProcessEngineContext and ControlSystemAttached/Detached

The methods `ControlSystemAttached` and `ControlSystemDetached` were renamed to `ProcessEngineAttached` and `ProcessEngineDetached` to match the naming of the framework. ControlSystem is a term for multiple modules and components used within the framework (ProcessEngine, SetupProvider, MaterialManager, ...).

<details>
  <summary> Code Replacement Snippets </summary>

  ```csharp
  public override IEnumerable<Session> ControlSystemAttached() // replace with
  protected override IEnumerable<Session> ProcessEngineAttached()

  public override IEnumerable<Session> ControlSystemDetached() // replace with
  protected override IEnumerable<Session> ProcessEngineDetached()
  ```
</details>

The `ProcessEngineContext` was added to the `ProcessEngineAttached` to provide the `Cell` a possibility to gather information from the process engine. The class is empty in 10.0 because it defines only the API. Features are implemented in the next feature-releases of MORYX 10.x.

#### ConstraintContext during activity-handling

The `IConstraintContext` interface was removed from `IProcess`. Instead a new wrapper was introduced `ActivityConstraintContext` which provides the Activity and the Process for better handling in `IConstraint` implementations.


#### Data Model Changes
- Removed `TypeName` from ProcessEntity. It was not used.

### Module Media

- Facade Renaming:
  - `RemoveContent` -> `DeleteContent`
  - `RemoveVariant` -> `DeleteVariant`

### Module Products

#### Async Lifecycle Support for ProductManagement

The ProductManagement has been updated to support **asynchronous lifecycle methods**, including initialization, startup, and shutdown processes.

Additionally, the APIs of these components have been updated to return `Task` or `Task<T>` to reflect asynchronous behavior.

**`IProductManagement`-facade:**

All methods loading ProductTypes, ProductInstances, Recipes or Workplans are now returning `Task<T>` and got the `Async`-suffix.

**`ProductStorage`:**

- All strategies of the ProductStorage must now return `Task` and the methods got the `Async`-suffix.

#### IProductManagement

- APIs (LoadType, Duplicate) are using `IIdentity` instead of `ProductIdentity` but in MORYX 10.0, only `ProductIdentity` is supported.
- Facade Renaming:
  - `DuplicateAsync` -> `DuplicateTypeAsync`
  - `GetRecipesAsync` -> `LoadRecipesAsync`
  - `GetInstanceAsync` -> `LoadInstanceAsync`
  - `GetInstancesAsync` -> `LoadInstancesAsync`
  - `DeleteProductAsync` -> `DeleteTypeAsync`
  - `RemoveRecipeAsync` -> `DeleteRecipeAsync`
  - `SaveRecipe` -> `SaveRecipeAsync`
  - `LoadRecipe` -> `LoadRecipeAsync`

#### IProductStorage

- API `LoadType` is using `IIdentity` instead of `ProductIdentity` but in MORYX 10.0, only `ProductIdentity` is supported.
- Removed `productId` from `SaveRecipes` of `IProductStorage` and changed argument to `IReadOnlyList`
- Added `DeleteTypeAsync` to `IProductStorage`

#### Product importer

- Introduced `ProductImporterAttribute` for harmonized registration of importers.
- The importers are registered transient by default.
- Subject to [#async-life-cycle](#async-life-cycle) changes.
  <details>
    <summary> Code Replacement Snippets </summary>

    ```csharp
    protected Task<ProductImporterResult> Import(ProductImportContext context, DemoImportParameters parameters) // replace with
    protected override Task<ProductImporterResult> ImportAsync(ProductImportContext context, DemoImportParameters parameters, CancellationToken cancellationToken)
    ```
  </details>

#### Product State
- `ProductState` and `ProductInstanceState` are now Flags-Enum.
- Introduced `ProductState.Generated` to classify a product as generated for internal use
- Added support to query by required product state

#### Removal of ProductFile

- The `ProductFileEntity` has been removed from the data model as it was not utilized effectively. This change helps streamline the data structure and reduce unnecessary complexity.
- If you were using `ProductFileEntity`, consider using alternative storage solutions such as file systems or dedicated file storage services to manage product-related files.
- The `ProductFile` was removed completely.

#### Data Model Changes
- Database configuration name changed `Moryx.Products.Model.ProductsContext.DbConfig.json` -> `Moryx.Products.Management.Model.ProductsContext.DbConfig.json`  
This is part of the unification of the general [data model changes](#data-model-changes)
- Combined `Classname`, `Namespace` in `TypeName` of `WorkplanStepEntity` and removed `Assembly`
- Renamed `Type` to `TypeName` in `RecipeEntity

### Module Orders

- Removed **Amount Reached Notification** from OperationData. **Upgrade hint:** Replace by custom module, to add notifications for certain modules.


### Modules-ProcessData
 
- Changed default measurement value types
- notifications_acknowledged: `Id` changed from `Field` to `Tag`
- controlSystem_processes: Renamed to processEngine_processes; `Id` changed from `Field` to `Tag`
- controlSystem_activities: Renamed to processEngine_activities; `Id` changed from `Field` to `Tag`; `waittimeMS` renamed to `waitTimeMs`

### Module Notifications
- Database configuration name changed `Moryx.Notifications.Model.NotificationsContext.DbConfig.json` -> `Moryx.Notifications.Publisher.Model.NotificationsContext.DbConfig.json`  
This is part of the unification of the general [data model changes](#data-model-changes)

### MQTT Driver

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

### OPC-UA Driver`

#### API changes to `IOpcUaDriver`

- `AddSubscription` -> `AddSubscriptionAsync`
- `GetNode` -> `GetNodeAsync`
- `ReadNode` -> `ReadNodeAsync`
- `RebrowseNodes` -> `RebrowseNodesAsync`
- `WriteNode` -> `WriteNodeAsync`
