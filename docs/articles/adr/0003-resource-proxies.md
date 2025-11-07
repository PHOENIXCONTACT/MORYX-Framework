# ADR-003: Export `ResourceProxy` out of Facade

**Date:** 2025-11-07 \
**Status:** Accepted \
**Context:** Resources / AbstractionLayer 5 / MORYX 6+ Projects

The `ResourceProxy` will be exported out of the module facade to enable interaction with resource APIs while maintaining the internal lifecycle and encapsulation principles defined by the MORYX architecture.

In the MORYX architecture, each module contains its own internal dependency injection (DI) container. This design prevents direct use of internal module components outside the module’s facade. To provide controlled access to resource instances from outside the module, the **ResourceManagement** module applies the **Proxy Pattern**.

This pattern allows external consumers to interact with resource interfaces without directly referencing or managing the actual resource instances, thereby preserving the module’s lifecycle integrity.

## Motivation

1. **Keep Module Lifecycle**
   - Ensures that the module’s lifecycle remains self-contained and stable.
   - When a module restarts or shuts down, all internal components are safely disposed.
   - The proxy detaches from the target resource during shutdown (`Detach`), preventing stale references.

2. **Automatic Proxy Generator**
   - Reduces developer overhead by generating proxy classes dynamically at runtime.
   - When a resource instance is resolved via the module’s facade, the system determines all interfaces implemented by the resource and creates a matching proxy type that exposes the same interfaces.
   - This approach simplifies extensibility and avoids repetitive boilerplate proxy code.

## Exceptions

The ResourceManagement-facade will always create a proxy type for each resource type with the following exceptions:

- Modification methods like `IResourceManagement.Create`, `IResourceManagement.Read`, `IResourceManagement.Modify`: They provide the original resource instance to have full access to it. The resource should only be used in the provided accessor-delegate and should never be exported from that.
- Method `IResourceManagement.GetAllResources`: Used for endpoint-controllers to export all properties from the resource e.g. internal properties or methods marked with `EntrySerialize` which are not part of any interface.

## Consequences

- Developers can work with a resource proxy as if it were the original resource.
- Proxies currently only implement interfaces derived from `IResource`.
  Interfaces unrelated to `IResource` must be explicitly marked with the `[ResourceAvailableAs(typeof(interface))]` attribute.
- This decision ensures safe cross-module interaction while maintaining isolation between modules.


## References

- [ResourceProxy Documentation](/docs/articles/framework/ResourceProxy.md)
