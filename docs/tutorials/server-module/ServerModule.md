---
uid: GettingStarted.ServerModule
---
# ServerModule

The ServerModule is the place where you have access to level 1 and the level 2 components of your module.
So this is the place where you have to link the different components and make them work together.

This document describes how to build (the basis for) a new ServerModule from scratch and step by step.

Add a new project to your solution. The name of the project should be the name of your new ServerModule. (In the following examples the name "Execution" is chosen.)

Your new ServerModule-Project gets at least one folder: "ModuleController". The "ModuleController" consists at least of three files.

For implementation details click on the file name:
- [ServerModule](#servermodule)
  - [The ModuleController.cs](#the-modulecontrollercs)
  - [The ModuleConfig.cs - File](#the-moduleconfigcs---file)
  - [The ModuleConsole.cs - File](#the-moduleconsolecs---file)

## The ModuleController.cs 
The ModuleController.cs-File is the key point of your module. Here, all the components of your module come together and you are responsible to initialize, start and stop them in the right way. Because every ServerModule can have its own architecture, there is not _the correct way_ to do so, but there are some _usual points_ a ModuleController.cs-File covers. These points are:

1. Import the global components your ServerModule needs
2. Register imported _global_ components to the internal container
3. Resolve the desired components from the container and **start** them
4. Stop the started components when the ServerModule is stopped
5. Export and Import facades -> this topic of its own, take a look into [this guide](Facades.md)

Now we will look at examples for these points. But first create your your class implementing `ServerModuleBase`. If your ServerModule exports facades, use `ServerModuleFacadeControllerBase` instead and specifiy those facades using `IFacadeContainer<TFacade>`.

For the following properties and attributes reference these files:

* Moryx.dll
* Moryx.Runtime.dll

````cs
[Description("Example description")]
public class ModuleController : ServerModuleBase<ModuleConfig>
{
    internal const string ModuleName = "ExampleName";
    
    /// <summary>
    /// Name of this module
    /// </summary>
    public override string Name
    {
        get { return ModuleName; }
    }
    
    ...
````

As an example for the first bullet point, we import the ResourceManagement and the ProductManagement. We do so by simply adding them as public properties, the global DI container will do the rest. (The RequiredModuleApi-Attribute is described [here](Facades.md))

The DbContextManager as well as the ConfigManager are part of the ASP Service Collection. This is the reason why they have to be injected via the constructor. 

````cs
[RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
public IResourceManagement ResourceManagement { get; set; }

[RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
public IProductManagement ProductManagement { get; set; }

/// <summary>
/// Generic component to access every data model
/// </summary>
public IDbContextManager DbContextManager { get; }

/// <summary>
/// Create new module instance
/// </summary>
public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory, IDbContextManager contextManager)
    : base(containerFactory, configManager, loggerFactory)
{
    DbContextManager = contextManager;
}
````

Now we will register the global components to the internal container of our module. We will also load the components of this module. Components can be for example Plugins or Strategies. We do this in the _OnInitialize_ method we must override form our base class:

````cs
/// <summary>
/// Code executed on start up and after service was stopped and should be started again
/// </summary>
protected override void OnInitialize()
{
    // Register all imported components
    Container.SetInstances(ResourceManagement, ProductManagement);

    // Load all components
    Container.LoadComponents<IExamplePlugin>();
    Container.LoadComponents<IExampleStrategy>();
}
````

After the initialization we have to start the custom plugins of our ServerModule activate facades and in many cases we also have to start WebServices. We do so in the derived _OnStart_ method.

````cs
protected override void OnStart()
{
    // Activate facades
    ActivateFacade(_playGroundExecution);

    // Start Plugin
    Container.Resolve<IExamplePlugin>().Start();
}
````

Even the greatest ServerModule must be stopped from time to time. We must override the _OnStop_ method to clean up our ServerModule and we must put it in a state in which it can be reinitialized. This includes for example to dispose and clear WCF hosts and deactivate facades: 

````cs
/// <summary>
/// Code executed when service is stopped
/// </summary>
protected override void OnStop()
{
    // Deactivate facades
    DeactivateFacade(_playGroundExecution);

    // Stop Plugin
    Container.Resolve<IExamplePlugin>().Stop();
}
````

## The ModuleConfig.cs - File 

In the _ModuleConfig.cs_-file you can define the data fields needed to configure your ServerModule. For each file a xml configuration file will be automatically during the *build* process. Here you can set the configuration values for your ServerModule manually. You can also use the CommandCenter website to edit the values.

The following points must be noted:

* Your ModuleConfig class must derive from ConfigBase.cs
* You must add the _DataContract_ attribute to your class
* You must add the _DataMember_ attribute for each of the data fields
* (Beyond this you can use the _DefaultValue_ attribute to add a default value to your data fields)

The following code is an example for a ModuleConfig.cs:

````cs
[DataContract]
public class ModuleConfig : ConfigBase
{
    public ModuleConfig()
    {
        ExecutionWebHostConfig = new HostConfig
        {
            Endpoint = "ExecutionWeb",
            BindingType = ServiceBindingType.WebHttp,
            MetadataEnabled = true,
            HelpEnabled = true
        };
    }
    [DataMember]
    [DefaultValue(42)]
    public int InstanceCount { get; set; }

    [DataMember]
    [DefaultValue("Hello World")]
    public string WcfMessage { get; set; }

    [DataMember]
    public HostConfig ExecutionWebHostConfig { get; set; }

}
````

## The ModuleConsole.cs - File

The module console provides a way to execute methods using the maintenance. It can be used for initial testing, debugging or 'admin access'-features. For this feature you need to create a `ModuleConsole.cs` file in your _ModuleController_ folder, implement `IServerModuleConsole` and add methods using the Attribute `EntrySerialize` in order to see them on the UI. Although the interface is empty, it's needed for the export.

````C#
[ServerModuleConsole]
internal class ModuleConsole : IServerModuleConsole
{
    
    [EntrySerialize]
    public void DoSomething(int input)
    {
        ...
    }

    ...
}
````