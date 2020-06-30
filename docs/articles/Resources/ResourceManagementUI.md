---
uid: ResourceManagementUI
---
# Resources

By default the resource management comes with a graphical UI shown in image below. The UI accesses the resource graph over a web-service hosted by the default implementation of IRootResource. The UI visualizes a resource tree on the left and a details view with aspects of a resource on the right. The web-service as well as the UI is optional and can easily be removed or replaced by applications.

![Resources workspace](images\ResourcesWorkspace.png)

* Region A shows the resource tree
* Region B shows some general details which are part of the master view and fix for any detail views
* Region C shows all configured aspects of a resource
* Region D contains the details from an aspect selected in region C

## Resource Tree

The resource tree contains all currently available resources. The details of a resource will be loaded by selecting it. A selection will lead a load of all information for the available aspects. There is the possibility to refresh the tree on top of it to get for example new created resources. There is also the posibility to add new resources. This will open a dialog which provides a type tree which is filtered by the selected resource by iterating over the type tree looking for all types that are assignable to the propertytype. All matches and their derived types are included in the type tree. That way the user can only add resources that are actually compatible and eliminating the risk of misconfiguration

If the creation of a resource requires parameters or it can be created in different ways, developers can deﬁne constructors. Two examples for resource constructors are shown in snippet below.

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

Because of the IoC approach and C# inability to split object creation and constructor invocation, resource constructors are implemented as methods with the [ResourceConstructor](xref:Moryx.AbstractionLayer.Resources.ResourceConstructorAttribute)-attribute. That way the resource manager can create the instance and wire all framework components and reference properties before invoking the constructor method. This makes it possible for developers to use the full feature set of the `AL` within the constructor.

## General Details

There are some general information of a resource which is a fix part of the master view. So it will be visible in all cases even if there is a custom details view. There is also a small area for buttons which contains currently only the `delete` button which will delete the selected resource. After selecting a resource will open its details. Some information are build on the platforms `Entry` format. The Entry-format is an enhanced, recursive key-value structure. It is designed to serialize types or objects and edit or instantiate them on the client, without knowledge of the concrete type. It is an important tool of the platform to easily add resources or other plugins on the server without the need for a new web-service or specialized UI. Developers can decide which properties should be visible by decorating them with the `EditorBrowsable` attribute. In many cases properties will be decorated with `DataMember` and `EditorBrowsable` to save modiﬁed values, but splitting them allows for special cases. Using only `DataMember` lets developers save values without giving the user access and using only `EditorBrowsable` lets users edit runtime values that should not be saved.

The details view with its aspect is the default which is sufficient for many resources and helps developers to get started quicker. If this default does not ﬁt or does not meet the requirements it can be easily replaced with a custom UI. To implement and register a custom UI a details view model is created that is registered for a server side resource type like shown below.

```cs
// On the server
public class Custom : Resource
{
    [EditorBrowsable]
    public SomeEnum EnumValue { get; set; }
}

// On the client
[ResourceDetailsRegistration("Custom")]
public class CustomDetailsViewModel : ResourceDetailsViewModelBase<CustomViewModel>
{
}
```

The type given in the registration is the name of the class on the server. The registration is not limited to an exact type match but will also apply to all derived types unless a more concrete details view model is registered. With this registration the resource UI will create an instance of the view model and its associated view. The custom details view also references a `CustomViewModel` in the generic argument of the base class. This view model is used to map the server side properties transmitted in the `Entry`-format.

```cs
public class CustomViewModel : PropertyChangedBase
{
    [ConfigKey("EnumValue")]
    public string Value { get; set; }

    public string [] PossibleEnumValues { get; set; }
}
```

The code for the custom view model declares two properties. One representing the current value of the enum property and the other contains all members declared on the server side. Per default properties of the view model class are mapped automatically, optionally the `ConﬁgKey`-attribute can be used. The automatic property map per detects properties that should be ﬁlled with possible values based on the `ConﬁgKey`-attribute or using a naming convention like in the example. Custom view models are not limited to ﬂat objects with strings. The mapper also performs the type conversion for example for numeric values and even maps to class properties if they exist in the view model. If the server side object declares a collection they are mapped to a specialized collection of type `EntryCollection<T>` where `T` is the type of view model entries. Building a user friendly custom view will always mean more effort and thereby costs more than the default view. But with the availability of the Entry-mapper and reusable controls building a custom view requires only a few lines of code.

## Aspects

A resource has different aspects which are available in seperate `Tabs`. Each aspect is configurable. So it is possible to hide aspects and show for example only the custom aspect. To show or hide an aspect just add or delete it from the config which is shown in the following example:

````json
{
  "AspectConfigurations": [
    {
      "PluginName": "PropertiesAspectViewModel"
    },
    {
      "PluginName": "ReferencesAspectViewModel"
    },
    {
      "PluginName": "ResourceMethodsAspectViewModel"
    },
    {
      "PluginName": "MyAspectViewModel"
    }
  ],
  "ConfigState": "Generated"
}
````

An aspect will hide itself automatically if it is not relevant. For example: The `PropertiesAspectViewModel` is not *relevant* if the the resource have no properties to configure.

To implement a custom aspect just use the base class [ResourceAspectViewModelBase](Moryx.Resources.UI.Interaction.Aspects.). It provides the `IEditableObject` and the methods `Load` and `Save` to load additional data and save the changes.

### Properties

The `Properties` aspect will show all available configurable properties of a resource. It can be change only in the Edit-Mode.

### References

The `Reference` aspect shows all possible references of the selected resouce. Every resource has a `Parent` reference. It can be used to reorder resource in the whole tree. Each reference gets an indicator if the reference is currently linked to another resource or not. All possible resource for a reference can be shown by selecting the reference. The currently linked resource shows the same indicator to show the connection. There are two buttons at the bottom of the aspect to link or unlink a resource. Each reference can be a single- or multireference. A singlereference can only be linked to `one` resource. A multireference can be linked to more then one resources.

### Methods

To interact with a resource there is the posibility to provide an additional web-service. An alternative is to decorate a method with the `EditorBrowsable` property. This will automatically lead to an entry in the method aspect. After selecting a method there is the posibility to enter values and inspect the returned value after invoke the method. Like the details view this feature uses the platforms `Entry` format and editor to serialize, transmit and visualize the methods parameters. This automatically includes features like the dropdown-box for enum parameters. With this feature previous tasks like adding a Selftest button to the UI, which would cost at least a full day, now only require the time for implementing the functionality, without any additional infrastructure effort. This aspect is independet to the `Edit Mode` and the containing methods can be called at any time.
