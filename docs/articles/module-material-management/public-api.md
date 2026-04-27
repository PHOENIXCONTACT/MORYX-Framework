---
uid: MaterialManagerPublicAPI
---

# Material Manager Public API

## Overview

The Material Manager module provides infrastructure for managing digital twins of material containers in a cyber-physical system. The public API consists of:

- **IMaterialManagement facade**: Container discovery and registration
- **Resource interfaces**: `IMaterialContainer`, `IOrderMaterialContainer`, `IOperatorMaterialContainer`, `IProductMaterialContainer`
- **Hook plugin system**: Extensible validation and side-effect handling for linking operations
- **Event-driven linking**: Containers emit linking events; hooks can inject validation and side-effects

## Core Design Principles

1. **Containers are Resources**: `IMaterialContainer` extends `IResource` and is part of the physical/digital system
2. **Linking on Resources**: Link operations happen on container resource instances, not via facade
3. **Polymorphic Container Interfaces**: Different interfaces for different linking purposes (orders, operators, products)
4. **Capability-based Constraints**: Container capabilities match product type requirements using MORYX capability system
5. **Event-Driven Hook Processing**: Hooks subscribe to linking events and inject validation/side-effects via context
6. **Full Lineage Preservation**: All material movement (split/merge) is fully traceable

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
    /// Attempt to link this container to an order
    ///
    /// Flow:
    /// 1. Fires BeginLinkingOrder event for hook plugins to modify LinkingContext
    /// 2. Waits for all hook plugins to complete (async)
    /// 3. Checks if any hook has IsBlocking = true
    /// 4. If blocked or input not provided, returns LinkResult with failed status
    /// 5. If successful, performs the link and fires LinkedOrder event
    ///
    /// Returns LinkResult indicating success/failure and any blocking hooks
    /// </summary>
    Task<LinkResult> LinkToOrderAsync(
        Order order,
        LinkingContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Attempt to unlink this container from its currently linked order
    ///
    /// Flow similar to LinkToOrderAsync via BeginUnlinkingOrder event
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
    /// Attempt to link this container to an operator
    /// Flow matches LinkToOrderAsync
    /// </summary>
    Task<LinkResult> LinkToOperatorAsync(
        Operator @operator,
        LinkingContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Attempt to unlink this container from its currently linked operator
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
    /// Attempt to link this container to a product
    /// Flow matches LinkToOrderAsync
    /// </summary>
    Task<LinkResult> LinkToProductAsync(
        ProductType product,
        LinkingContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Attempt to unlink this container from its currently linked product
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
/// Modified by hook plugins during BeginLinking* events to inject validation, side-effects, and UI inputs
/// Entry Format serialization only happens at the REST API boundary
/// </summary>
public class LinkingContext
{
    /// <summary>
    /// Whether this linking operation is manual (initiated by operator) or automatic (workflow)
    /// </summary>
    public bool IsManual { get; set; }

    /// <summary>
    /// Collection of hooks that should be applied to this linking operation
    /// Populated and modified by hook plugins when BeginLinking* event fires
    /// </summary>
    public ICollection<LinkingHook> Hooks { get; } = new List<LinkingHook>();

    /// <summary>
    /// Convenience flag: true if any hook has IsBlocking = true
    /// Checked after all hooks complete to determine success/failure
    /// </summary>
    public bool IsBlocked => Hooks.Any(h => h.IsBlocking);

    /// <summary>
    /// Optional custom data for the operation
    /// Will be serialized to Entry format at REST API boundary only
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
/// </summary>
public class UnlinkingContext
{
    /// <summary>
    /// Whether this unlinking operation is manual or automatic
    /// </summary>
    public bool IsManual { get; set; }

    /// <summary>
    /// Collection of hooks to apply to unlinking
    /// </summary>
    public ICollection<LinkingHook> Hooks { get; } = new List<LinkingHook>();

    /// <summary>
    /// Convenience flag: true if any hook has IsBlocking = true
    /// </summary>
    public bool IsBlocked => Hooks.Any(h => h.IsBlocking);

    /// <summary>
    /// Optional custom data for the audit trail
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
/// Injected by hook plugins when BeginLinking* event fires
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
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// Optional user input required by this hook
    /// Can be any type (e.g., OperatorAcknowledgment)
    /// Serialized to Entry format only at REST API boundary
    /// </summary>
    public object? Input { get; set; }
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

Hook plugins are registered in the dependency injection container with the `[Plugin]` attribute and subscribe to container events. They are strongly typed by the domain construct they handle.

### Order Link Hook Interface

```csharp
/// <summary>
/// Plugin interface for handling order linking hooks
/// Registered via [Plugin] attribute in Module initialization
/// </summary>
public interface IOrderMaterialLinkHook
{
    /// <summary>
    /// Hook name for audit trail identification
    /// </summary>
    string HookName { get; }

    /// <summary>
    /// Called when a BeginLinkingOrder event fires
    /// Hook can inspect the context and inject blocks, side-effects, or input requirements
    /// </summary>
    Task OnBeginLinkingOrderAsync(
        IOrderMaterialContainer container,
        Order order,
        LinkingContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Called when a BeginUnlinkingOrder event fires
    /// </summary>
    Task OnBeginUnlinkingOrderAsync(
        IOrderMaterialContainer container,
        UnlinkingContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Optional: Called after successful linking for side-effects
    /// </summary>
    Task OnOrderLinkedAsync(
        IOrderMaterialContainer container,
        Order order,
        CancellationToken ct = default);

    /// <summary>
    /// Optional: Called after successful unlinking for side-effects
    /// </summary>
    Task OnOrderUnlinkedAsync(
        IOrderMaterialContainer container,
        CancellationToken ct = default);
}

/// <summary>
/// Same pattern for operator and product hooks
/// </summary>
public interface IOperatorMaterialLinkHook { /* ... */ }
public interface IProductMaterialLinkHook { /* ... */ }
```

### Example Hook Implementation

```csharp
[Plugin(LifeCycle.Transient, typeof(IOrderMaterialLinkHook), Name = nameof(CriticalAreaAcknowledgmentHook))]
public class CriticalAreaAcknowledgmentHook : IOrderMaterialLinkHook
{
    public string HookName => nameof(CriticalAreaAcknowledgmentHook);

    public async Task OnBeginLinkingOrderAsync(
        IOrderMaterialContainer container,
        Order order,
        LinkingContext context,
        CancellationToken ct = default)
    {
        // Check if this is a critical area (e.g., based on storage location)
        var isCriticalArea = CheckIfCritical(container.Storage);

        if (isCriticalArea && context.IsManual)
        {
            // Require operator acknowledgment
            var hook = new LinkingHook
            {
                Name = HookName,
                IsBlocking = false, // Will become blocking if not resolved
                Remark = "Critical area - operator acknowledgment required",
                Input = new OperatorAcknowledgment(),
                InputEntry = new Entry { /* ... */ }
            };

            context.Hooks.Add(hook);
        }
    }

    // ... other hook methods
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

## Entry Format & REST API Serialization

Entry Format serialization is used **only at the REST API boundary**, not within business models:

- **Business models** (facade, interfaces, contexts) use strongly typed C# objects
- **REST endpoints** convert these objects to `Entry` format using `EntryConvert.EncodeObject()`
- **Audit trail** stores `Entry?` representation of `LinkingContext/UnlinkingContext` for full reconstruction
- **Hook plugin inputs** (e.g., `OperatorAcknowledgment`) are serialized via Entry Format only when returned to clients

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
