---
uid: ResourceTypeTree
---
# Resource type tree

All plugin types within the resource management are derived from the [Resource](/src/Moryx.AbstractionLayer/Resources/Resource.cs) class, either directly or by deriving from a subtype or [Driver](/src/Moryx.AbstractionLayer/Drivers/Driver.cs). 
Inheriting makes it possible to extend and customize existing resources or share functionality among resources by creating a common, abstract base class. 
The central base-class `Resource` is especially important. 
The [Resource Management's](ResourceManagement.md) objective is to model the CPS (cyber physical system) and make it easily accessible to other modules. 
Types derived from `Resource` or more general those implementing [IResource](/src/Moryx.AbstractionLayer/Resources/IResource.cs) are the entry points into the resource graph from the facade. 
In that sense, the abstraction layer can be compared to the `Android HAL` and its device contracts. 
Interfaces derived from `IResource` are those contracts that can be defined by applications from different domains. 

Storage of resources and their information is done by storing it as part of a resource itself or by using a dedicated resource. 
Thereby, the complete type information is taken from the class definition. 
Base type, provided interfaces, dependencies and typed references to other resource can all be expressed with C# code. 
The minimal required code to create a plugin for the resource management is shown in an example below.

```cs
public class MyFirstResource : Resource
{
}
```

The above example shows only the minimum required code to illustrate extensibility and lack of boilerplate code. 
Most resource implementations will also declare the [ResourceRegistration](/src/Moryx.AbstractionLayer/Resources/Attributes/ResourceRegistrationAttribute.cs) attribute to activate dependency injection for non-resource components like logging. 
During module startup all types are loaded and the bidirectional [type tree](/src/Moryx.AbstractionLayer/Resources/IResourceTypeTree.cs) is constructed using reflection. 
For each node in the tree it is possible to traverse the tree in both directions. 
Especially the ability to directly access all derived types is an advantage over the standard .NET reflection API. 
Besides the base and derived types, each node also exports the system type, its name and information how to construct instances of the type.

## Resource types
The abstraction layer allows for grouping similar resources types in branches. Look [here](Types/Overview.md) for more information on that.