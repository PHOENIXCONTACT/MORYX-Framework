---
uid: MaterialManagerPublicAPI
---

# Material Manager Public API

## Overview

The Material Manager module provides infrastructure for managing digital twins of material containers in a cyber-physical system. The public API consists of:

- **IMaterialManagement facade**: Container lifecycle, discovery, and event publishing
- **Resource interfaces**: `IMaterialContainer`, `IOrderMaterialContainer`, `IOperatorMaterialContainer`, `IProductMaterialContainer`
- **Two-phase linking API**: `GetLinkingRequirementsAsync()` for validation, `LinkToOrderAsync()` for execution (ADR: Material Manager Linking Flow)
- **Hook plugin system**: Extensible validation and side-effect handling via configuration-driven plugins (ADR: Material Manager Hook Plugin System)
- **Event-driven architecture**: Containers emit events; internal module listener orchestrates hook execution via factory pattern
- **Full lineage tracking**: Split/merge operations with complete genealogy preservation

---

## Architectural Decisions Requiring ADR

The following design decisions should be formalized in Architectural Decision Records:

- **ADR: Material Manager Linking Flow** — Two-phase separation of validation (GetLinkingRequirementsAsync) and execution (LinkToOrderAsync) for async processing and user interaction
- **ADR: Material Manager Hook Plugin System** — Factory-based, configuration-driven hook instantiation with sequential priority-ordered execution
- **ADR: Material Manager Hook Priorities** — How hook priorities enable deterministic execution and state inspection across multiple hook instances

---

## Core Design Principles

1. **Containers are Resources**: `IMaterialContainer` extends `IResource` and is part of the physical/digital system
2. **Linking on Resources**: Link operations happen on container resource instances; facade provides extension methods for orchestrated flows
3. **Polymorphic Container Interfaces**: Different interfaces for different linking purposes (orders, operators, products)
4. **Two-Phase Linking**: Separate validation (requirements collection) from execution to enable async processing and user interaction
5. **Event-Driven Hook Processing**: Internal module listener subscribes to container events; configures hooks via factory pattern
6. **Sequential Hook Execution**: Hooks execute in priority order with shared context mutation; exception-safe
7. **Capability-based Constraints**: Container capabilities match product type requirements using MORYX capability system
8. **Full Lineage Preservation**: All material movement (split/merge) is fully traceable

---

## IMaterialManagement Facade

```csharp
/// <summary>
/// Main facade for material management operations
/// Provides container lifecycle, discovery, and event publishing
/// </summary>
public interface IMaterialManagement
{
    // Container Lifecycle (SPEC-MMM-002)

    /// <summary>
    /// Register a container with the material management system
    /// Creates the container in ResourceManagement and prepares it for linking
    /// </summary>
    /// <remarks>
    /// This is a convenience method to avoid using both ResourceManagement and MaterialManagement facades
    /// Internally creates the resource and initializes linking infrastructure
    /// </remarks>
    Task<IMaterialContainer> CreateAndRegisterContainerAsync(
        string containerName,
        string containerType,
        long? storageResourceId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Register an existing container resource with the material management system
    /// Initializes linking infrastructure and audit tracking
    /// </summary>
    Task RegisterContainerAsync(
        IMaterialContainer container,
        CancellationToken ct = default);

    /// <summary>
    /// Unregister and delete a container
    /// Cascades unlinking from all domain constructs
    /// </summary>
    Task DeleteContainerAsync(
        long containerResourceId,
        CancellationToken ct = default);

    // Container Discovery (SPEC-MMM-003)

    /// <summary>
    /// Get a container by its resource ID
    /// </summary>
    Task<IMaterialContainer?> GetContainerAsync(
        long containerResourceId,
        CancellationToken ct = default);

    /// <summary>
    /// Get all registered material containers
    /// </summary>
    Task<IReadOnlyList<IMaterialContainer>> GetContainersAsync(
        CancellationToken ct = default);

    // Lineage Operations (SPEC-MMM-008)

    /// <summary>
    /// Record a lineage event when containers are split or merged
    /// </summary>
    /// <remarks>
    /// Split: one source → multiple destinations
    /// Merge: multiple sources → one destination
    /// Typically called by workflow/automation after container resource quantity updates
    /// </remarks>
    Task RecordLineageAsync(
        ContainerLineageEvent lineageEvent,
        CancellationToken ct = default);

    // History & Records (SPEC-MMM-011)

    /// <summary>
    /// Get material records for container lifecycle events
    /// Provides timestamped traceability of all material operations
    /// </summary>
    Task<IReadOnlyList<ContainerRecord>> GetMaterialRecordsAsync(
        MaterialRecordsQueryCriteria criteria,
        CancellationToken ct = default);

    // Events

    /// <summary>
    /// Fired when a container is registered with the system
    /// </summary>
    event EventHandler<ContainerRegisteredEventArgs> ContainerRegistered;

    /// <summary>
    /// Fired when a container is deleted from the system
    /// </summary>
    event EventHandler<ContainerDeletedEventArgs> ContainerDeleted;

    /// <summary>
    /// Fired when a container's state changes (linking, unlinking, quantity, storage)
    /// Provides real-time notifications of material container state changes
    /// </summary>
    event EventHandler<ContainerChangedEventArgs> ContainerChanged;
}
```

