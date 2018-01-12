---
uid: GettingStarted.Facades
---
Facades
========

Facades are the Runtime ways of interaction between server modules. This tutorial shall cover the both ends of this concept - exporting a facade and importing a facade. This tutorial does require basic knowledge of server modules. The first steps do not represent good code style and should not be used as a reference for facade design. Please complete the tutorial to get the right understanding of how facades work and how they are supposed to be used. As a starting base we need two server modules. Make sure to clear the facade entries on the first tab. Name one module Facades.Dependency and the othter Facades.Dependent. Your solutions should now look like this: 

![](images/FacadeGuideProjects.png)

## Exporting a facade
In the first step we will create a facade and export it in our dependency module.
Creating a facade:
- Open the Facades.Dependency project and create a folder Facade
- Add an interface "IFacade.cs" to the folder
- Define a "Foo"-method in the interface that accepts two integers and returns an integer.
- Add a class "Facade.cs" to the folder an make it implement "IFacade".
- Implement "Foo" by returning the sum of a and b

After we defined our facade we want to export it from the server module. All we must do is let our module controller implement the "IFacadeContainer<T>" interface. Add the "IFacadeContainer<IFacade>" interface to the module controller class and implement it at the bottom of the file: 

![](images/FacadeExport.png)

It is important, that the property always returns the same object in order for the Runtime to function correctly. 

The facade property should be implemented explicit for a simple reason. If our module should export more than one facade it would have two properties with the same name but different type. While this is not possible for implicit class members, explicit interface implementations do not cause any conflicts. In our case this does not matter but since it does no harm for single facades.

````cs
IFacade IFacadeContainer<IFacade>.Facade => _facade;
````

## Importing a facade
Now that we have exported the facade we want to use it in another server module. To do this our dependent module must reference the dependency module. In real world applications the facade interface will be located in a separate bundle library to reduce coupling of dependency and implementation.

Steps:

1. Reference the project "Facades.Dependency" and set CopyLocal to false
2. Add a property of type "IFacade" to the module controller
3. Decorate it with [RequiredModuleApiAttribute](xref:Marvin.Runtime.ModuleManagement.RequiredModuleApiAttribute)
  3.1 IsStartDependency = true will instruct the module manager to bind the dependent modules life cycle to the dependency ones
  3.2 IsOptional = true will allow this property to be null if no other module exports it. If this remains false the module manager will abort the boot process due to incomplete dependencies.

Your dependent module should look like this: 

![](images/FacadeGuideDependent.png)

Of course a module can import more than one facade. Just add more properties of the facades you want to import and decorate them with RequiredModuleApiAttribute

## Importing many facades
Like with every DI-container the Runtime facade linker is able to inject a collection of instances if more than one component exports the interface. While most frameworks will leave you the choice of collection type. Currently collection injection only works with arrays. So far the dependency manager does not take collection injection into account when it comes to start dependencies. Therefor be careful when accessing the facades as the might raise InvalidHealthStateExceptions.
Usage in code: 

````cs
[RequiredModuleApi]
public IFacade[] AllInstances { get; set; }
````

# Facade Design Guideline

## Bidirectional Communication
Occasionally the relationship between two modules requires bidirectional communication. The standard .NET way to to this is by using method calls from the dependent
to its dependencies and events/callbacks from dependency to dependent. The starting behaviour of the Runtime however created problems in the kind of missed events
when the dependency raised events before the listener has reached the necessary state. After many long discussions including circular dependencies, three way handshake
and sacrificing kittens to the race-condition-god, we came up with a fairly simple solution. Events are supposed to propagate changes and should be used as such.

Every event invocation represents an incremental change that occurred within an object. An entry added to the collection or a modified property. Without the initial/
previous state of the object its current state can not be determined. Therefore every API that offers events must also provide methods to retrieve the current state
of the component. For `NotifyPropertyChanged` and `NotifyCollectionChanged` the initial state is the object itself.

Transferring this to MaRVIN facades means that for every event there must be way to retrieve the current state of the module. This decouples the life cycles and allows
the dependent to be started and stopped at any time without having to worry what he might miss in the process. For example the API for the JobManagement could simply
be:

````cs
/// <summary>
/// Facade of the JobManagement
/// </summary>
public interface IJobManagement
{
    /// <summary>
    /// Get current state of the job listener
    /// </summary>
    public IEnumerable<Job> CurrentJobs { get; }

    /// <summary>
    /// Retrieve a specific job by its Id. This includes all jobs, even finished and removed ones.
    /// </summary>
    public Job this[long id] { get; }

    public EventHandler<Job> JobStateChanged;

    /// <summary>
    /// Event raised when was finished and is removed from the list
    /// </summary>
    public EventHandler<Job> JobFinished;
}
````