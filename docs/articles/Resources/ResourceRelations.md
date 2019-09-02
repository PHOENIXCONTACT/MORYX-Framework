---
uid: ResourceRelations
---
# Resource relations

Resources only declare the relations to other resources as auto-properties and decorate it with an attribute to specify relation type and optionally role and name. The relation type is deﬁned through an enum of different relations like parent and child or referencing a driver. The role refers to the role of the referenced resource in the relationship and can be either `Source` or `Target` where `Target` is the default. The Source role is required for a bidirectional reference. The name of the relationship can be used to distinguish between two references of the same type or to declare a custom reference. After the [resource manager](xref:ResourceManagement) instantiated a resource it ﬁlters all reference properties using reﬂection. For each of those properties it reads the attributes values and tries to ﬁnd a match in the relation entities loaded from the database. Depending on the role it will use either the source or target id of the relation to fetch the instance of the referenced resource. For a single reference the instance is assigned to the property and otherwise it is added to the references collection. When a resource is saved the resource manager iterates over the same reference properties. For each property it reads the attribute deﬁnition and current value. Based on the attribute it then tries to fetch the current entity. Depending on value and entity lookup result it will either create a missing entity, update an existing one or remove a no longer required one.

```cs
public class ReferenceSample : Resource
{
    [ResourceReference(ResourceRelationType.Extension), Required]
    public ISomeResource Other { get; set; }

    [ResourceReference(ResourceRelationType.Parts, AutoSave = true)]
    public IReferences<IPart> Parts { get ; set ; }
}
```

The code example above shows how to declare references to other resources. References to a collection of other resources must be of type [IReferences](xref:Marvin.AbstractionLayer.Resources.IReferences`1). The specialized type is necessary for the resource manager. Collections can optionally be decorated with `AutoSave` = true to enable automatic saving of modifications to the database. While `RaiseResourceChanged` saves the entire resource and all its references, `AutoSave` raises an event that only saves the new state of the collection. The [Resource](xref:Marvin.AbstractionLayer.Resources.Resource) base class declares two relations - `Parent` and `Children`. Both are of relation type ParentChild. The reference to the parent uses the attributes role property to identify the reference as the Source in the relationship and distinguish it from the `Children`.

## Required References

Resources can declare references as `Required` for their functionality as shown in the example above. Required references **must** be set to save a resource, otherwise the `ResourceLinker` will throw a `ValidationException`. The information is also transmitted to the client, which visualizes and validates the reference **before** transmitting the model to the server. 

## Reference Override

If a resource is derived from a base type that already declares a certain relation type it is possible to override this references resource type by declaring a new property with the [ReferenceOverride](xref:Marvin.AbstractionLayer.Resources.ReferenceOverrideAttribute) attribute. A common use case shown in the example below is to override the `Children` reference declared in the `Resource` base class to limit the type of resources that can be added as children.

```cs
public class OverrideSample : Resource
{
    [ReferenceOverride(nameof(Children)]
    public IReferences<IMyChild> MyChildren { get; set; }
}
```