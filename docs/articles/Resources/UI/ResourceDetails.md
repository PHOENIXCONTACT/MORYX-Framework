---
uid: ResourceDetails
---
# Resource details

Selecting a resource on the left will open its detail view B,C & D. Per default a generic UI is shown that displays the resources basic information at the top (B), generic buttons (C) below it and a large editor for settings and references (D). The editor in the settings tab is generic and built on the platforms `Entry` format. The Entry-format is an enhanced, recursive key-value structure. It is designed to serialize types or objects and edit or instantiate them on the client, without knowledge of the concrete type. It is an important tool of the platform to easily add resources or other plugins on the server without the need for a new web-service or specialized UI. One specialty of the `Entry`-format is its ability to alter collections and the type of referenced classes on the client without knowledge of the class itself. If an entry represents a collection or a property of a class is a class itself, it has an attribute called prototypes. Prototypes are `Entry`-converted instances of types that can be assigned to the property or added to the collection on the server side. Using the prototype pattern the client can replace properties or extend collections by cloning the prototype selected by the user. The properties shown in the editor are read directly from the resource object on the server. Developers can decide which properties should be visible by decorating them with `EditorVisible`. In many cases properties will be decorated with `DataMember` and `EditorVisible` to save modiﬁed values, but splitting them allows for special cases. Using only `DataMember` lets developers save values without giving the user access and using only `EditorVisible` lets users edit runtime values that should not be saved. In addition to the settings tab, region D also shows a references tab. All reference properties of the resource are listed in this tab. While the settings tab is used to conﬁgure the resource instance, the references tab conﬁgures its role and relations in the resource graph. As previously mentioned the [ﬁgure in Overview](xref:ResourcesUIOverview) only shows the default UI. That default is sufﬁcient for many resources and helps developers to get started quicker. If this default does not ﬁt or does not meet the requirements it can be easily replaced with a custom partial UI. To implement and register a custom UI a details view model is created that is registered for a server side resource type like shown below.

```cs
// On the server
public class Custom : Resource
{
    [EditorVisible]
    public SomeEnum EnumValue { get; set; }
}

// On the client
[ResourceDetailsRegistration("Custom")]
public class CustomDetailsViewModel : ResourceDetailsViewModelBase<CustomViewModel>
{
}
```

The type given in the registration is the name of the class on the server. The registration is not limited to an exact type match but will also apply to all derived types unless a more concrete details view model is registered. With this registration the resource UI will create an instance of the view model and its associated view when an instance matching the type requirements is selected from the tree view (A). While custom detail views are already part of the current version, the inheritance based view selection for derived types is a unique feature of the new resource management.
The code below also references a `CustomViewModel` in the generic argument of the base class. This view model is used to map the server side properties transmitted in the `Entry`-format.

```cs
public class CustomViewModel : PropertyChangedBase
{
    [ConfigKey("EnumValue")]
    public string Value { get; set; }

    public string [] PossibleEnumValues { get; set; }
}
```

The code for the custom view model declares two properties. One representing the current value of the enum property and the other contains all members declared on the server side. Per default properties of the view model class are mapped automatically, optionally the `ConﬁgKey`-attribute can be used. The automatic property map per detects properties that should be ﬁlled with possible values based on the `ConﬁgKey`-attribute or using a naming convention like in the example. Custom view models are not limited to ﬂat objects with strings. The mapper also performs the type conversion for example for numeric values and even maps to class properties if they exist in the view model. If the server side object declares a collection they are mapped to a specialized collection of type `EntryCollection<T>` where `T` is the type of view model entries. This collection is necessary to utilize the previously mentioned prototypes to add entries. Building a user friendly custom view will always mean more effort and thereby costs than the default view. But with the availability of the Entry-mapper and reusable controls building a custom view requires only a few lines of code. Depending on the applications target audience time that might otherwise be spent for support. The default view is perfect to get started, as an alternative for simple resources or for resources only conﬁgured by experts, otherwise a user friendly custom view is a recommended part of a resource package.
