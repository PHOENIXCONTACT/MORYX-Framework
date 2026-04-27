# MORYX-Framework Architectural Patterns

## Overview
The MORYX-Framework uses a modular, layered architecture with clear separation between public APIs (facades), internal implementations, and REST endpoints. Each module follows a consistent pattern for exposing functionality and integrating with other modules.

---

## 1. Module Facade Pattern

### Core Concept
Each MORYX module exposes its public API through **facade interfaces** implemented by the module controller.

### Pattern Structure

**Facade Interfaces** - Define the public contract:
- `IOrderManagement` - Orders module facade
- `IOperatorManagement` - Operators module facade
- `IProductManagement` - Products module facade
- `IResourceManagement` - Resources module facade

**Module Controller** - Exports the facade:
```csharp
public class ModuleController : ServerModuleBase<ModuleConfig>,
    IFacadeContainer<IOrderManagement>
{
    private readonly OrderManagementFacade _orderManagement = new();

    IOrderManagement IFacadeContainer<IOrderManagement>.Facade => _orderManagement;

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        ActivateFacade(_orderManagement);
    }
}
```

### Multiple Facades
Modules can export multiple facades simultaneously:
```csharp
public class ModuleController : ServerModuleBase<ModuleConfig>,
    IFacadeContainer<IProductManagement>,
    IFacadeContainer<IWorkplans>
{
    private readonly ProductManagementFacade _productManagement = new();

    IProductManagement IFacadeContainer<IProductManagement>.Facade => _productManagement;
    IWorkplans IFacadeContainer<IWorkplans>.Facade => _productManagement;
}
```

### Module Dependencies
Modules import other facades using `[RequiredModuleApi]` attribute:
```csharp
[RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
public IResourceManagement ResourceManagement { get; set; }

[RequiredModuleApi(IsStartDependency = true)]
public IOperatorManagement OperatorManagement { get; set; }
```

---

## 2. Facade Interface Design

### Characteristics
- **Async-first**: Methods return `Task<T>` for async operations
- **Event-based**: Use events for state changes and notifications
- **Type-safe**: Generic methods with constraints for resource querying
- **Context-aware**: Use context objects for complex operations

### Examples

**IOrderManagement** (Facade):
```csharp
public interface IOrderManagement
{
    // CRUD Operations
    Task<Operation> LoadOperationAsync(Guid identifier, CancellationToken cancellationToken = default);
    Task<Operation> AddOperationAsync(OperationCreationContext context, CancellationToken cancellationToken = default);
    Task UpdateOperationAsync(Operation operation, OperationUpdate update, CancellationToken cancellationToken = default);
    IReadOnlyList<Operation> GetOperations(Func<Operation, bool> filter);

    // State Transitions
    Task BeginOperationAsync(Operation operation, int amount, CancellationToken cancellationToken = default);
    Task ReportOperationAsync(Operation operation, OperationReport report, CancellationToken cancellationToken = default);

    // Events for state changes
    event EventHandler<OperationChangedEventArgs> OperationProgressChanged;
    event EventHandler<OperationStartedEventArgs> OperationStarted;
    event EventHandler<OperationReportEventArgs> OperationCompleted;
}
```

**IOperatorManagement** (Facade):
```csharp
public interface IOperatorManagement
{
    IReadOnlyList<Operator> Operators { get; }
    void AddOperator(Operator @operator);
    void UpdateOperator(Operator @operator);
    void DeleteOperator(string identifier);

    event EventHandler<OperatorChangedEventArgs> OperatorChanged;
}
```

**IProductManagement** (Facade):
```csharp
public interface IProductManagement : IRecipeProvider, IWorkplans
{
    Task<IReadOnlyList<ProductType>> LoadTypesAsync(ProductQuery query, CancellationToken cancellationToken = default);
    Task<ProductInstance> CreateInstanceAsync(ProductType productType, CancellationToken cancellationToken = default);
    Task<ProductInstance> LoadInstanceAsync(long id, CancellationToken cancellationToken = default);

    event EventHandler<ProductType> TypeChanged;
}
```

**IResourceManagement** (Generic Facade):
```csharp
public interface IResourceManagement
{
    TResource GetResource<TResource>() where TResource : class, IResource;
    TResource GetResource<TResource>(long id) where TResource : class, IResource;
    TResource GetResource<TResource>(ICapabilities requiredCapabilities) where TResource : class, IResource;

    IEnumerable<TResource> GetResources<TResource>() where TResource : class, IResource;
    IEnumerable<TResource> GetResources<TResource>(ICapabilities requiredCapabilities) where TResource : class, IResource;
}
```

---

## 3. Business Models and Domain Entities

### Core Entities

