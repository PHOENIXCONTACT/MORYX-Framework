---
uid: Capabilities
---
# Capabilities

[Capabilities](xref:Moryx.AbstractionLayer.Capabilities.ICapabilities) are basically a self description of a *Resource*. In every `Activity` is defined which capabilities are required and these information will be used to find a matching resource to handle the activity. It is also possible that a resource can have multiple capabilities and to be able to handle multiple activities.

## Single Capabilities

Just implement the interface to define a capability like in the example:

```` cs
[DataContract]
public class MyCapabilities : ICapabilities
{
    public bool IsCombined => false;

    public bool ProvidedBy(ICapabilities provided) => provided is MyCapabilities;

    public bool Provides(ICapabilities required) => required is MyCapabilities;

    public IEnumerable<ICapabilities> GetAll()
    {
        yield return this;
    }
}
````

It is also possible to use the base class `ConcreteCapabilities` which reduces the code to the following lines:

```` cs
[DataContract]
public class MyCapabilities : ConcreteCapabilities
{
    protected override bool ProvidedBy(ICapabilities provided) => provided is MyCapabilities;
}
````

If the resource is an AssembleResource then the AssembleCapabilities class should be derived to define the application specific capabilities like in the following example:

```` cs
[DataContract]
public class MyCapabilities : AssembleCapabilities
{
    protected override bool ProvidedBy(ICapabilities provided) => provided is MyCapabilities;
}
````

This looks similar to the derived ConcreteCapabilities but this one can be used for AssembleResources. In All examples you can extend your capabilities with more properties to give the realize are more meaningfull `self description`. For example ScrewingCapabilities. May be there are more than one station which has ScrewingCapabilities but they can handle different screw heads. This could be look like the following example:

```` cs
public enum ScrewHead
{
    Default, 
    Phillips
}

[DataContract]
public class ScrewingCapabilities : AssembleCapabilities
{
    [DataMember]
    public ScrewHead Head { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public ScrewingCapabilities()
    {    
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScrewingCapabilities"/> class.
    /// </summary>
    public ScrewingCapabilities(ScrewHead providedHead)
    {
        Head = providedHead;
    }

    protected override bool ProvidedBy(ICapabilities provided)
    {
        var capabilities = provided as ScrewingCapabilities;
        return capabilities != null && capabilities.Head == Head;
    }
}
````

So it is possible to extend the capabilities with different information to distinct between resources with the same capabilities.

## Multiple Capabilities

A resource can also have multiple capabilities. Therefore the class `CombinedCapabilities` should be used to combine multiple capabilities for a resource. The class CombinedCapabilities implementes also the ICapabilities interface so it is possible to just set the resource capabilities to a list of capabilities like in the following example:

```` cs
// some resource code

public override void Initialize()
{
    base.Initialize();

    Capabilities = new CombinedCapabilities(new List<ICapabilities>
    {
        new MyCapabilities(),
        new ScrewingCapabilities(ScrewHead.Phillips),
        new CoolCapabilities(),
        new HotCapabilities(),
        new LitCapabilities()
    });
}

// some more resource code
````

It is also possible to use multiple capabilities for AssembleResources but in the little different way like in the following example. But remember this works only for AssembleCapabilities:

```` cs
var myStation = EntityCreation.CreateResource(openContext, new AssembleCell
{
    Name = "My Station",
    LocalIdentifier = "Foo Bang Bang",
    AssembleCapabilities = new AssembleCapabilities[]
    {
        new MyCapabilities(),
        new ScrewingCapabilities(ScrewHead.Phillips),
        new CoolCapabilities(),
        new HotCapabilities(),
        new LitCapabilities()
    }
}, parentResource);
````