---

## Material Container Resource Interfaces

### Base Interface

```csharp
/// <summary>
/// Base interface for all material container resources
/// A container is a physical entity (barrel, box, tray, etc.) holding material
/// </summary>
public interface IMaterialContainer : IResource
{
    /// <summary>
    /// Reference to the storage/location where this container resides
    /// Enables embedding in plant's digital twin context
    /// </summary>
    IContainerStorage? Storage { get; set; }

    /// <summary>
    /// Current quantity of material in this container
    /// </summary>
    decimal Quantity { get; set; }

    /// <summary>
    /// Capabilities defining what products/materials can be held
    /// Matches against product type requirements for validation
    /// </summary>
    ICapabilities? Capabilities { get; }
}

/// <summary>
/// Interface for resources that can store material containers
/// Examples: Plant, Workplace, Station
/// </summary>
public interface IContainerStorage : IResource
{
    /// <summary>
    /// All containers currently located at this storage resource
    /// </summary>
    IReadOnlyList<IMaterialContainer> Containers { get; }
}
```

### Order-Linked Container Interface

```csharp
/// <summary>
/// Container that can be linked to orders
/// </summary>
public interface IOrderMaterialContainer : IMaterialContainer
{
    /// <summary>
    /// The order this container is currently linked to (if any)
    /// </summary>
    Order? LinkedOrder { get; }

    /// <summary>
    /// Event fired before attempting to link this container to an order
    /// Hook plugins can add validation blocks or input requirements
    /// </summary>
    event EventHandler<LinkingOrderEventArgs> BeginLinkingOrder;

    /// <summary>
    /// Event fired after successful linking to an order
    /// </summary>
    event EventHandler<LinkedOrderEventArgs> LinkedOrder;

    /// <summary>
    /// Event fired before attempting to unlink from an order
    /// </summary>
    event EventHandler<UnlinkingOrderEventArgs> BeginUnlinkingOrder;

    /// <summary>
    /// Event fired after successful unlinking from an order
    /// </summary>
    event EventHandler<UnlinkedOrderEventArgs> UnlinkedOrder;

    /// <summary>
    /// Phase 1: Validate linking and collect requirements from hooks
    /// This is the first step in a two-phase linking flow (ADR: Material Manager Linking Flow)
    ///
    /// Fires BeginLinkingOrder event where hook plugins can:
    /// - Examine the container and order
    /// - Inject validation blocks (IsBlocking = true)
    /// - Add input requirements (e.g., operator acknowledgment)
    /// - Add tracing/context data
    ///
    /// Flow:
    /// 1. Fires BeginLinkingOrder event
    /// 2. Waits for all configured hooks to complete (sequential by priority)
    /// 3. Collects any exceptions and logs them (fails gracefully)
    /// 4. Returns LinkResult with context containing all hook requirements
    ///
    /// Returns LinkResult.Success = false if any hook blocks or required input is missing.
    /// The returned context is used in phase 2 after user provides required input.
    /// </summary>
    Task<LinkResult> GetLinkingRequirementsAsync(
        Order order,
        CancellationToken ct = default);

    /// <summary>
    /// Phase 2: Execute the linking with hook requirements satisfied
    /// This is the second step in a two-phase linking flow (ADR: Material Manager Linking Flow)
    ///
    /// Assumes the LinkingContext has been collected from GetLinkingRequirementsAsync
    /// and all hook requirements (acknowledgments, inputs) have been fulfilled by the caller.
    /// Hooks are NOT invoked again; this is purely execution.
    ///
    /// Flow:
    /// 1. Validates that context is provided
    /// 2. Performs the actual link: LinkedOrder = order
    /// 3. Fires LinkedOrder event for side-effects and audit trail
    /// 4. Records linking action in material history
    ///
    /// Returns LinkResult.Success = true if linking completed.
    /// </summary>
    Task<LinkResult> LinkToOrderAsync(
        Order order,
        LinkingContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Attempt to unlink this container from its currently linked order
    ///
    /// Similar two-phase flow pattern:
    /// 1. GetUnlinkingRequirementsAsync() - fires BeginUnlinkingOrder, collects hook requirements
    /// 2. UnlinkFromOrderAsync() - executes with fulfilled requirements
    /// Hooks run only in the requirements phase, not execution.
    /// </summary>
    Task<LinkResult> GetUnlinkingRequirementsAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Attempt to unlink this container from its currently linked order
    ///
    /// Phase 2 of unlinking: Executes the unlinking after requirements are satisfied.
    /// </summary>
    Task<UnlinkResult> UnlinkFromOrderAsync(
        UnlinkingContext context,
        CancellationToken ct = default);
}
```

