---
uid: ResourceManagement
---
# Resource management

The new resources management also changes the way other modules interact with resources.
The API offers direct access to all of the resource methods, properties and events as long as they are declared through an interface derived from [IResource](/src/Moryx.AbstractionLayer/Resources/IResource.cs).
Public resources with [NullCapabilities](/src/Moryx.AbstractionLayer/Capabilities/NullCapabilities.cs), the null object implementation of [ICapabilities](/src/Moryx.AbstractionLayer/Capabilities/ICapabilities.cs), are considered non-public.
Users of the resource management can resolve instances by type, name or capabilities over the [Facade](/src/Moryx.Resources.Management/Facades/ResourceManagementFacade.cs).

```cs
// Get by name
var res = Facade.GetResource<IMyResource>("Some");
// Get by capabilities
res = Facade.GetResource<IMyResource>(new MyCapabilities());
// Get all
var all = Facade.GetResources<IMyResource>();
// Get filtered
all = Facade.GetResources<IMyResource>(r => r.Id > 42);
```

The API and interaction is decoupled from the resource graph. The user code resolves a single instance or list of instances to work with, but never the whole graph.
This seems like a limitation at ﬁrst, but it ensures structure independent user code limited to the interface of each resource and the capabilities to distinguish instances of the same interface.
For example, the underlying system's structure can be as simple as a single, manually operated resource or a completely automatic working system of resources composed of different devices, utility providers, etc.
There could even be an underlying redundant architecture.
In all cases, the module responsible for mapping `Activities` to `Resources` always sees an enumeration of [IResource](/src/Moryx.AbstractionLayer/Resources/IResource.cs).
For more information on the structure of MORYX look into [this article](/docs/articles/framework/index.md).

## Provided Endpoint

This module provides a REST API for managing resources. See [Resources Endpoint](endpoint.md) for details on available operations and permissions.

## Resource Proxies

Since the architecture of MORYX includes an internal DI-Container for each module, resource instances cannot be directly exposed outside the module. The resource management applies the proxy pattern to provide safe access to resource APIs through the facade. See [Resource Proxies](resource-proxies.md) for details on how proxies work, supported features, and limitations.
