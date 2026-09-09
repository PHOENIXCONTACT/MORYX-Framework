---
uid: ResourceProxies
---
# Resource Proxies

Since the MORYX architecture includes an internal DI container for each module, resource instances cannot be directly exposed outside the module's [Facade](/src/Moryx.Resources.Management/Facades/ResourceManagementFacade.cs). The resource management applies the **proxy pattern** to provide controlled access to resource APIs while preserving module lifecycle integrity. See [ADR: Export ResourceProxy out of Facade](/docs/adr/003-resource-proxies.md) for the architectural decision behind this pattern.

The proxy types implement the same interfaces as the resource type they represent. All calls are forwarded to the real resource, and events are forwarded to listeners with the sender replaced by the proxy. If methods or properties return resources or collections of resources, the proxy converts those on the fly to proxies as well.

## Proxy creation

Proxy classes are generated on demand at runtime using [Castle.DynamicProxy](https://www.castleproject.org/projects/dynamicproxy/). When another module resolves a resource instance over the facade, the [ResourceProxyBuilder](/src/Moryx.Resources.Management/Resources/Proxies/ResourceProxyBuilder.cs) determines all relevant interfaces and creates an interface proxy.

## Proxy Cache

Each resource has exactly one proxy instance, stored in the [ResourceTypeController](/src/Moryx.Resources.Management/Resources/ResourceTypeController.cs). The cache avoids creating duplicate proxies for the same resource, which saves memory. It also enables reference equality checks, so `proxy1 == proxy2` works as expected when both reference the same resource.

## Interface Discovery

The proxy implements all relevant interfaces of the resource type:

1. Public interfaces derived from `IResource` (excluding `IResource` itself)
2. Interfaces declared via `[ResourceAvailableAs(typeof(IMyInterface))]` (including non-`IResource` interfaces)
3. Base interfaces of any already-relevant interface

Derived resource types that do not add new public interfaces reuse the base type's proxy type. For example, if `DerivedResource` extends `SimpleResource` without adding new interfaces, both share the same proxy type.

## Detach Lifecycle

When the resource management shuts down, all proxies are detached. After detach, accessing properties or calling methods on the proxy throws `ProxyDetachedException`.

## Unsafe Bypass Methods

Certain facade methods skip proxying for cases that require direct access:

| Method | Use Case |
|--------|----------|
| `CreateUnsafeAsync` | Raw instance access during resource creation |
| `ReadUnsafe` | Raw instance access for reading |
| `ModifyUnsafeAsync` | Raw instance access during modification |
| `GetResourcesUnsafe` | Endpoint controllers needing full property access (e.g. `EntrySerialize`) |

The resource should only be used within the provided accessor delegate and must not be stored or exported.

## Supported Features

| Feature | Details |
|---------|---------|
| Property forwarding | Read/write properties declared on any proxied interface |
| Method forwarding | Instance methods including overloaded methods |
| Explicit interface implementations | Properties, methods, and events implemented explicitly |
| Generic methods | Methods with type parameters (e.g. `T Get<T>()`) |
| Events (`EventHandler` / `EventHandler<T>`) | Forwarded with sender replaced by proxy |
| Same-named events on different interfaces | Each interface event is wired and raised independently |
| Resource reference conversion | Return values and event args of type `IResource` or `IEnumerable<IResource>` are automatically wrapped as proxies |
| Resource argument extraction | Method arguments of type `IResource` are unwrapped from proxies before forwarding |
| Non-`IResource` interfaces | Supported via `[ResourceAvailableAs(typeof(IMyInterface))]` attribute |
| Derived type proxy reuse | Derived types that add no new public interfaces reuse the base type's proxy |
| Singleton proxy per resource | One proxy instance per resource, enabling reference equality checks |
| Detach lifecycle | `DetachProxy` unsubscribes all events and releases the target on shutdown |
| `ToString()` | Delegates to the target resource, shows "Detached Proxy" after detach |

## Limitations

| Limitation | Reason |
|------------|--------|
| Custom delegate events are not supported | The event forwarding relies on the `EventHandler(sender, args)` convention for sender replacement and resource argument conversion. |
| Custom attributes are not replicated to the proxy type | Currently not implemented. Castle supports adding attributes via `ProxyGenerationOptions.AdditionalAttributes`, but different attributes produce different proxy types, which would break proxy type reuse for derived resources. |
| `DebuggerDisplayAttribute` does not work | Castle's generated proxy types produce invalid module metadata for debugger display. Use `ToString()` instead, which delegates to the target. |
| Do not rely on proxy type names | The generated proxy type name may change between versions. Do not use `GetType().Name` or `GetType().FullName` on a proxy instance. Only rely on the interfaces implemented by the proxy. |
