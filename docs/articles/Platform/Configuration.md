---
uid: Configuration
---
# Configuration

## Introduction

Configuration of components is one of the core features of the platform. Configuration provides the base to create more flexible software and enables programmers to design components for a range of different application scenarios. Furthermore configuration gives developers the opportunity to adapt an installation to the local environment. For example a configuration may contain database names, ports or directories. It may also contain flags and values to specify different behaviours of the component. Collections in the configuration may be used to ether support a number of endpoints or to configure objects of the same type but variable number.

## Defining a configuration

Configurations in the platform are written as classes to make in-code usage as easy as possible and avoid unnecessary file and string parsing. Configuration values are written as properties, classes or collections of class instances. With respect to a couple of rules and conventions almost any structure a developer might need is possible. Some of theses conventions are necessary to save to configuration to a file, others are boundaries set by the generic configuration editor which will explained later. These rules can be ignored but don't blame me, if this results in non permanent configurations or poor editor support!
Quick example of a configuration:

````cs
[DataContract]
public class ModuleConfig : ConfigBase
{
    [DataMember]
    public SubConfig SubConfiguration { get; set; }

    [DataMember, Description("Some bool flag")]
    public bool SomeBool { get; set; }

    [DataMember, Description("List of configured strategies")]
    [PluginConfigs(typeof(IStrategy), false)]
    public List<StrategyConfig> Strategies { get; set; }
}

[DataContract]
public class SubConfig
{
    [DataMember]
    public string SomeString { get; set; }
}
````

The basic structure of the configuration object including the [IConfig](xref:Marvin.Configuration.IConfig) implementation. The dummy properties and subclass can be renamed or deleted once you understand the structure of a configuration.

## Conventions

As mentioned before there are a couple of rules and conventions when defining a configuration. Some of them are mandatory while others are optional comfort features like selfrepair or initialization. To improve understanding of the configuration management inside the platform the details and reasons for each of these conventions will be explained below.

### DataContract and DataMember

These attributes are defined inside the .NET and are necessary to save the configuration to an json file. Classes that shall be serialized or used as `DataMember` properties in another `DataContracts` must be decorated with the `DataContractAttribute`. All properties that shall be saved to the file and restored on the next start must be decorated with the `DataMemberAttribute`. Properties without `DataMemberAttribute` are still visible to the component and the configuration editor but will always have their default value and all modifications are lost on reload.

### Collections

The platform configuration management supports collections of subclasses if they meet two requirements. The type of elements in the collection must be `DataContracts` in order to be saved to and reloaded from the file.

Supported Collection Types:

- Lists: `List<T>`
- Arrays: `ArrayType[]`
- Dictionaries: `Dictionary<ref, obj>`
  - Primitives as value are currently not supported

### Item Naming

In the configuration editor each entry is represented by its property name and value. Because entries in collections do not have a property name they will be displayed using their class name. Therefor it is recommended to overwrite ToString() on a subclass to give each item in the collection overview a more meaningful key.

## Attributes

To make working with configurations more convinient and support the development of flexible modules for the runtime, the MarvinRuntime does provide three base attributes allthough two of them are closely related. These attributes can be inherited to customize their behaviour or provide application specific behaviour. A couple of commonly used implementations are distributed with the Marvin.RuntimeUtils.ConfigAttributes.dll as part of the full Runtime-package. First the base types will be explained in detail including a guide to customize them. Then all distributed attributes will be briefly explained regarding their behaviour.

### PossibleValuesAttribute

In many cases it makes sense to limit the number of options a user or maintainer can select for a value. Either to eliminate values which are not supported or to offer a preselected list of values to reduce complexity. An easy example would be to give a range of integer values for a timer interval and a more complex approach is to list all possible component names that support a given interface. This attribute is abstract and can not be used directly but must be inherited to implement a certain behaviour. This attribute is only used for user interaction purposes and is not validated by neither the configuration management nor any other component of the platform. Values modified code or the json will still be loaded as valid and passed on to the module! Its abstract members are described below.

````cs
/// <summary>
/// Base attribute for all attributes that support multiple values
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public abstract class PossibleValuesAttribute : Attribute
{
    /// <summary>
    /// All possible values for this member represented as strings. The given container might be null
    /// and can be used to resolve possible values
    /// </summary>
    public abstract IEnumerable<string> ResolvePossibleValues(IContainer pluginContainer);

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
    /// String to value conversion. Must be override if <see cref="OverridesConversion"/> is set to true"/>
    /// </summary>
    public virtual object ConvertToConfigValue(IContainer container, string value)
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
[DataMember, IntegerSteps(0, 100, 10, EStepMode.Addition)]
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

## Futher supported .NET attributes

- `DescriptionAttribute`
- `DefaultValueAttribute`

### DefaultValueAttribute(object value)

This attribute serves a dual purposes - it sets default values on config creation and restores missing values. The second one comes pretty handy whenever the config class is modified and properties are added or renamed because in that case the deserializer fails to set the value of the property based on the json and would leave it at its default value(0, 0.0 or null). Using the `DefaultValueAttribute` will save you from constantly checking config values for null and a working combinations of config defaults will simplify application deployment and testing significantly. Note: `DefaultValueAttribute` will overwrite the properties value everytime it has its system default e.g. 0, false or null! Therefor it should not be used on properties where these values are considered valid. For bool it is recommended to simply invert the naming the make the system default valid. For example a property "Enabled" shall be true by default could be named "Disabled" with false as default. Enums can declare the member 0-Unset which helps differentiate between system and application default.