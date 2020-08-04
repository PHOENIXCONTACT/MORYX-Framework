---
uid: InteractionResource
---
# InteractionResource

[InteractionResources](xref:Moryx.Resources.Interaction.InteractionResource%601>) are another type of [Resource](xref:Moryx.AbstractionLayer.Resources.Resource). 
They host web services in addition to the one hosted by the `ResourceInteractionHost`, that hosts the endpoint for the standard resource UI.
One specialty of interaction resources is their different registration shown in the code snippet below. 
The second parameter in the attribute deﬁnes an additional service this resource is registered for besides IResource. 
It also uses the life cycle `Singleton` instead of the standard `Transient` life cycle.
Because of the additional service and modiﬁed life cycle the container managed service instances can declare a dependency on their host.
Because of the singleton instance they can use it as an entry point into the [Resource Graph](../ResourceObjectGraph.md). 
This enables developers to build special interfaces to interact with resources that are not covered by the standard web-service. 
As a side effect of this approach it is not possible to conﬁgure two instances of the same service at different endpoints.

```cs
public interface ISomeInteraction : IResource
{
    long GetId( string name);
}

[ResourceRegistration(nameof(SomeInteraction)), typeof(ISomeInteraction)]
public class SomeInteraction : InteractionResource<ISomeService>, ISomeInteraction
{
    [ResourceReference(ResourceRelationType.Extension, Role = ReferenceRole.Source]
    public IReferences<Resource> Users { get; set; }

    public long GetId(string name)
    {
        return Users.First(u => u.Name == name).Id;
    }
}

[Plugin(LifeCycle.Transient, typeof(ISomeService)]
internal class SomeService : ISomeService
{
    // Injected dependency 
    public ISomeInteraction Host { get; set; }

    public long GetId( string name) { return Host.GetId(name); }
}
```
