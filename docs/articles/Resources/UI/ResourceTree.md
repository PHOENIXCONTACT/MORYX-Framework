---
uid: ResourceTree
---
# Resource tree

The resource tree on the left, region A, has its origin in the ﬁrst version of the resource management, when `ParentChild` relations were handled specially. In the latest version it is created on the server-side by recursively following the Children-relation. Below the resource tree are two buttons, to add and remove resources. When selecting a resource as parent to add a resource to, a dialog opens shown below. The user then selects the type of resource to add. Previously the displayed type tree would contain all types found in the servers type table. The user needed to know which types were actually installed as plugins and which of those could be used as children of the selected resource.
The improved version of the web-service parses the reference properties, including the Children-property and its overrides, and extracts the resource type. It then proceeds by iterating over the type tree looking for all types that are assignable to the propertytype. All matches and their derived types are included in the response. That way users can only add resources that are actually compatible, eliminating the risk of misconﬁguration. By default when creating an instance with the dialog the resource type is instantiated by the resource creator. If the creation of a resource requires parameters or it can be created in different ways, developers can deﬁne constructors. Two examples for resource constructors are shown in snippet below.

![Add resource](images\AddResource.png)

```cs
public class MyResource : Resource
{
    [ResourceConstructor(IsDefault = true)]
    public void DefaultConstructor()
    {
        Named("Test");
    }

    [ResourceConstructor] 
    public void Named( string name)
    {
        Name = name;
    }
}
```

Because of the IoC approach and C# inability to split object creation and constructor invocation, resource constructors are implemented as methods with the [ResourceConstructor](xref:Marvin.AbstractionLayer.Resources.ResourceConstructorAttribute)-attribute. That way the resource manager can create the instance and wire all framework components and reference properties before invoking the constructor method. This makes it possible for developers to use the full feature set of the `AL` within the constructor.
