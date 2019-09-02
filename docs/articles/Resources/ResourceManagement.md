---
uid: ResourceManagement
---
# Resource management

The new resources management also changes the way other modules interact with resources. The API offers direct access to all of the resource methods, properties and events as long as they are declared through an interface derived from IPublicResource. Public resources with NullCapabilities, the null object implementation of ICapabilities, are considered non-public. Users of the resource management can resolve instances by type, name or capabilities over the facade.

```cs
// Get by name
var res = Facade.GetResource<IMyResource>("Some" );
// Get by capabilities
res = Facade . GetResource<IMyResource>(new MyCapabilities ());
// Get all
var all = Facade.GetAllResources<IMyResource >();
// Get filtered
all = Facade.GetAllResources<IMyResource>(r => r . Id > 42);
```

The API and interaction is decoupled from the resource graph. The user code resolves a single instance or list of instances to work with, but never the whole graph. This seems like a limitation at ﬁrst, but it ensures structure independent user code limited to the interface of each resource and the capabilities to distinguish instances of the same interface. For example in the control system the underlying structure can be as simple as a single, manually operated cell or fully automated assembly line with stations, third party devices, buffers and four redundant packaging places. In both cases the ActivityDispatcher, the component starting activities in cells as a consequence of the workplan, always sees an enumeration of ICell.
Because of the restriction to use components of the internal container outside of the module the resource management applies the proxy pattern to provide access to the resources API while simultaneously hiding the resource instance from the user.

![Resource proxy pattern](images\ResourceProxyPattern.png)

The proxy types implement the same interfaces as the resource type they are representing. The proxy forwards all calls to the `Target` and forwards events of the resource to listeners after replacing the sender object with itself. If methods or properties return a resource or collection of resources the proxy converts those on the ﬂy to proxies as well. When the resource management is shutdown it calls `Detach` on the proxy to release the reference to the `Target`. To spare the developers the additional effort of creating a matching proxy class for each resource, the proxy classes are created on demand at runtime as classes derived from `ResourceProxy`. When another module resolves a resource instance over the facade the proxy builder determines all interfaces that resource implements and creates a proxy type that offers the same interfaces. Next the proxy builder moves up the type tree looking for the least speciﬁc base type that implements the same number of interfaces. That way the resulting proxy type can be used for all derived types that only customize the existing behavior. In the ﬁgure above instances of ResourceA and ResourceB would use a different proxy type, but instances of ResourceC would be represented by ProxyB as well. Once the proxy type is created, it is instantiated for the initially requested target. The type and instance are stored within the resource management. All access to the same resource instance is handled by the same proxy to save memory and enable object reference comparison outside the resource management. The stored types are used to create proxies for instances of the same or compatible resource type.
