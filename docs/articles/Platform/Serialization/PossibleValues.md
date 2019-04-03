---
uid: PossibleValues
---
# PossibleValuesAttribute

In many cases it makes sense to limit the number of options a user or maintainer can select for a value. Either to eliminate values which are not supported or to offer a preselected list of values to reduce complexity. An easy example would be to give a range of integer values for a timer interval and a more complex approach is to list all possible component names that support a given interface. This attribute is abstract and can not be used directly but must be inherited to implement a certain behaviour. This attribute is only used for user interaction purposes and is not validated by neither the configuration management nor any other component of the platform. Values modified code or the json will still be loaded as valid and passed on to the module! Its abstract members are described below.

````cs
/// <summary>
/// Base attribute for all attributes that support multiple values
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public abstract class PossibleValuesAttribute : Attribute
{
    /// <summary>
    /// Flag if this member implements its own string to value conversion. If this is set to false the runtime
    /// will try to convert the the value with a predefined set of conversions for native types. This must be overriden
    /// whenever your string repesents and object.
    /// </summary>
    public abstract bool OverridesConversion { get; }

    /// <summary>
    /// Flag if new values shall be updated from the old value. This is used only for object values which support multiple
    /// types. In this case the runtime will try to copy as many values from the old to the new object.
    /// </summary>
    public abstract bool UpdateFromPredecessor { get; }

    /// <summary>
    /// All possible values for this member represented as strings. The given container might be null
    /// and can be used to resolve possible values
    /// </summary>
    public abstract IEnumerable<string> GetValues(IContainer container);

    /// <summary>
    /// String to value conversion. Must be override if <see cref="OverridesConversion"/> is set to true"/>
    /// </summary>
    public virtual object Parse(IContainer container, string value)
    {
       return value;
    }
}
````

### PrimitiveValuesAttribute

This basic implemtantion of the `PrimitiveValuesAttribute` enables you to add several possible values to basic-datatype properties. It supports the following datatypes:

- bool
- byte
- int
- long
- double
- string

To realize that it uses overloaded constructures with a param-parameter in the specific data-type.

### CpuCountAttribute

As a direct descendant this attribute can be used to configure the number of parallel processes used for a certain operation. An overload of the constructor allows to specify how many threads should be reserved for the rest of the application. If this value is greater than the total thread count, it will be initialized with 1.

````cs
[DataMember, Description("If this property is 0 it will be set to the systems thread count")]
[CpuCount]
public int ImporterThreads { get; set; }

[DataMember, Description("If this property is 0 it is set to the systems thread count - 2 unless the system does not offer enough threads, then 1")]
[CpuCount(2)]
public int ModestThreadCount { get; set; }
````

### CurrentHostNameAttribute

Whenever you want to configure a hostname for WCF this attribute provides a nice alternative to `[DefaultValue("localhost")]`. Its usage in code is pretty straight forward.

````cs
[DataMember, CurrentHostName]
public string HostName { get; set; }
````

### IntegerStepsAttribute

To provide a preselection of values for integer config values this attribute can be used. It generates a list of values defined by a minimum, maxium, progression value and progression type. It does offer either a linear or exponential increase of the values. This defines wether the new value is created by adding the step or multiplying the step.

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

To specify which implementation to use, the platform provides support for the factory pattern and named components. The name of the component is defined in its component attribute and the factory provides a method `Create(string name)`. To reduce complexity of deployment and configuration and avoid typos when specifing these names the ComponentNameSelectorAttribute creates a list of available implementations of a certain interface. Its usage is pretty straight forward and only requires the type of interface to look for.

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