### Operator-Linked Container Interface

```csharp
/// <summary>
/// Container that can be linked to operators
/// </summary>
public interface IOperatorMaterialContainer : IMaterialContainer
{
    /// <summary>
    /// The operator this container is currently linked to (if any)
    /// </summary>
    Operator? LinkedOperator { get; }

    /// <summary>
    /// Event fired before attempting to link this container to an operator
    /// </summary>
    event EventHandler<LinkingOperatorEventArgs> BeginLinkingOperator;

    /// <summary>
    /// Event fired after successful linking to an operator
    /// </summary>
    event EventHandler<LinkedOperatorEventArgs> LinkedOperator;

    /// <summary>
    /// Event fired before attempting to unlink from an operator
    /// </summary>
    event EventHandler<UnlinkingOperatorEventArgs> BeginUnlinkingOperator;

    /// <summary>
    /// Event fired after successful unlinking from an operator
    /// </summary>
    event EventHandler<UnlinkedOperatorEventArgs> UnlinkedOperator;

    /// <summary>
    /// Phase 1: Validate operator linking and collect hook requirements
    /// </summary>
    Task<LinkResult> GetLinkingRequirementsAsync(
        Operator @operator,
        CancellationToken ct = default);

    /// <summary>
    /// Phase 2: Execute the linking to operator
    /// </summary>
    Task<LinkResult> LinkToOperatorAsync(
        Operator @operator,
        LinkingContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Phase 1: Validate operator unlinking and collect hook requirements
    /// </summary>
    Task<LinkResult> GetUnlinkingRequirementsAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Phase 2: Execute the unlinking from operator
    /// </summary>
    Task<UnlinkResult> UnlinkFromOperatorAsync(
        UnlinkingContext context,
        CancellationToken ct = default);
}
```

### Product-Linked Container Interface

```csharp
/// <summary>
/// Container that can be linked to products
/// </summary>
public interface IProductMaterialContainer : IMaterialContainer
{
    /// <summary>
    /// The product type this container is currently linked to (if any)
    /// </summary>
    ProductType? LinkedProduct { get; }

    /// <summary>
    /// Event fired before attempting to link this container to a product
    /// </summary>
    event EventHandler<LinkingProductEventArgs> BeginLinkingProduct;

    /// <summary>
    /// Event fired after successful linking to a product
    /// </summary>
    event EventHandler<LinkedProductEventArgs> LinkedProduct;

    /// <summary>
    /// Event fired before attempting to unlink from a product
    /// </summary>
    event EventHandler<UnlinkingProductEventArgs> BeginUnlinkingProduct;

    /// <summary>
    /// Event fired after successful unlinking from a product
    /// </summary>
    event EventHandler<UnlinkedProductEventArgs> UnlinkedProduct;

    /// <summary>
    /// Phase 1: Validate product linking and collect hook requirements
    /// </summary>
    Task<LinkResult> GetLinkingRequirementsAsync(
        ProductType product,
        CancellationToken ct = default);

    /// <summary>
    /// Phase 2: Execute the linking to product
    /// </summary>
    Task<LinkResult> LinkToProductAsync(
        ProductType product,
        LinkingContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Phase 1: Validate product unlinking and collect hook requirements
    /// </summary>
    Task<LinkResult> GetUnlinkingRequirementsAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Phase 2: Execute the unlinking from product
    /// </summary>
    Task<UnlinkResult> UnlinkFromProductAsync(
        UnlinkingContext context,
        CancellationToken ct = default);
}
```