**Order & Operation**:
```csharp
public class Order
{
    public virtual string Number { get; protected set; }  // "ORD-001"
    public virtual string Type { get; protected set; }    // Custom field
    public virtual IReadOnlyList<Operation> Operations { get; protected set; }
}

public class Operation
{
    public virtual Guid Identifier { get; protected set; }
    public virtual Order Order { get; protected set; }

    // Identifiers
    public virtual string Number { get; protected set; }      // "0030"
    public virtual string Name { get; protected set; }

    // Quantities
    public virtual int TotalAmount { get; protected set; }
    public virtual int TargetAmount { get; protected set; }
    public virtual int OverDeliveryAmount { get; protected set; }
    public virtual int UnderDeliveryAmount { get; protected set; }

    // Scheduling
    public virtual DateTime PlannedStart { get; protected set; }
    public virtual DateTime PlannedEnd { get; protected set; }
    public virtual DateTime? Start { get; protected set; }
    public virtual DateTime? End { get; protected set; }
    public virtual double TargetCycleTime { get; protected set; }
}
```

**Operator**:
```csharp
public class Operator
{
    public virtual string Identifier { get; set; }  // Unique: card number, personal number
    public virtual string? FirstName { get; set; }
    public virtual string? LastName { get; set; }
    public virtual string? Pseudonym { get; set; }
}
```

**Resource** (Base):
```csharp
[DataContract, EntrySerialize(EntrySerializeMode.Never)]
public abstract class Resource : ILoggingComponent, IResource, IAsyncInitializablePlugin
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public virtual object Descriptor => this;  // For UI serialization

    public Resource Parent { get; set; }
    public IReferences<Resource> Children { get; set; }

    protected virtual Task OnInitializeAsync(CancellationToken cancellationToken) { }
    protected virtual Task OnStartAsync(CancellationToken cancellationToken) { }
    protected virtual Task OnStopAsync(CancellationToken cancellationToken) { }
}
```

**Product Types**:
```csharp
public class ProductType
{
    // Identity
    public virtual IIdentity Identity { get; set; }  // Name + Revision

    // Type info
    public virtual string Name { get; set; }

    // Relationships
    public virtual ProductType Parent { get; set; }  // For inheritance
}

public class ProductInstance
{
    public virtual long Id { get; set; }
    public virtual ProductType ProductType { get; set; }
    public virtual ProductInstanceState State { get; set; }
}
```

---

## 4. Event Publishing Pattern

### Event Args Classes
Events use custom `EventArgs` classes to provide context:

**OperationChangedEventArgs**:
```csharp
public class OperationChangedEventArgs : EventArgs
{
    public Operation Operation { get; }

    public OperationChangedEventArgs(Operation operation)
    {
        Operation = operation;
    }
}
```

**OperatorChangedEventArgs**:
```csharp
public class OperatorChangedEventArgs : EventArgs
{
    public OperatorChange Change { get; set; }
    public Operator Operator { get; set; }

    public OperatorChangedEventArgs(Operator @operator)
    {
        Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
    }
}
```

### Multiple Event Types
Modules publish multiple events for different scenarios:

**Orders Module Events**:
- `OperationProgressChanged` - Quantity/state updates
- `OperationStarted` - Operation begun with user info
- `OperationUpdated` - General updates
- `OperationAdviced` - Advice provided
- `OperationReportEventArgs` - Partial report submitted
- `OperationCompleted` - Full report and completion
- `OperationInterrupted` - Interruption occurred

### Event Pattern
```csharp
// Event handler registration in facade
public event EventHandler<OperationChangedEventArgs> OperationProgressChanged;

// Publishing
protected virtual void OnOperationProgressChanged(Operation operation)
{
    OperationProgressChanged?.Invoke(this, new OperationChangedEventArgs(operation));
}
```

---

## 5. REST Endpoint Pattern

### Structure
Each module has an `.Endpoints` project with:
1. **Controllers** - ASP.NET Core controllers (one per facade)
2. **Models** - DTOs for JSON serialization
3. **Converter** - Converts between domain models and DTOs
4. **Permissions** - Policy-based authorization

### Controller Example

