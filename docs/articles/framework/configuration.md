---
uid: Configuration
---
# Configuration

## Introduction

Configuration of components is one of the core features of the platform. Configuration provides the base to create more flexible software and enables programmers to design components for a range of different application scenarios. Furthermore configuration gives developers the opportunity to adapt an installation to the local environment. For example a configuration may contain database names, ports or directories. It may also contain flags and values to specify different behaviors of the component. Collections in the configuration may be used to ether support a number of endpoints or to configure objects of the same type but variable number.

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

The basic structure of the configuration object including the [ConfigBase](/src/Moryx/Configuration/ConfigBase.cs) implementation. The dummy properties and subclass can be renamed or deleted once you understand the structure of a configuration.

## Conventions

As mentioned before there are a couple of rules and conventions when defining a configuration. Some of them are mandatory while others are optional comfort features like self-repair or initialization. To improve understanding of the configuration management inside the platform the details and reasons for each of these conventions will be explained below.

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

To make working with configurations more convenient and support the development of flexible modules for the runtime, the MoryxRuntime does provide three base attributes although two of them are closely related. These attributes can be inherited to customize their behavior or provide application specific behavior. A couple of commonly used implementations are distributed with the Moryx.RuntimeUtils.ConfigAttributes.dll as part of the full Runtime-package. First the base types will be explained in detail including a guide to customize them. Then all distributed attributes will be briefly explained regarding their behavior.

### further supported .NET attributes

- `DescriptionAttribute`
- `DefaultValueAttribute`

### DefaultValueAttribute(object value)

This attribute serves a dual purposes - it sets default values on config creation and restores missing values. The second one comes pretty handy whenever the config class is modified and properties are added or renamed because in that case the deserializer fails to set the value of the property based on the json and would leave it at its default value(0, 0.0 or null). Using the `DefaultValueAttribute` will save you from constantly checking config values for null and a working combinations of config defaults will simplify application deployment and testing significantly. Note: `DefaultValueAttribute` will overwrite the properties value every time it has its system default e.g. 0, false or null! Therefor it should not be used on properties where these values are considered valid. For bool it is recommended to simply invert the naming the make the system default valid. For example a property "Enabled" shall be true by default could be named "Disabled" with false as default. Enums can declare the member 0-Unset which helps differentiate between system and application default.

## Integrating ASP.NET Configuration Providers

In ASP.NET Core, the default configuration mechanism is `IConfiguration`, which is backed by a set of configuration providers. This is also the preferred way to supply secrets to an ASP.NET—and therefore a MORYX—application.

Integration of `IConfiguration` into MORYX is optional and must be explicitly enabled when registering the MORYX kernel:

```csharp
// Program.cs

services.AddMoryxKernel(configurationIntegration: true);

```
### Mapping IConfiguration Keys to MORYX Configurations
Once enabled, configuration values from IConfiguration can be used to populate MORYX configuration properties that do not already have a value.
By default, configuration keys are mapped as follows:
```
<FullyQualifiedConfigurationTypeName>:<PropertyName>
```

### Example
The ModuleConfig of the ProductManagement module contains a property called MaxImporterWait.
To configure this value via IConfiguration, define the following key:
```
Moryx.Products.Management.ModuleConfig:MaxImporterWait
```

You can archive this by adding the following to your appsettings.json
``` jsonc
// appsettings.json
{
  "Moryx.Products.Management.ModuleConfig": {
    "MaxImporterWait": 15
  }
}
```

### Nested Properties and Collections
This approach also works for nested objects and collections. You can traverse the structure by chaining property names and indices.
For example, to configure the plugin used by the third entry in TypeStrategies:

```
Moryx.Products.Management.ModuleConfig:TypeStrategies:2:PluginName
```
This can be expressed in appsettings.json in multiple ways:
``` jsonc
// appsettings.json

{
  "Moryx.Products.Management.ModuleConfig": {
    "TypeStrategies": [
      { },
      { },
      {
        "PluginName": "ExamplePlugin"
      }
    ]
  }
}
```
or alternatively
```jsonc
// appsettings.json
{
  "Moryx.Products.Management.ModuleConfig:TypeStrategies:2": {
    "PluginName": "ExamplePlugin"
  }
}
```

> [!CAUTION]
> The collection integration works **if and only if** the list is null or if all the needed entries in the list already exist and can be modified.
> Due to the way configuration management in MORYX works, it is impossible to add an additional entry that is only defined through ConfigurationProviders to a List.

> [!CAUTION]
> The alternative syntax can be much shorter for long existing lists, where you want to make sparse changes.  
> It is only equivalent to the first assuming the type strategies already exist in the MORYX Config.  
> If no list exists yet, the entries that have no values are pruned away by Microsofts binding mechanism.  
> You might expect TypeStrategies to be [null, null, { "PluginName": "ExamplePlugin" }], but it's actually [{ "PluginName": "ExamplePlugin" }].  
> This could lead to subsequent problems, because the indices in the conifg keys do not match the indices in the instantiated objects.

### Customizing Key Mapping with ProvidedConfigAttribute
The default key mapping can be customized using the ProvidedConfigAttribute.

#### On Class Level
Applying the attribute to a configuration class allows you to override the default prefix (the fully qualified class name):

```c#
[ProvidedConfig("Example")]
public class ExampleConfig
```
This changes the base configuration path to Example.

### On Property Level
When applied to a property, the attribute allows you to define a custom configuration key:

* **Absolute path**: Any value not starting with .: is interpreted as a full configuration path.
* **Relative path**: Prefix the value with .: to make it relative to the class-level prefix.

### Dynamic Keys via Property Values
To avoid hardcoding configuration keys, you can dynamically resolve them from another property using the fromProperty flag.

### Complete example
```c#
[ProvidedConfig("Example")]
public class ExampleConfig
{
    [DataMember]
    public int ExampleNumber { get; set; }
    // Resolved as: Example:ExampleNumber

    public string Secret1 { get; set; }
    // Resolved as: Example:Secret1
    // Not marked with DataMember to avoid storing secrets in plain text

    [ProvidedConfig("ConfigKey")]
    public string Secret2 { get; set; }
    // Resolved as: ConfigKey (absolute path)

    [ProvidedConfig(".:Secret")]
    public string Secret3 { get; set; }
    // Resolved as: Example:Secret (relative path)

    [DataMember]
    public string ConfigKey1 { get; set; } = "Secret";

    [ProvidedConfig(nameof(ConfigKey1), true)]
    public string ConfiguredSecret1 { get; set; }
    // Resolved as: Secret (value taken from ConfigKey1)

    [DataMember]
    public string ConfigKey2 { get; set; } = ".:Relative";

    [ProvidedConfig(nameof(ConfigKey2), true)]
    public string ConfiguredSecret2 { get; set; }
    // Resolved as: Example:Relative (relative path resolved from ConfigKey2)
}
