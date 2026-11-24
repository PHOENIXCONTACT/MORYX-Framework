---
uid: PossibleValues
---
# Possible Values

In many cases it makes sense to limit the number of options a user or maintainer can select for a value. Either to eliminate values which are not supported or to offer a preselected list of values to reduce complexity. An easy example would be to give a range of integer values for a timer interval and a more complex approach is to list all possible component names that support a given interface.

There are two options to define possible values
1. .NET `AllowedValuesAttribute`/`DeniedValuesAttribute`
2. MORYX `PossibleValuesAttribute`

## .NET `AllowedValuesAttribute`/`DeniedValuesAttribute`

The `AllowedValuesAttribute` specifies a list of values that should be allowed in a property. The `DeniedValuesAttribute` specifies a list of values that should not be allowed in a property.

You can provide allowed values as follows:

```cs
[AllowedValues(1, 5, 10)]
public int MyIntValues { get; set; }

[AllowedValues(SomeEnum.A, SomeEnum.B, SomeEnum.Z)]
public SomeEnum MyEnumValues { get; set; }
```

and you can provide denied values as follows. All other enum values are automatically possible.

```cs
[DeniedValues(SomeEnum.C, SomeEnum.D)]
public SomeEnum MyEnumValues { get; set; }
```

It is not supported to use `AllowedValues` and `DeniedValues` in combination. `AllowedValues` are always be preferred.

## PossibleValuesAttribute

This attribute is abstract and can not be used directly but must be inherited to implement a certain behavior. This attribute is only used for user interaction purposes and is not validated by neither the configuration management nor any other component of the platform. Values modified code or the json will still be loaded as valid and passed on to the module! All members are documented in [PossibleValuesAttribute](/src/Moryx/Serialization/PossibleValues/PossibleValuesAttribute.cs).

### Display names and descriptions

The values of `GetValues` are only keys, e.g. type names. They are used to create prototypes and are passed to `Parse`, so they must stay stable. To show a more readable text in the UI, override `GetPossibleValues` and return an `EntryPossible` with a display name and description for each key. The default implementation uses the values of `GetValues` as key and display name, so overriding it is optional.

```cs
public class PossibleColorsAttribute : PossibleValuesAttribute
{
    public override bool OverridesConversion => false;

    public override bool UpdateFromPredecessor => false;

    public override IEnumerable<string> GetValues(IContainer localContainer, IServiceProvider serviceProvider)
        => ["Red", "Green"];

    public override IEnumerable<EntryPossible> GetPossibleValues(IContainer localContainer, IServiceProvider serviceProvider)
    {
        yield return new EntryPossible { Key = "Red", DisplayName = "Stop", Description = "Machine must stop" };
        yield return new EntryPossible { Key = "Green", DisplayName = "Go", Description = "Machine may run" };
    }
}
```

The keys of `GetPossibleValues` must match the values of `GetValues`. If the attribute uses `RequiresPossibleValuesContext`, override the overload with `PossibleValuesContext` instead.

For attributes that list types, the extension `Type.ToEntryPossible(key)` creates the entry from the `DisplayAttribute`/`DisplayNameAttribute` and `DescriptionAttribute` of the type. The built-in `PossibleTypesAttribute`, `PluginConfigsAttribute`, `AvailableCapabilitiesAttribute` and `ResourceTypesAttribute` already do this.

```cs
public override IEnumerable<EntryPossible> GetPossibleValues(IContainer localContainer, IServiceProvider serviceProvider)
    => GetTypes().Select(type => type.ToEntryPossible(type.Name));
```

> [!NOTE]
> `GetValues` will be removed in the next major version and `GetPossibleValues` will become the only method to provide possible values.

## Implementations

### IntegerStepsAttribute

To provide a pre-selection of values for integer config values this attribute can be used. It generates a list of values defined by a minimum, maximum, progression value and progression type. It does offer either a linear or exponential increase of the values. This defines wether the new value is created by adding the step or multiplying the step.

```cs
[DataMember, IntegerSteps(0, 100, 10, StepMode.Addition)]
public int Offset { get; set; }
```