**OrderManagementController**:
```csharp
[ApiController]
[Route("api/moryx/orders/")]
[Produces("application/json")]
public class OrderManagementController : ControllerBase
{
    private readonly IOrderManagement _orderManagement;

    #region Queries
    [HttpGet]
    [Authorize(Policy = OrderPermissions.CanView)]
    public ActionResult<OperationModel[]> GetOperations(string orderNumber = null)
    {
        var operations = _orderManagement.GetOperations(_ => true)
            .Where(o => orderNumber is null || o.Order.Number == orderNumber)
            .Select(Converter.ToModel)
            .ToArray();
        return operations;
    }

    #endregion

    #region Event Streaming
    [HttpGet("stream")]
    public async Task OperationStream(CancellationToken cancellationToken)
    {
        var response = Response;
        response.Headers["Content-Type"] = "text/event-stream";

        var operationsChannel = Channel.CreateUnbounded<Tuple<string, string>>();

        // Subscribe to events
        _orderManagement.OperationUpdated += (_, eventArgs) =>
        {
            var json = JsonConvert.SerializeObject(Converter.ToModel(eventArgs.Operation));
            operationsChannel.Writer.TryWrite(new Tuple<string, string>(
                nameof(OperationTypes.Update), json));
        };

        // Stream events
        while (!cancellationToken.IsCancellationRequested)
        {
            var changes = await operationsChannel.Reader.ReadAsync(cancellationToken);
            await response.WriteAsync($"event: {changes.Item1}\n", cancellationToken);
            await response.WriteAsync($"data: {changes.Item2}\r\r", cancellationToken);
        }
    }
    #endregion
}
```

**OperatorManagementController** (CRUD Pattern):
```csharp
[ApiController]
[Route("api/moryx/operators/")]
[Produces("application/json")]
public class OperatorManagementController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [Authorize(Policy = OperatorPermissions.CanManage)]
    public ActionResult<string> Add(OperatorModel model)
        => Response(() => _operatorManagement.AddOperator(model.ToType()));

    [HttpPut("{identifier}")]
    [Authorize(Policy = OperatorPermissions.CanManage)]
    public ActionResult<string> Update(string identifier, OperatorModel model)
    {
        model.Identifier = identifier;
        return Response(() => _operatorManagement.UpdateOperator(model.ToType()));
    }

    [HttpDelete("{operatorIdentifier}")]
    [Authorize(Policy = OperatorPermissions.CanManage)]
    public ActionResult<string> DeleteOperator(string operatorIdentifier)
        => Response(() => _operatorManagement.DeleteOperator(operatorIdentifier));
}
```

### Data Transfer Objects (DTOs)

**OperationModel**:
```csharp
[DataContract(IsReference = true)]
public class OperationModel
{
    [DataMember]
    public Guid Identifier { get; set; }

    [DataMember]
    public int TotalAmount { get; set; }

    [DataMember]
    public string Number { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public DateTime? Start { get; set; }

    [DataMember]
    public DateTime? End { get; set; }
}
```

**OperatorModel**:
```csharp
public class OperatorModel
{
    public string? Identifier { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Pseudonym { get; set; }
}
```

### Authorization Pattern

**Permission Classes**:
```csharp
public static class OrderPermissions
{
    public const string CanView = "ORDER_VIEW";
    public const string CanBegin = "ORDER_BEGIN";
    public const string CanReport = "ORDER_REPORT";
    public const string CanViewDocuments = "ORDER_VIEW_DOCUMENTS";
}

public static class OperatorPermissions
{
    public const string CanView = "OPERATOR_VIEW";
    public const string CanManage = "OPERATOR_MANAGE";
}
```

---

## 6. CRUD Operations Pattern

### Standard Operations Flow

**Create**:
```csharp
// Facade (Orders module)
Task<Operation> AddOperationAsync(OperationCreationContext context, CancellationToken cancellationToken = default);

// Endpoint
[HttpPost("operations")]
public async Task<ActionResult<OperationModel>> CreateOperation(OperationCreationContextModel model)
{
    var context = Converter.ToContext(model);
    var operation = await _orderManagement.AddOperationAsync(context);
    return Converter.ToModel(operation);
}
```

**Read**:
```csharp
// Facade
Task<Operation> LoadOperationAsync(Guid identifier, CancellationToken cancellationToken = default);
IReadOnlyList<Operation> GetOperations(Func<Operation, bool> filter);

// Endpoint
[HttpGet("{guid}")]
public async Task<ActionResult<OperationModel>> GetOperation(Guid guid)
{
    var operation = await _orderManagement.LoadOperationAsync(guid);
    return operation == null ? NotFound() : Converter.ToModel(operation);
}
```

**Update**:
```csharp
// Facade
Task UpdateOperationAsync(Operation operation, OperationUpdate update, CancellationToken cancellationToken = default);

// Endpoint
[HttpPut("{guid}")]
public async Task<ActionResult> UpdateOperation(Guid guid, OperationUpdateModel model)
{
    var operation = await _orderManagement.LoadOperationAsync(guid);
    var update = Converter.ToContext(model);
    await _orderManagement.UpdateOperationAsync(operation, update);
    return Ok();
}
```

**Delete**:
```csharp
// Facade
Task AbortOperationAsync(Operation operation, CancellationToken cancellationToken = default);

// Endpoint
[HttpDelete("{guid}")]
public async Task<ActionResult> DeleteOperation(Guid guid)
{
    var operation = await _orderManagement.LoadOperationAsync(guid);
    await _orderManagement.AbortOperationAsync(operation);
    return NoContent();
}
```

