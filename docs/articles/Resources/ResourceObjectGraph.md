---
uid: ResourceObjectGraph
---
# Resource object graph

The resource type tree has uniﬁed the different types of plugins within the resource management. The resource graph standardizes their creation, conﬁguration, references and dependencies. Except for the meta components required to construct the type tree, build and manage the resource graph and provide access to the resources all plugins of the resource management are resources. At module startup the resource manager loads all resource and relation entities from the database and for each one creates an instance of the type declared on the entity. It copies all standard properties to the instance and populates the rest from the JSON extension data. Additional properties or ﬁelds of a resource that should be saved to and loaded from the database are decorated with `DataMember` for the NewtonSoft JSON serializer used in the MORYX ecosystem. A big difference between the new resource graph and the previous list of linked resources is its agility at runtime. The current version required resource items as a dynamic data storage for all information that were only temporarily, because resources and their relationships could only be changed over the database. For the changes to take effect it was necessary to restart at least the module. Information stored in resource items were equipped material or the identiﬁer of the product on the workpiece carrier. Thereby resources would represent a static structure and resource items all dynamic information. Resources could read from and write to resource items over a specialized component, the `ResourceItemMapper`. The new resource graph and its Inversion of Control (IoC) approach promoted resource objects to the leading entity. The database is hidden in the background and updated to the current state when necessary. One trigger to update the database entry for a resource is an external event, like the `Save call from the web`-service. Internally resources can trigger updating their current state by calling `RaiseResourceChanged`.

The code example below shows a resource that declares two properties with the `DataMember`-attribute. The second property uses an explicit implementation and the `ResourceChanged`-event.

```cs
public class PropertyDummy : Resource
{
    [DataMember]
    public int TimerMs { get; set; }

    private long _processId;

    [DataMember]
    public long ProcessId
    {
        get { return _processId; }
        set
        {
            if (_processId == value)
                return;
            _processId = value;
            RaiseResourceChanged();
        }
    }
}
```

The ability to make changes at runtime that are automatically saved is not limited to an existing resource instance. It is also possible to change [relations](xref:ResourceRelations) between resources to expand and reduce the resource graph at runtime. To make sure the resource manager knows of the new resource and takes care of its dependencies and reference properties, it is not possible to create new resources naturally through constructor invocation. Instead each resource can use the [IResourceGraph](xref:Moryx.AbstractionLayer.Resources.IResourceGraph) to create and destroy resource instances. Resources access the graph through the inherited `Graph` property. It sets the reference on each instance it loads in the startup phase and those created through the graph interface at runtime. This ensures that dynamic resource creation is not limited to the ﬁrst generation of objects, but works indeﬁnitely. The code below shows the usage of the resource graph. The `Grow` method in the example shows all three different alternatives to instantiate resources with the graph. The statically typed one is the replacement for direct constructor invocation. The type speciﬁed is also the type of the returned object. The other two overloads are similar to the factory pattern and can return different objects speciﬁed by the type string.

This makes object creation more ﬂexible than a constructor and enables subclassing the created resource type. This feature can be combined with the [ResourceTypes](xref:Moryx.AbstractionLayer.Resources.ResourceTypesAttribute) attribute to generate a drop-down box of available resource types for properties or method parameters.

```cs
public class DynamicTree : Resource
{
    [EditorBrowsable, ResourceTypes(typeof(IBranch ))]
    public string BranchType { get; set; }

    public void Grow()
    {
        // Grow statically typed
        IBranch branch = Graph.Instantiate<Branch>();
        // Grow dynamically typed
        branch = Graph.Instantiate<IBranch>(BranchType);
        // Grow dynamic
        Resource child = Graph.Instantiate("SomeType");
    }

    public void Shrink()
    {
        var child = Children.First ();
        // Remove reversible
        Graph.Destroy(child);
        // Remove permanently
        Graph.Destroy(child, true)
    }
}

```

The `Shrink`-method shows the two alternatives for destroying resource instances. The caller can specify whether to remove the object permanently, thereby ﬂagging it as deleted or actually deleting the entry from the database. The call with a single argument is a shortcut for the second one with permanent = false. In both cases the object is removed from the resource graph and all references it occurs in to allow proper garbage collection.