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

Since the architecture of Moryx includes an internal DI-Container for each module, it imposes a restriction on using components from inside the container outside of the [Facade](/src/Moryx.Resources.Management/Facades/ResourceManagementFacade.cs).
Because of that, the resource management applies the proxy pattern to provide access to the resources API while simultaneously hiding the resource instance from the user.
For more information on the structure of Moryx look into [this article](/docs/articles/framework/index.md).

## Provided Endpoint

This module provides a REST API for managing resources. See [Resources Endpoint](endpoint.md) for details on available operations and permissions.

## Resource Proxies

![Resource proxy pattern](images/resource-proxy-pattern.png)

The proxy types implement the same interfaces as the resource type they are representing.
The proxy forwards all calls to the `ProxyTarget` and forwards events of the resource to listeners after replacing the sender object with itself.
If methods or properties return a resource or collection of resources the proxy converts those on the ﬂy to proxies as well. When the resource management is shut down it calls `DetachProxy` on the proxy to release the reference to the `ProxyTarget`.
To spare the developers the additional effort of creating a matching proxy class for each resource, the proxy classes are generated on demand at runtime using [Castle.DynamicProxy](https://www.castleproject.org/projects/dynamicproxy/).
When another module resolves a resource instance over the `Facade` the [ResourceProxyBuilder](/src/Moryx.Resources.Management/Resources/Proxies/ResourceProxyBuilder.cs) determines all interfaces that the resource implements and creates an interface proxy that offers the same interfaces.
Castle.DynamicProxy handles proxy type caching internally, reusing generated types for resources that implement the same set of interfaces.
The proxy instance is stored within the [ResourceTypeController](/src/Moryx.Resources.Management/Resources/ResourceTypeController.cs).
All access to the same resource instance is handled by the same proxy to save memory and enable object reference comparison outside the resource management.
