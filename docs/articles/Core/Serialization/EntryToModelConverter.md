---
uid: EntryToModel
---
# Entry to Model Converter

TODO: describe generically. Do not reference to components which are not related to the platform

For improved usability the ResourceUI should show specialized UIs which require custom ViewModels. In order to avoid creating one WCF service per resource
type we wanted to create a way to use our generic Entry format used to serialize any class tree in specialized ViewModels in the client. The solution
is the ConfigToViewModelConverter. It is a custom tool that maps a received Entry object tree onto instances of a certain type with a minimalistic API.

## Usage

To use the converter a type specific instance must be created with one of the `Create()` overloads. This instance offers two methods `ToConfig` and `FromConfig`
that will write the config values onto ViewModel and back. The converter either tries to map by property name or uses the `ConfigKeyAttribute` definition.
Collections of sub entries must be of type `ConfigEntryCollection` to activate converter support. An example can be found below:

````cs
public class SomeViewModel : IConfigEntryLoader
{
    // Creating an instance 
    private static readonly Converter = ConfigToViewModelConverter.Create<SomeConfigViewModel>();

    public void Load(Entry[] config)
    {
        _config = config;
        ConfigViewModel = new SomeConfigViewModel();
        Converter.FromConfig(ConfigViewModel, _config);
    }

    public void Write()
    {
        Converter.ToConfig(ConfigViewModel, _config);
        SendToServer(_config);
    }
}

public class SomeConfigViewModel
{
    private int _value;
    public int Value
    {
        get { return _value; }
        set
        {
            _value = value;
            NotifyOfPropertyChange();
        }
    }

    private string _otherValue;
    [ConfigKey("OtherKey")]
    public string OtherValue
    {
        get { return _otherValue; }
        set
        {
            _otherValue = value;
            NotifyOfPropertyChange();
        }
    }

    private ConfigEntryCollection<SubViewModel> _enums;
    public ConfigEntryCollection<SubViewModel> Enums
    {
        get { return _enums; }
        set
        {
            _enums = value;
            NotifyOfPropertyChange();
        }
    }

    // Adds prototype instance to the collection
    public async Task Add()
    {
        await Enums.Add();
    }
}

public class SubViewModel
{
    private string _enumValue;
    public string EnumValue
    {
        get { return _enumValue; }
        set
        {
            _enumValue = value;
            NotifyOfPropertyChange();
        }
    }

    // Auto matched to regex Possible(?<key>\w)s
    public string[] PossibleEnumValues { get; set; }
}
````

## Concept

This section is intended for developers that want to understand and extend the converter. The probably most important aspect is to understand the two stage
reflection approach. When creating the converter for a type it parse the class with all its properties and prepares an internal representation of the class
and its properties. Later when invoking `ToConfig` and `FromConfig` only this object tree is used and reflection is kept to an absolute minimum, which is the
reason for its performance.

### Building the Class Model

In the `Create()`-method