---

## Linking Context & Operations

```csharp
/// <summary>
/// Context for a linking operation
/// Modified by hook plugins during GetLinkingRequirementsAsync to inject validation, side-effects, and input requirements
/// Entry Format serialization of hook.Data happens only at the REST API boundary
/// </summary>
public class LinkingContext
{
    /// <summary>
    /// Whether this linking operation is manual (initiated by operator) or automatic (workflow)
    /// Hooks may use this to apply different validation logic
    /// </summary>
    public bool IsManual { get; set; }

    /// <summary>
    /// Collection of hooks that have been applied to this linking operation
    /// Populated by configured hook plugins during GetLinkingRequirementsAsync phase
    /// Each hook can inject blocks, input requirements, or tracing data
    /// </summary>
    public ICollection<LinkingHook> Hooks { get; } = new List<LinkingHook>();

    /// <summary>
    /// Convenience flag: true if any hook has IsBlocking = true
    /// Checked after all hooks complete to determine LinkResult.Success
    /// </summary>
    public bool IsBlocked => Hooks.Any(h => h.IsBlocking);

    /// <summary>
    /// Optional custom metadata for the operation
    /// Examples: user ID, timestamp, batch reference
    /// Not typically used; hook.Data is preferred for hook-specific data
    /// </summary>
    public object? CustomData { get; set; }
}

/// <summary>
/// Result of a link operation
/// </summary>
public class LinkResult
{
    /// <summary>
    /// Whether the linking was successful
    /// False if blocked by hooks or required input was not provided
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The context that was processed during linking
    /// Includes all hooks and their block/approval status
    /// Serialized to Entry format for REST API responses
    /// </summary>
    public LinkingContext Context { get; set; }
}

/// <summary>
/// Context for an unlinking operation
/// Modified by hook plugins during GetUnlinkingRequirementsAsync to inject validation and requirements
/// </summary>
public class UnlinkingContext
{
    /// <summary>
    /// Whether this unlinking operation is manual or automatic
    /// </summary>
    public bool IsManual { get; set; }

    /// <summary>
    /// Collection of hooks to apply to unlinking
    /// Populated by hook plugins during GetUnlinkingRequirementsAsync phase
    /// </summary>
    public ICollection<LinkingHook> Hooks { get; } = new List<LinkingHook>();

    /// <summary>
    /// Convenience flag: true if any hook has IsBlocking = true
    /// </summary>
    public bool IsBlocked => Hooks.Any(h => h.IsBlocking);

    /// <summary>
    /// Optional custom metadata for audit trail
    /// </summary>
    public object? CustomData { get; set; }
}

/// <summary>
/// Result of an unlink operation
/// </summary>
public class UnlinkResult
{
    /// <summary>
    /// Whether the unlinking was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The context that was processed
    /// Serialized to Entry format for REST API responses
    /// </summary>
    public UnlinkingContext Context { get; set; }
}

/// <summary>
/// A validation/side-effect hook attached to a linking operation
/// Injected by hook plugins during the requirements phase (BeginLinking* event)
/// </summary>
public class LinkingHook
{
    /// <summary>
    /// Name/identifier of the hook for audit purposes
    /// Example: "CriticalAreaAcknowledgment", "IncompatibleProductType"
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// If true, this hook blocks the linking operation
    /// Linking fails if any hook has IsBlocking = true
    /// </summary>
    public bool IsBlocking { get; set; }

    /// <summary>
    /// Optional message explaining the block or providing context
    /// Example: "Critical area - operator acknowledgment required"
    /// Displayed to the user when the linking is blocked or input is required
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// Hook-specific data serving multiple purposes:
    /// - User input requirement: e.g., OperatorAcknowledgment object for the UI to fill
    /// - Tracing/context information: e.g., readonly metadata for automatic processing
    /// - Prefilled context: e.g., readonly objects providing business logic context
    ///
    /// This field is mutable by the hook and by the caller between phases.
    /// Serialized to Entry format only at the REST API boundary for UI consumption/modification.
    /// When returned from GetLinkingRequirementsAsync, contains uninitialized user input objects.
    /// When passed to LinkToOrderAsync phase 2, contains user-provided values.
    /// </summary>
    public object? Data { get; set; }
}
```