---

## 7. Module Integration Patterns

### Dependency Injection

**Modules declare module-level dependencies**:
```csharp
public class ModuleController : ServerModuleBase<ModuleConfig>,
    IFacadeContainer<IOperatorManagement>
{
    [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
    public IResourceManagement ResourceManagement { get; set; }

    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        Container.SetInstance(ResourceManagement);
        return Task.CompletedTask;
    }
}
```

### Facade Array Injection
Multiple instances of a facade can be injected:
```csharp
[RequiredModuleApi(IsStartDependency = true)]
public IFacadeB[] AllInstances { get; set; }
```

### Cross-Module Communication

**Orders Module uses Products**:
```csharp
// In Orders.Endpoints controller
public class OrderManagementController
{
    private readonly IOrderManagement _orderManagement;
    private readonly IProductManagement _productManagement;  // From Products module

    // Converts product references
}
```

---

## 8. Context Objects Pattern

### Purpose
Context objects encapsulate complex operation parameters and provide validation/metadata:

**OperationCreationContext**:
```csharp
public class OperationCreationContext
{
    public Recipe Recipe { get; set; }
    public ProductInstance ProductInstance { get; set; }
    public int PlannedQuantity { get; set; }
    // Additional metadata
}
```

**BeginContext**:
```csharp
public class BeginContext
{
    // Contains information needed to begin an operation
    // Returned from GetBeginContext() for validation
}
```

**ReportContext**:
```csharp
public class ReportContext
{
    // Contains information needed to report on an operation
    // Returned from GetReportContext() for validation
}
```

---

## 9. Configuration and Initialization

### Module Lifecycle

```csharp
public class ModuleController : ServerModuleBase<ModuleConfig>,
    IFacadeContainer<IOrderManagement>
{
    private readonly OrderManagementFacade _orderManagement = new();

    // Initialization phase
    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        Container
            .ActivateDbContexts(dbContextManager)
            .SetInstance(ResourceManagement)
            .SetInstance(ProductManagement);
        return Task.CompletedTask;
    }

    // Startup phase
    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        ActivateFacade(_orderManagement);
        await Container.Resolve<IOrderManager>().Start();
    }

    // Shutdown phase
    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        DeactivateFacade(_orderManagement);
        return base.OnStopAsync(cancellationToken);
    }
}
```

---

## 10. Extensibility and Serialization

### Custom Descriptors
Resources expose a `Descriptor` for UI serialization:
```csharp
[EntrySerialize(EntrySerializeMode.Never)]
public abstract class Resource
{
    [EntrySerialize]
    public virtual object Descriptor => this;
}
```

### Entry Serialization
Products use entry-based serialization for flexibility:
```csharp
[HttpPost("types")]
public async Task<ActionResult<long>> SaveType(ProductModel newTypeModel)
{
    var type = ReflectionTool.GetPublicClasses<ProductType>()
        .FirstOrDefault(t => t.Name == newTypeModel.Type);

    var newType = await _productConverter.ConvertProductBack(newTypeModel, productType);
    return await _productManagement.SaveTypeAsync(newType);
}
```

---

## Summary Table

| Pattern | Purpose | Example |
|---------|---------|---------|
| **Facade Interface** | Public module API | `IOrderManagement`, `IOperatorManagement` |
| **Module Controller** | Implements `IFacadeContainer<T>` | Exports facades and manages lifecycle |
| **Domain Entity** | Business model | `Operation`, `Operator`, `ProductType` |
| **EventArgs** | Event metadata | `OperationChangedEventArgs` |
| **DTO Model** | REST serialization | `OperationModel`, `OperatorModel` |
| **Controller** | HTTP endpoints | `OrderManagementController` |
| **Context Object** | Complex operation params | `OperationCreationContext`, `BeginContext` |
| **Permission Class** | Authorization policies | `OrderPermissions`, `OperatorPermissions` |
| **Converter** | Model transformation | Domain ↔ DTO conversion |
| **Descriptor** | UI customization | Resource extensibility |

---

## Key Design Principles

1. **Facade First**: All public APIs go through facades, not direct class instantiation
2. **Async by Default**: Facades use `Task<T>` for I/O operations
3. **Event-Driven**: State changes notify subscribers through events
4. **Generic Constraints**: Resource queries use generics with type constraints
5. **Explicit Interface Implementation**: Multiple facades use explicit implementation
6. **Dependency Injection**: Modules inject other facades explicitly
7. **Contextual Parameters**: Complex operations use context objects
8. **Authorization Everywhere**: All endpoints require explicit policy authorization
9. **Consistent REST Pattern**: Follows standard REST conventions with streaming support
10. **Model Separation**: Domain models kept separate from DTOs