### RelativeDirectoriesAttribute

Often directories are used as config parameters and to reduce typos or invalid paths, this attribute can list the full directory tree within a certain directory. By default the root directory is the Runtime root directory, but it can also be a folder within the Runtime directory or a path anywhere on the system.

```cs
[DataMember, RelativeDirectories]
public string Directory { get; set; }
```

### PluginNameSelectorAttribute

To specify which implementation to use, the platform provides support for the factory pattern and named components. The name of the component is defined in its component attribute and the factory provides a method `Create(string name)`. To reduce complexity of deployment and configuration and avoid typos when specifying these names the `PluginNameSelectorAttribute` creates a list of available implementations of a certain interface from the respective, available container. Its usage is pretty straight forward and only requires the type of interface to look for.

```cs
[DataMember]
[PluginNameSelector(typeof(IStrategy))]
public string ActiveStrategy { get; set; }
```

### PluginConfigsAttribute

Similar to the `PluginNameSelectorAttribute` this attribute provides a list of component configurations. These configurations are expected by the implementations of a certain interface and allow each implementation to define its own configuration. This does require the implementation to be decorated with the attribute `[ExpectedConfig(typeof(MyConfig))]`. This attribute can be used on single properties or collections. Therefor you can either select the type of a new entry or change an existing entry's type. All configs will be presented as a list of their respective type names and converted into an object if a type name is selected. If the config type is decorated with a `DisplayAttribute`/`DisplayNameAttribute` or `DescriptionAttribute`, the UI shows these instead of the type name. When decorating a config member with this attribute the configs base type can be excluded if it is abstract. Always make sure to specify the interface type and not the base config type!

```cs
[DataMember]
[PluginConfigs(typeof(IStrategy))]
public StrategyConfig ActiveStrategy { get; set; }

[DataMember]
[PluginConfigs(typeof(IPlugin))]
public List<PluginConfig> ActivePlugins { get; set; }
```

### PossibleTypesAttribute

This attribute generates possible values from given or derived types found at runtime, either as simple type name or full name. This attribute is useful when creating modules or components, that can be extended with derived types later. Like for `PluginConfigsAttribute`, the UI shows the display name and description of the type if available.

Properties of type `System.Type` are serialized by the built-in `TypeEntryEncoder`. Without `PossibleTypesAttribute` the value is read-only; adding the attribute makes the property editable and restricts selection to the specified types:

```cs
[PossibleTypes(new []{ typeof(Type1), typeof(Type2) })]
public Type ActiveType { get; set; }
```

The attribute can also be used on `string` properties that store only the type name rather than the `Type` object:

```cs
[PossibleTypes(typeof(MyBaseType), UseFullName = true)]
public string ConfiguredType { get; set; }

[DataMember]
[PossibleTypes(new []{ typeof(Type1), typeof(Type2) })]
public List<string> SupportedTypes { get; set; }
````

### Custom Instance aware PossibleValues

Implement a custom attribute when you need a possible value that is extracted from the current instance. You can use
that custom attribute to perform your desired use case. 
````cs
public class MyPossibleUserAttribute : PossibleValuesAttribute
{
    public MyPossibleUserAttribute()
    {
        RequiresPossibleValuesContext = true;
    }

    public override IEnumerable<string> GetValues(PossibleValuesContext ctx)
    {
        return ctx.Instance is MyType myInstance ? myInstance.PossibleUsers : [];
    }

}
````
The example `PossibleUserAttribute` inherits from the `PossibleValuesAttribute`. The `PossibleValuesContext.Instance` property represent the current instance of the class in which the attribute was used. You also have indirect access to the local `Container` and the `ServiceProvider` via the `PossibleValuesContext.GetService(Type)` method.

````cs
    [EntrySerialize]
    [DataMember]
    public List<string> PossibleUsers { get; set; }

    [DataMember]
    [MyPossibleUserAttribute]
    public string SelectedUser { get; set; }
````

The `RequiresPossibleValuesContext` flag by default is optional, but here it is important and required if you need access to the current instance.
