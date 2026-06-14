---
uid: MaterialManager
---
# MaterialManager

## Feature Specifications

Feature specifications for intended behavior and provided functionality of the module can be found in the [requirements documentation](./requirements.md).
Reviewing the supplemantary [diagram of material flow](./material-flow.md) through a MORYX system provides further insights into the intends of this module.

## Provided Facades

This module exports no facade.

## Referenced facades

 Plugin API | Start Dependency | Optional | Usage
-----------|------------------|----------|------
`IResourceManagement`|Yes|No|??
`IProductManagement`|Yes|No|??

## Used DataModels

This module uses no data model.

## Structure

### Moryx.Material
Base package with public API of the module as well as base types and interfaces.

#### Public API (Moryx.Material)

- `IMaterialContainer` (interface)
  - Abstraction for containers that hold material; defines host reference, content metadata, quantity/unit, lifecycle state, and change notifications.
  - Source: [src/Moryx.Material/IMaterialContainer.cs](../../../src/Moryx.Material/IMaterialContainer.cs)
- `MaterialContainer` (abstract base class)
  - Resource base implementing common container behavior, change events, and lifecycle state handling.
  - State machine details: see [Architecture](./architecture.md#container-state-machine)
  - Source: [src/Moryx.Material/MaterialContainer.cs](../../../src/Moryx.Material/MaterialContainer.cs)
- `BasicMaterialContainer` (concrete base)
  - Ready-to-use container derived from MaterialContainer for simple scenarios or as a starting point for custom types.
  - Source: [src/Moryx.Material/BasicMaterialContainer.cs](../../../src/Moryx.Material/BasicMaterialContainer.cs)

## Frequently Asked Questions

### How do I register a new material container to the MORYX system from within a resource?

Just create a `MaterialContainer` of the desired type within your resource, execute the resource constructor taking a the container's information, and save it on the `ResourceGraph`.

```csharp
var registeredContainer = Graph.Instantiate<MaterialContainer>().Configure(c => c.With(
    // identity: new YourContainerIdentityType(),
    material: "Some Material",
    quantity: 42,
    unit: "Some Unit"
));
await Graph.SaveAsync(requestedContainer, cancellationToken);
```

### How do I request material from within a resource?

Just create a `MaterialContainer` of the desired type in your resource, execute the resource constructor taking a `MaterialRequest`, and save it on the `ResourceGraph`.

```csharp
var materialRequest = new MaterialRequest()
{
    Id = "Some Id",
    Material = "Some Material",
    RequestedQuantity = 42,
    Unit = "Some Unit",
    ExpectedArrival = DateTime.UtcNow,
    // ContainerIdentity = new YourContainerIdentityType()
};
var requestedContainer = Graph.Instantiate<MaterialContainer>().Configure(c => c.With(materialRequest));
await Graph.SaveAsync(requestedContainer, cancellationToken);
```