---

## Lineage & History

```csharp
/// <summary>
/// Represents container genealogy from split/merge operations
/// </summary>
public class ContainerLineageEvent
{
    /// <summary>
    /// Type of operation creating this lineage: Register, Split, Merge
    /// </summary>
    public ContainerLineageOperation Operation { get; set; }

    /// <summary>
    /// Source container(s)
    /// For Register: empty; for Split: single source; for Merge: multiple sources
    /// </summary>
    public IMaterialContainer[] SourceContainers { get; set; }

    /// <summary>
    /// Destination container(s)
    /// For Register: single container; for Split: multiple; for Merge: single
    /// </summary>
    public IMaterialContainer[] DestinationContainers { get; set; }

    /// <summary>
    /// Quantity transferred in this operation
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// When this lineage operation occurred
    /// Provides timestamped traceability
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Custom data to include in the record
    /// </summary>
    public object? CustomData { get; set; }
}

public enum ContainerLineageOperation
{
    /// <summary>
    /// Initial registration of a container
    /// </summary>
    Register,

    /// <summary>
    /// Container unregistered and removed from system
    /// </summary>
    Unregister,

    /// <summary>
    /// One container split into multiple
    /// </summary>
    Split,

    /// <summary>
    /// Multiple containers merged into one
    /// </summary>
    Merge
}

/// <summary>
/// A timestamped record of a container lifecycle event or material operation
/// Base class for specific record types (linking, unlinking, lineage, etc.)
/// </summary>
public abstract class ContainerRecord
{
    /// <summary>
    /// Incrementing unique identifier for this record in the sequence
    /// </summary>
    public long RecordId { get; set; }

    /// <summary>
    /// When this event/operation occurred
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// The container involved in this operation
    /// </summary>
    public IMaterialContainer Container { get; set; }

    /// <summary>
    /// Type of action: Register, LinkedOrder, UnlinkedOrder, LinkedOperator, etc.
    /// Also includes lineage operations: Split, Merge, Unregister
    /// </summary>
    public ContainerRecordActionType ActionType { get; set; }
}

public enum ContainerRecordActionType
{
    /// <summary>
    /// Container registered
    /// </summary>
    Register,

    /// <summary>
    /// Container unregistered and removed
    /// </summary>
    Unregister,

    /// <summary>
    /// Container linked to an order
    /// </summary>
    LinkedOrder,

    /// <summary>
    /// Container unlinked from an order
    /// </summary>
    UnlinkedOrder,

    /// <summary>
    /// Container linked to an operator
    /// </summary>
    LinkedOperator,

    /// <summary>
    /// Container unlinked from an operator
    /// </summary>
    UnlinkedOperator,

    /// <summary>
    /// Container linked to a product
    /// </summary>
    LinkedProduct,

    /// <summary>
    /// Container unlinked from a product
    /// </summary>
    UnlinkedProduct,

    /// <summary>
    /// Container moved to a different storage location
    /// </summary>
    StorageChanged,

    /// <summary>
    /// Quantity changed (filling level)
    /// </summary>
    QuantityChanged,

    /// <summary>
    /// Container split into multiple containers
    /// </summary>
    Split,

    /// <summary>
    /// Multiple containers merged into one
    /// </summary>
    Merge
}

/// <summary>
/// Query criteria for container records
/// </summary>
public class MaterialRecordsQueryCriteria
{
    /// <summary>
    /// Filter by container resource ID
    /// </summary>
    public long? ContainerResourceId { get; set; }

    /// <summary>
    /// Filter by action type
    /// </summary>
    public ContainerRecordActionType? ActionType { get; set; }

    /// <summary>
    /// Filter events after this time
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// Filter events before this time
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Filter by storage location resource ID
    /// </summary>
    public long? StorageResourceId { get; set; }

    /// <summary>
    /// Paging support
    /// </summary>
    public int PageSize { get; set; } = 100;
    public int PageNumber { get; set; } = 0;
}
```

---

## Events

