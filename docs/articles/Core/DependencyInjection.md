---
uid: DependencyInjection
---
# Dependency Injection

The dependency injection in MORYX-Modules is basically realized with a `Castle Windsor` container. It is encapsulated in the class `Moryx.Container.CastleContainer` to decorate it with some useful methods. It is also implementing the interface `Moryx.Container.IContainer`. So it is only necessary to know this interface instead of the concrete container class. This provides the possibility to change the container flexible to use for example derived containers or just to switch from Castle Windsor to a different DI Container.

MORYX Container is more than just inject the first suitable object from the Container. It provides the possibility to choose the object you want to inject from a pool of suitable objects by using [different Component Selectors](https://github.com/castleproject/Windsor/blob/master/docs/typed-factory-facility-interface-based.md) or the possibility to define at your component how you want to register it just by using the `Moryx.Container.RegistrationAttribute`.

## How to register Components

Components can be registered in the Container by two main methods:

````cs
Container.SetInstance(MyComponent);

Container.LoadComponents<IMyComponentInterface>(); 
```` 

The main Task is to find the needed components and to register it by the `Moryx.Container.RegistrationAttribute`. There are several ways to register an component. In most case methods **LoadComponents** or **SetInstance** are enough but there are several `Register` methods for different ways of component registration. The following sections will describe the possibilities.

### LoadComponents

If you want to register components by a given Interface then use `LoadComponents()`. The `LoadComponents()` method is a generic method which needs an interface to search for Components which are implementing this interface. It will search in the AppDomain. The AppDomain includes your default build folder to search for Components.

````cs
Container.LoadComponents<IMyComponentInterface>();
````

A condition can be useful if there are a bunch of components, but you want to register a sub-amount of it.
For example, you want to register all components which are complete different but marked with an attribute. So you can register these components like that.

````cs
Container.LoadComponents<object>(type => type.GetCustomAttribute(typeof(MyAttribute)) != null);
````

Or you can just filter components with the same interface but some of them are marked with an attribute.

````cs
Container.LoadComponents<IMyComponentInterface>(type => type.GetCustomAttribute(typeof(MyAttribute)) != null);
````

### SetInstance

If you want to just register a concrete instance without a search by an interface or a predicate to filter for atrributes or something similar then just use the `SetInstance()` method for the registration.

````cs
var myInstance = new HappyClass();
Container.SetInstance(myInstance);
````

If your class implements an interface then you can also register it for that interface.
So a new instance of the class will be registered with the given interface.

````cs
var myInstance = new HappyClass();
Container.SetInstance<IHappyClass>(myInstance);
````

If you have more instances of a component which are almost similar, but you want to distinguish the instances then you can also use its name for the registration.

````cs
var modules = new List<MyModule>()
{
    new ModuleA(), new ModuleB()
};

foreach(var module in modules)
    Container.SetInstance(module, module.Name);
````

Here you can also register the instances with an additional interface.

````cs
var modules = new List<MyModule>()
{
    new ModuleA(), new ModuleB()
};

foreach(var module in modules)
{
    Container.SetInstance<IMyComponentInterface>(module, module.Name);
}
````

If you have structures like

````cs
Container.SetInstance(component1);
Container.SetInstance(component2);
Container.SetInstance(component3);
````

Then you can just use the fluent method to register more than one component. You can register multiple instances by.

````cs
Container.SetInstance(component1)
    .SetInstance(component2);
    .SetInstance(component3);
````

## How to get Components

If there are already registered components then you have some possibilities to get them.

### Resolve

For example, you have a `ServerModule` and you want to initialize its `Plugins` in a defined order during the `ServerModule` start. So you have to resolve the `Plugins` you want and call the necessary method. To resolve a `Plugin` just use the `Resolve`-method.

````cs
var myPlugin = Container.Resolve<IMyPlugin>();
````

Then you get the first plugin with the given interface. If there are more than one Plugin` which is implementing the given interface then you can use the concrete type or the name. Remember, don´t forget to register the plugins before with its type or name.

````cs
var myConcretePlugin1 = Container.Resolve<MyPluginClass>();
var myConcretePlugin2 = Container.Resolve("MyPluginName");
var myConcretePlugin3 = Container.Resolve<IMyPlugin>("Trick17Plugin");
````

### ResolveAll

It is also possible to get more than one component from the container just by using an interface or the class type. Just use the ResolveAll method. This works for Interfaces and Class Types but not for defined names.

````cs
IMyPlugin>[] plugins1 = Container.ResolveAll<IMyPlugin>();
MyPluginClass[] plugins2 = Container.ResolveAll<MyPluginClass>();
````

## How to register Plugins or Components

If you implement a ServerModule then you may also want to implement some `Plugins` or `Components`. The main difference of a Plugin and a Component is that a Component is part of a fix structure. A plugin is just a small exchangeable part. So usually a Component will be registered with the `Component` Attribute and a Plugin with the `Plugin` Attribute. Both are `RegistrationAttribute` which will be used for the automatic registration. Both will be registered in the container of the `ServerModule`.

````cs
[Component(LifeCycle.Transient, typeof(IMyComponent), Name = ComponentName)]
public class MyComponent : IMyComponent
{
    private const string ComponentName = nameof(MyComponent);
    ...
}
````

````cs
[Plugin(LifeCycle.Transient, typeof(IMyPlugin), Name = PluginName)]
public class MyPlugin : IMyPlugin
{
    private const string ComponentName = nameof(MyPlugin);
    ...
}
````

---

### Register additional plugins or components from your assembly

In some case you want to implement a component which needs some additional plugins from your assembly. So you can extend your list of component registration, or you can use the `Moryx.Container.DependencyRegistrationAttribute`. You can decide if all other components from your assembly should be registered (that would be lazy) or a list of some types.

As an example we want to implement a UserAssignment so we implement a Component which is part of a ServerModule:

````cs
[Component(LifeCycle.Transient, typeof(IUserAssignment))]
public class UserAssignment : IUserAssignment
{
    private const string ModuleName = nameof(UserAssignment);
    ...
}
````

Then there is a requirement that an external system should be informed about the assignment. So we can use for example the Hook Pattern.

````cs
[Plugin(LifeCycle.Transient, typeof(IUserAssignmentHook), Name = PluginName)]
internal class UserAssignmentHook : IUserAssignmentHook
{
    public const string PluginName = nameof(UserAssignmentHook);
    ...
}
````

So the component has a dependency to the hook. Lets register it with the `Moryx.Container.DependencyRegistrationAttribute`:

````cs
[Component(LifeCycle.Transient, typeof(IUserAssignment))]
[DependencyRegistration(typeof(IUserAssignmentHook))]
public class UserAssignment : IUserAssignment
{
    private const string ModuleName = nameof(UserAssignment);
    ...
}
````

So if the UserAssignment will be injected then it will have a dependency to the hook, and it will be also injected.
In this case the `Moryx.Container.DependencyRegistrationAttribute.InstallerMode` is set to `Specified`. It is also possible to set the InstallerMode to `All` to register all other plugins and components from your assembly but this can cause some problems. If you register all other plugins and components with the `DependencyRegistrationAttribute` instead of the explicit registration at your Container with the `LoadComponents` or `SetInstance` method then other components are indirectly depending to your component because they get their plugins because your component load them **accidentally** with the DependencyRegistration.

### Register additional plugins or components from another assembly

The same use case as in the section before. You implement a component but there is an requirement to extend it. Now the extension needs a handling with an external system and you are a good software programmer so you outsource it in a different assembly to encapsulate it because in some other cases you don´t need this extension.

In this case you also need a `DependencyRegistrationAttribute` but with an additional `Moryx.Container.ISubInitializer`. A `SubInitializer` can load types from other assemblies like in the following example.

Know we implement a Plugin in a different Assembly:

````cs
[Plugin(LifeCycle.Transient, typeof(IUserAssignmentHook), Name = PluginName)]
internal class UserAssignmentHook : IUserAssignmentHook
{
    public const string PluginName = nameof(UserAssignmentHook);
    ...
}
````

Then we implement the `SubInitializer` to register the needed types from the other Assembly

````cs
public class MyInitializer : ISubInitializer
{
    public void Initialize(IContainer container)
    {
        container.LoadComponents<IUserAssignmentHook>();
    }
}

````

Now we can use the `SubInitializer` for the `DependencyRegistrationAttribute`:

````cs
[Component(LifeCycle.Transient, typeof(IUserAssignment))]
[DependencyRegistration(typeof(IUserAssignmentHook), Initializer = typeof(MyInitializer))]
public class UserAssignment : IUserAssignment
{
    private const string ModuleName = nameof(UserAssignment);
    ...
}
````

## How to register Factories

In some case it is helpful or necessary to implement a Factory. A Factory can also be registered in the Container so every Component or Plugin can get injected the Factory. You have only to use the `Moryx.Container.PluginFactoryAttribute`. You have also to define in the `RegistrationAttribute` if the Factory should create Plugins by its Configuration or by its Name. So you have to choose the right `ComponentSelector` so that the Factory can create the needed Plugin out of the LocalContainer.

### Name based component selector

If you have some plugins without any configuration then you can just use the `Moryx.Container.INameBasedComponentSelector` for the Factory. Let's implement the plugins:

````cs
[Plugin(LifeCycle.Transient, typeof(IMyPlugin), Name = nameof(MyPluginA))]
public class MyPluginA : IMyPlugin, IPlugin
{

}

[Plugin(LifeCycle.Transient, typeof(IMyPlugin), Name = nameof(MyPluginB))]
public class MyPluginB : IMyPlugin, IPlugin
{

}
````

Now we have two different plugins with different names. Make sure that the Plugins have different names and implement your name based factory like:

````cs
[PluginFactory(typeof(INameBasedComponentSelector))]
public interface IMyFactory
{
    IMyPlugin Create(string name);

    void Destroy(IMyPlugin instance);
}
````

This is the simplest way to get the needed plugin from a factory.

### Config based component selector

If the plugins gets more complex and they need an extra configuration. Then you can use the `Moryx.Container.IConfigBasedComponentSelector` for the Factory to get the plugin instances by its configuration.

Let's create a configuration class.

````cs
[DataContract]
public class MyConfig : IPluginConfig
{
    [DataMember]
    public virtual string PluginName { get; set; }

    [DataMember]
    public int HowOftenIForgotTheDataMemberAttribute { get; set; }
}
````

Now we have a configuration class we can use for our plugins or inherit them for an extended configuration class. Let's implement the plugins:

````cs
[Plugin(LifeCycle.Transient, typeof(IMyPlugin), Name = nameof(MyPluginA))]
public class MyPluginA : IConfiguredModulePlugin<MyConfig>
{

}

[Plugin(LifeCycle.Transient, typeof(IMyPlugin), Name = nameof(MyPluginB))]
public class MyPluginB : IConfiguredModulePlugin<MyConfig>
{

}
````

Now we have two plugins which implements the `IConfiguredModulePlugin` interface with a given config. So we can get a plugin from the `Factory` with a matching configuration.

````cs
[PluginFactory(typeof(IConfigBasedComponentSelector))]
public interface IMyFactory
{
    IMyPlugin Create(MyConfig pluginConfig);

    void Destroy(IMyPlugin instance);
}
````

## Common Mistakes

### No RegistrationAttribute

"Why i get a NullReferenceException?", "Why is my component null?" and "Do you use your own Framework?". This are typical questions from Joe Gunchy. But Joe forgot the `Moryx.Container.RegistrationAttribute` for his components. Don't be stupid like Joe. Use `RegistrationAttribute`s so the MORYX container knows how to register the components you want.

### Wrong build path

YOu must reference your project in the application-project or start-project so that the MORYX Container can find your components. Joe Gunchy forgot to reference his project in the application-project so the container can't find his components. Don´t be stupid like Joe.

### Missing Name, Class or Interface

"I am using RegistrationAttributes but my component is still null! Why is your system so shitty". Joe Gunchy tries to get his components by using a name/class/interface, but he forgot to register the component with the name/class/interface. Don't be stupid like Joe. Make shre you don´t forget to register your components with the information you need for the loading.

### Config based instead of name based component selector

Joe Gunchy wants to use a Factory for simple plugins. The Plugins can be distinguished by the names but Joe implements the `IConfiguredModulePlugin` interface and adds a Config class just to set the PluginName and uses the `IConfigBasedComponentSelector` for the Factory `RegistrationAttribute`. Joe Gunchy is very stupid so don´t be like Joe. In simple cases without a config, use the `INameBasedComponentSelector` for your Factory. No unnecessary classes anymore.

### Missing DependencyRegistration

This is Joe Gunchy. He assumed that everything will be registered somehow automatically and will be injected somehow automatically. Most things seems to be magic but it isn´t. So Joe Gunchy is a complete idiot. Don't be stupid like Joe Gunchy. You need for everything a `RegistrationAttribute` then the MORYX Container will collect your stuff and register it how YOU defined it. Then you can use it.