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

````cs
[AllowedValues(1, 5, 10)]
public int MyIntValues { get; set; }

[AllowedValues(SomeEnum.A, SomeEnum.B, SomeEnum.Z)]
public SomeEnum MyEnumValues { get; set; }
````

and you can provide denied values as follows. All other enum values are automatically possible.

````cs
[DeniedValues(SomeEnum.C, SomeEnum.D)]
public SomeEnum MyEnumValues { get; set; }
````

It is not supported to use `AllowedValues` and `DeniedValues` in combination. `AllowedValues` are always be prefered.

## PossibleValuesAttribute

This attribute is abstract and can not be used directly but must be inherited to implement a certain behavior. This attribute is only used for user interaction purposes and is not validated by neither the configuration management nor any other component of the platform. Values modified code or the json will still be loaded as valid and passed on to the module! Its abstract members are described below.

````cs
/// <summary>
/// Base attribute for all attributes that support multiple values
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public abstract class PossibleValuesAttribute : Attribute
{
    /// <summary>
    /// Flag if this member implements its own string to value conversion. If this is set to false the runtime
    /// will try to convert the the value with a predefined set of conversions for native types. This must be overridden
    /// whenever your string represents and object.
    /// </summary>
    public abstract bool OverridesConversion { get; }

    /// <summary>
    /// Flag if new values shall be updated from the old value. This is used only for object values which support multiple
    /// types. In this case the runtime will try to copy as many values from the old to the new object.
    /// </summary>
    public abstract bool UpdateFromPredecessor { get; }

    /// <summary>
    /// All possible values for this member represented as strings. The given containers might be null
    /// and can be used to resolve possible values
    /// </summary>
    public virtual IEnumerable<string> GetValues(IContainer container, IServiceProvider serviceProvider);

    /// <summary>
    /// String to value conversion. Must be override if <see cref="OverridesConversion"/> is set to true"/>
    /// </summary>
    public virtual object Parse(IContainer container, IServiceProvider serviceProvider), string value)
    {
       return value;
    }
}
````

## Implementations

### IntegerStepsAttribute

To provide a pre-selection of values for integer config values this attribute can be used. It generates a list of values defined by a minimum, maximum, progression value and progression type. It does offer either a linear or exponential increase of the values. This defines wether the new value is created by adding the step or multiplying the step.

````cs
[DataMember, IntegerSteps(0, 100, 10, StepMode.Addition)]
public int Offset { get; set; }
````

### RelativeDirectoriesAttribute

Often directories are used as config parameters and to reduce typos or invalid paths, this attribute can list the full directory tree within a certain directory. By default the root directory is the Runtime root directory, but it can also be a folder within the Runtime directory or a path anywhere on the system.

````cs
[DataMember, RelativeDirectories]
public string Directory { get; set; }
````

### PluginNameSelectorAttribute

To specify which implementation to use, the platform provides support for the factory pattern and named components. The name of the component is defined in its component attribute and the factory provides a method `Create(string name)`. To reduce complexity of deployment and configuration and avoid typos when specifying these names the ComponentNameSelectorAttribute creates a list of available implementations of a certain interface. Its usage is pretty straight forward and only requires the type of interface to look for.

````cs
[DataMember]
[PluginNameSelector(typeof(IStrategy))]
public string ActiveStrategy { get; set; }
````

### PluginConfigsAttribute

Similar to the `PluginNameSelectorAttribute` this attribute provides a list of component configurations. These configurations are expected by the implementations of a certain interface and allow each implementation to define its own configuration. This does require the implementation to be decorated with the attribute `[ExpectedConfig(typeof(MyConfig))]`. This attribute can be used on single properties or collections. Therefor you can either select the type of a new entry or change the entries type. All configs will be presented as list of their respective types name and converted into in object if one is selected. When decorating a config member with this attribute the configs base type can be excluded if it is abstract. Always make sure to specify the interface type and not the base config type!

````cs
[DataMember]
[PluginConfigs(typeof(IStrategy))]
public StrategyConfig ActiveStrategy { get; set; }

[DataMember]
[PluginConfigs(typeof(IPlugin))]
public List<PluginConfig> ActivePlugins { get; set; }
````

### PossibleTypesAttribute

This attribute generates possible values from given or derived types found at runtime, either as simple type name or full name. This attribute is useful when creating modules or components, that can be extended with derived types later.

````cs
[PossibleTypes(typeof(MyBaseType), UseFullName = true)]
public string ConfiguredType { get; set; }

[DataMember]
[PossibleTypes(new []{ typeof(Type1), typeof(Type2) })]
public List<string> SupportedTypes { get; set; }
````