```csharp
public class ContainerRegisteredEventArgs : EventArgs
{
    public IMaterialContainer Container { get; set; }
}

public class ContainerDeletedEventArgs : EventArgs
{
    public IMaterialContainer Container { get; set; }
}

public class ContainerChangedEventArgs : EventArgs
{
    /// <summary>
    /// The container that changed
    /// </summary>
    public IMaterialContainer Container { get; set; }

    /// <summary>
    /// Type of change that occurred
    /// </summary>
    public ContainerChangeType ChangeType { get; set; }
}

public enum ContainerChangeType
{
    /// <summary>
    /// Container linked to an order
    /// </summary>
    OrderLinked,

    /// <summary>
    /// Container unlinked from an order
    /// </summary>
    OrderUnlinked,

    /// <summary>
    /// Container linked to an operator
    /// </summary>
    OperatorLinked,

    /// <summary>
    /// Container unlinked from an operator
    /// </summary>
    OperatorUnlinked,

    /// <summary>
    /// Container linked to a product
    /// </summary>
    ProductLinked,

    /// <summary>
    /// Container unlinked from a product
    /// </summary>
    ProductUnlinked,

    /// <summary>
    /// Container moved to a different storage/location
    /// </summary>
    StorageChanged,

    /// <summary>
    /// Container filling level changed
    /// </summary>
    FillingLevelChanged
}

public class LinkingOrderEventArgs : EventArgs
{
    public IOrderMaterialContainer Container { get; set; }
    public Order Order { get; set; }
    public LinkingContext Context { get; set; }
}

public class LinkedOrderEventArgs : EventArgs
{
    public IOrderMaterialContainer Container { get; set; }
    public Order Order { get; set; }
}

public class UnlinkingOrderEventArgs : EventArgs
{
    public IOrderMaterialContainer Container { get; set; }
    public UnlinkingContext Context { get; set; }
}

public class UnlinkedOrderEventArgs : EventArgs
{
    public IOrderMaterialContainer Container { get; set; }
    public Order PreviousOrder { get; set; }
}

// Similar EventArgs for Operator and Product linking
// LinkingOperatorEventArgs, LinkedOperatorEventArgs, UnlinkingOperatorEventArgs, UnlinkedOperatorEventArgs
// LinkingProductEventArgs, LinkedProductEventArgs, UnlinkingProductEventArgs, UnlinkedProductEventArgs
```

---

## Hook Plugin System

### Architecture (ADR: Material Manager Hook Plugin System)

Hook plugins are registered in the dependency injection container with the `[Plugin]` attribute. The execution model is:

1. **Event-driven subscription**: Container resource fires `BeginLinking*` events
2. **Internal orchestration**: Material Manager module's internal listener subscribes to these events
3. **Config-based instantiation**: On event, the listener uses a factory to create transient hook instances from `ModuleConfig`
4. **Sequential execution**: Hooks execute in configuration order (sorted by priority)
5. **Exception handling**: Exceptions are caught and logged; hooks fail gracefully without interrupting others
6. **Context mutation**: All hooks operate on the same context, mutations are shared across all plugins

### Hook Plugin Interfaces

Hook plugins are strongly typed by the domain construct they handle (Order, Operator, Product). Each implements `IConfiguredModulePlugin<TConfig>` for factory-based creation.

#### Order Link Hook Interface

```csharp
/// <summary>
/// Plugin interface for order-linking validations and side-effects
/// Registered with [Plugin] attribute and discovered via IConfigBasedComponentSelector factory
/// Hooks run only during the requirements phase (GetLinkingRequirementsAsync)
/// </summary>
public interface IOrderMaterialLinkHook
{
    /// <summary>
    /// Called when GetLinkingRequirementsAsync fires BeginLinkingOrder event
    /// Hook can inspect container and order, then mutate context to inject validation/requirements
    /// Exceptions are caught and logged; hook execution continues
    /// </summary>
    Task OnBeginLinkingOrderAsync(
        IOrderMaterialContainer container,
        Order order,
        LinkingContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Called when GetUnlinkingRequirementsAsync fires BeginUnlinkingOrder event
    /// Hook can block the unlinking or add audit requirements
    /// </summary>
    Task OnBeginUnlinkingOrderAsync(
        IOrderMaterialContainer container,
        UnlinkingContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Optional hook: Called after successful LinkToOrderAsync for side-effects
    /// Examples: audit logging, external system notification, state machine transitions
    /// NOT executed during requirements phase
    /// </summary>
    Task OnOrderLinkedAsync(
        IOrderMaterialContainer container,
        Order order,
        CancellationToken ct = default) => Task.CompletedTask;

    /// <summary>
    /// Optional hook: Called after successful UnlinkFromOrderAsync for side-effects
    /// </summary>
    Task OnOrderUnlinkedAsync(
        IOrderMaterialContainer container,
        CancellationToken ct = default) => Task.CompletedTask;
}

/// <summary>
/// Same pattern for operator and product hooks
/// </summary>
public interface IOperatorMaterialLinkHook { /* ... */ }
public interface IProductMaterialLinkHook { /* ... */ }
```

### Hook Configuration

Hooks are configured in `ModuleConfig` with plugin-specific settings and priorities:

```csharp
[DataContract]
public class ModuleConfig : ConfigBase
{
    [DataMember]
    [PluginConfigs(typeof(IOrderMaterialLinkHook))]
    public List<OrderMaterialLinkHookConfig> OrderLinkingHooks { get; set; } = new();

    [DataMember]
    [PluginConfigs(typeof(IOperatorMaterialLinkHook))]
    public List<OperatorMaterialLinkHookConfig> OperatorLinkingHooks { get; set; } = new();

    [DataMember]
    [PluginConfigs(typeof(IProductMaterialLinkHook))]
    public List<ProductMaterialLinkHookConfig> ProductLinkingHooks { get; set; } = new();
}
```

### Hook Configuration Base

```csharp
[DataContract]
public abstract class MaterialLinkHookConfigBase : IPluginConfig
{
    [DataMember]
    public string PluginName { get; set; }

    /// <summary>
    /// Execution priority for this hook instance (0-100)
    /// Lower numbers execute first; allows hooks to reason about state
    /// ADR: Material Manager Hook Priorities
    /// </summary>
    [DataMember]
    public int Priority { get; set; } = 50;
}

[DataContract]
public class OrderMaterialLinkHookConfig : MaterialLinkHookConfigBase
{
    /// <summary>
    /// Example: Critical storage locations that require acknowledgment
    /// Can use custom PossibleValuesAttribute to list available IContainerStorage resources
    /// </summary>
    [DataMember]
    public List<long> CriticalAreaStorageResourceIds { get; set; } = new();
}
```

### Example Hook Implementation

```csharp
[Plugin(LifeCycle.Transient, typeof(IOrderMaterialLinkHook), Name = nameof(CriticalAreaAcknowledgmentHook))]
public class CriticalAreaAcknowledgmentHook : IOrderMaterialLinkHook, IConfiguredModulePlugin<OrderMaterialLinkHookConfig>
{
    private OrderMaterialLinkHookConfig _config;

    /// <summary>
    /// Factory injection: called by IConfigBasedComponentSelector before use
    /// </summary>
    public void Initialize(OrderMaterialLinkHookConfig config)
    {
        _config = config;
    }

    public async Task OnBeginLinkingOrderAsync(
        IOrderMaterialContainer container,
        Order order,
        LinkingContext context,
        CancellationToken ct = default)
    {
        // Check if this is a critical area based on configuration
        if (_config.CriticalAreaStorageResourceIds.Contains(container.Storage?.Id ?? 0) && context.IsManual)
        {
            // Require operator acknowledgment
            var hook = new LinkingHook
            {
                Name = nameof(CriticalAreaAcknowledgmentHook),
                IsBlocking = false,  // Initially not blocking
                Remark = "Critical area detected - operator acknowledgment required",
                Data = new OperatorAcknowledgment()  // Empty object for UI to fill
            };

            context.Hooks.Add(hook);
        }
    }

    public Task OnBeginUnlinkingOrderAsync(
        IOrderMaterialContainer container,
        UnlinkingContext context,
        CancellationToken ct = default)
    {
        // Similar pattern for unlinking
        return Task.CompletedTask;
    }

    public Task OnOrderLinkedAsync(
        IOrderMaterialContainer container,
        Order order,
        CancellationToken ct = default)
    {
        // Side-effect: audit trail or external notification
        return Task.CompletedTask;
    }

    public Task OnOrderUnlinkedAsync(
        IOrderMaterialContainer container,
        CancellationToken ct = default)
    {
        // Side-effect: cleanup or notification
        return Task.CompletedTask;
    }
}
```

### Data Objects for Hook Input

Hook plugins can use hook.Data to request input from the user:

```csharp
[DataContract]
public class OperatorAcknowledgment
{
    /// <summary>
    /// Operator identifier/badge code
    /// Set by UI after user provides acknowledgment
    /// </summary>
    [DataMember]
    public string OperatorId { get; set; }

    /// <summary>
    /// Optional reason for acknowledgment
    /// </summary>
    [DataMember]
    public string? Reason { get; set; }

    /// <summary>
    /// Timestamp added by the system
    /// </summary>
    [DataMember]
    public DateTime AcknowledgedAt { get; set; } = DateTime.UtcNow;
}
```

---

## Integration with Other Modules

### Orders Module
- Container links to `Order` instances
- Container has reference to `LinkedOrder`

### Operators Module
- Container links to `Operator` instances
- Container has reference to `LinkedOperator`

### Products Module
- Container links to `ProductType` instances
- Container has reference to `LinkedProduct`
- Container `ICapabilities` match against product requirements

### Resources Module
- Container implements `IResource`
- Storage locations implement `IContainerStorage`
- Bidirectional references for context embedding

---

## Two-Phase Linking Flow (ADR: Material Manager Linking Flow)

Linking operations use a two-phase pattern to separate validation from execution. This enables asynchronous hook processing and user interaction:

### Phase 1: Collect Requirements

```csharp
// Get hook requirements and validation blocks
var requirements = await container.GetLinkingRequirementsAsync(order, ct);

if (!requirements.Success)
{
    // Linking blocked by hooks, or input required
    foreach (var hook in requirements.Context.Hooks.Where(h => h.IsBlocking))
    {
        Console.WriteLine($"Blocked by: {hook.Name} - {hook.Remark}");
    }

    var inputHooks = requirements.Context.Hooks.Where(h => h.Data != null).ToList();
    if (inputHooks.Any())
    {
        // Serialize hooks[x].Data to Entry format, send to UI for user input
        var hookData = EntryConvert.EncodeObject(inputHooks);
        await userInterface.RequestInputAsync(hookData);

        // User provides input, callbacks populate requirements.Context.Hooks[x].Data
    }
}
```

### Phase 2: Execute Linking

```csharp
// After requirements satisfied and input collected, execute the link
var result = await container.LinkToOrderAsync(order, requirements.Context, ct);

if (result.Success)
{
    // Linking complete, LinkedOrder event fired, audit trail recorded
    Console.WriteLine($"Container {container.Name} linked to {order.Number}");
}
```

### Key Design Decisions

- **Hooks run only in Phase 1**: No re-evaluation during Phase 2 execution
- **Context is mutable**: All hooks work on the same context object; mutations are shared
- **Data field is flexible**: Can contain user input, tracing info, readonly context, or prefilled metadata
- **Sequential execution by priority**: Hooks execute in configuration order, allowing state inspection
- **Exception handling**: Hook exceptions are caught and logged; other hooks continue

---

## Entry Format & REST API Serialization

Entry Format serialization is used **only at the REST API boundary**, not within business models:

- **Business models** (facade, interfaces, contexts) use strongly typed C# objects
- **REST endpoints** receive `LinkingContext` and convert `LinkingHook.Data` objects to `Entry` format using `EntryConvert.EncodeObject()`
- **UI interaction**: Hook data is serialized to Entry format, sent to UI for user input, then deserialized back
- **Audit trail** stores `Entry?` representation of hook data for full reconstruction and traceability
- **Hook plugin implementation** works only with strongly typed C# objects (Entry Format is transparent to hooks)

This separation maintains type safety in code while providing flexible serialization for UI/persistence.

---

## Cascade Unlinking on Container Deletion

When a container resource is deleted via ResourceManagement:

1. ResourceManagement fires a deletion event
2. MaterialManagement subscribes to this event
3. Material Manager **automatically cascades unlinking** without triggering hooks
   - All links are severed immediately
   - Audit trail records this as automatic cascade operation
   - Hook plugins are NOT invoked (bypass validation)

This ensures containers can be safely deleted from the system without getting stuck on hook validations.

---

## Extension Method Helpers (Future)

While not part of the core public API, extension methods can be added later to simplify common operations:

```csharp
// Example (not included in initial API)
public static Task LinkToOrderByNumberAsync(
    this IOrderMaterialContainer container,
    string orderNumber,
    IOrderManagement orderManagement,
    LinkingContext context,
    CancellationToken ct = default)
{
    // Resolve order by number, then call LinkToOrderAsync
}
```

---
