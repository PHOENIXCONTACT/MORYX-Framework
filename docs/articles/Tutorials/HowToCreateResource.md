---
uid: HowToCreateAResource
---
How to create a resource {#howToCreateResource}
======

The general concept of the resource management is described on the [architecture](xref:architecture) pages.
In this document we concentrate on creating a custom resource.
Please read the documenation about [products, articles](xref:concepts-Products) and [resource architecture](xref:concepts-Products) very carefull.
In this howto we will implement a resource which will assign manually scanned numbers to an article identifier.
 
The resource should be connected with a [scanner driver](xref:scannerDriver) to read a code and assign it to an article.

## Preparing Project Structure
In this chapter we create the general class structure of our new scan resource. A resource has a ResourceConfig calls and the Resource compont class at least. You might add more classes to define custom ResourceMessages or implement helper methods.
***
### Resource Config Class
First of all, create a class which represents your resource configuration. If your resource does not need a configuration you can skip this step
Earch resource can have their custom configuration. For this resource we need only one configuration property to select the right scanner driver.
Create a class with the postfix config: _ScanResourceConfig.cs_. 
The class should derive from IUpdatableConfig. Lets have a look on some sample code:

~~~~{.cs}
[DataContract]
public class ScanResourceConfig : UpdatableEntry
{
    [DataMember, DriverNames(typeof(IScannerDriver))]
    public string ScannerDriver { get; set; }
}
~~~~

The class has the DataContractAttribute to indicate that the configuration is serializable. 
The next property is the _ScannerDriver_. It will be used to configure a scanner driver for this resource.
Next to the DataMemberAttribute (serialization info), the [DriverNamesAttribute](xref:Marvin.Drivers.DriverNamesAttribute) 
will help UI's and config transformer to find possible config values for this entry. The driver will be configured only by a name. 
The name can be set whithin the driver configuration in the resource management.

#### Store config in database
To save this config in the database, you need to add a DataConverte class to the Resource component class. 

#### Define a default config value
If you want the Config to have default values, you can add them with the DefaultValue attribute. 
As long as there is no config defined in the database, the property will be initialized with this value after start.
~~~~{.cs}

        [DataMember, DefaultValue(@"D:\Temp\Source"), Description("Location to read printing images")]
        public string SourceImagePath { get; set; }
~~~~
***
### Resource Component Class 
For the resource component, we need to create a second class. Lets call it _ScanResource.cs_. 
This class will represent the scanner resource. The class has to implement IResource. 
If you add this interface to the new class you will see a lot of properties and methods to implement. 
To reduce the work in all resources, we created a [ResourceBase](xref:Marvin.Resources.ResourceBase) which will contain 
some default code. So derive your resource from [ResourceBase](xref:Marvin.Resources.ResourceBase). If your resources requires configuration
use the base class with the type parameter. The type parameter will represent your configuration class. 

Lets have a look on our code:

~~~~{.cs}
[ResourceRegistration(ResourceName)]
public class ScanResource : ResourceBase<ScanResourceConfig>
{
    internal const string ResourceName = "ScanResource";

    public override ResourceMode Mode
    {
        get { return ResourceMode.Production; }
    }

    protected override MessageHandlerMap CreateHandlerMap(MessageHandlerMap currentMap)
    {
        return currentMap
            .Register<QueryStateMessage>(QueryState)
            .Register<StartActivity>(HandleStartActivity)
            .Register<SequenceCompleted>(HandleSequenceComplete);
    }
    
    protected override Initialize(IUnitOfWork uow, ResourceContext context)
    {
        base.Initialize(uow, context);
        Capabilities = new ScannerCapabilities(true, true, true);
    }
}
~~~~

I added the [ResourceRegistrationAttribute](xref:Marvin.Resources.ResourceRegistrationAttribute) on top of the class
to handle the registration within the resource management. Do it also for your class.
For the variable configuration, the resource base will read the configuration from the database. As default, a default config serializer will be used. 
For custom configs, you have to implement a [data converter](xref:dataconverter) described below in this document.

Visual Studio will give you a hint to implement some properties and methods. For the ResourceMode just use Production for this HowTo.
As described in the architecture, the resource management will provide the resources implementations by their capabilities. 
For this resource we can use the [ScannerCapabilities](xref:Marvin.AbstractionLayer.Capabilities.ScannerCapabilities) which are allready implemented
in the AbstractionLayer.

~~~~{.cs}
public bool CanContinuousScan { get; private set; }
public bool CanSingleShot { get; private set; }
public bool CanTriggerSingleShot { get; private set; }
~~~~

#### Receive ResourceMessages

If your resource is running in the context of a control system, you will need to handle the following messages. If you use the Resource without the control system, you might implement only a fow of these messages and add custom messages to transport the necessary information inyour application. 
The resource should handle general activities:
* [QueryStateMessage](xref:Marvin.AbstractionLayer.Resources.QueryStateMessage)
* [StartActivity](xref:Marvin.AbstractionLayer.Resources.StartActivity)
* [InspectArticleActivity](xref:Marvin.AbstractionLayer.InspectArticleActivity)
* [SequenceCompleted](xref:Marvin.AbstractionLayer.Resources.SequenceCompleted)

The method `CreateHandlerMap` is used to dispatch calls to the `Handle(message)` to different methods based on the message type. 
It will be called by the resource management to send several messages to the implemented resource. We should implement a method per message.
If you do not derive from `ResourceBase` directly you should call `base.CreateHandlerMap`. **Note** that `CreateHandlerMap` builds a chain of 
responsibility and the order of invocation can have an impact on the way your resource operates.

Assume you have two derived messages `MessageA` and `MessageB`. `MessageB` is derived from `MessageA` and your base class has registered a handler
for `MessageA`. Depending on the order of invocation your handler might never get called 

~~~~{.cs}
// By first calling base.CreateHandlerMap() the base handlers will be considered before yours. Therefor the `HandleB` method is never called
protected override MessageHandlerMap CreateHandlerMap(MessageHandlerMap currentMap)
{
    return base.CreateHandlerMap(currentMap)
        .Register<MessageB>(HandleB);
}

// By registering your handler first it will catch all instances of `MessageB` and its derived types before forwarding the base class
protected override MessageHandlerMap CreateHandlerMap(MessageHandlerMap currentMap)
{
    currentMap = currentMap.Register<MessageB>(HandleB);
    return base.CreateHandlerMap(currentMap);
}
~~~~


You should implement OnStart() and OnDispose() for registering and loading your scanner driver ([ScannerDriver](xref:scannerDriver)).

Let us concentrate on the key resource methods.

#### QueryState

The query state message will ask the resource for the current state. The resource should answer with an [ReadyToWork](xref:Marvin.AbstractionLayer.Resources.ReadyToWork) or an [NotReadyToWork](xref:Marvin.AbstractionLayer.Resources.NotReadyToWork).
We should check our driver state to check if we are ready to do some work.

~~~~{.cs}
private void QueryState(QueryStateMessage queryStateMessage)
{
    var readyToWork = Session.StartSession(ResourceMode.Production, Id, ReadyToWorkType.Push);
    if (_scanner.CurrentState.Classification == StateClassification.Running)
        _currentState = readyToWork;
    else
        _currentState = readyToWork.PauseSession();

    RaiseMessageOccured(_currentState);
}
~~~~

#### StartActivity

If the upper layer recieves an ready to work, they can send the resource some work. Specially, the upper level can create a [StartActivity](xref:Marvin.AbstractionLayer.Resources.StartActivity)
with the ready to work. For this resource it makes sense to save the [StartActivity](xref:Marvin.AbstractionLayer.Resources.StartActivity) for a while in a private variable.

~~~~{.cs}
private void HandleStartActivity(StartActivity message)
{
    _currentSession = message;
}
~~~~

If the scanner driver will raise the event that an code was scanned, we can write the code to the article.

~~~~{.cs}
private void OnScannerCodeRead(object sender, string code)
{
    // Check if we should currently read barcodes
    if (_currentSession == null || _currentSession.StartableActivity.Type != InspectArticleActivity.TypeName)
        return;

    var activity = (InspectArticleActivity)_currentSession.StartableActivity;
    var process = (ProductionProcess)activity.Process;

    process.Article.Identity.SetIdentifier(code);

    var activityCompletedMsg = _currentSession.CreateActivityResultMessage((long)InspectionResult.Success);

    _currentSession = null;

    RaiseMessageOccured(activityCompletedMsg);
}
~~~~

After successfull reading of the code, the resource will create an [ActivitiyCompleted](xref:Marvin.AbstractionLayer.Resources.ActivityCompleted) and sends it to the upper levels. 
If we get no more activities the upper level will send a [SequenceCompleted](xref:Marvin.AbstractionLayer.Resources.SequenceCompleted) to indicate that no more activities will be send.
For this resource we do not have to handle the completed message.



#### CapabilityChanged event

If it is nesessary to react when a capability has changed, is it possible to attach to the CapabilityChanged event. The signature depends on wether the event is handled within the resource management or outside the module. Internally the `sender` contains the resource object that changed its capabilities. Outside of the module the sender is the `IResourceManagement` facade and a `CapabilitiesChangedEventArgs` is published that contains the id of the resource.

Internal:

~~~~{.cs}
private void ScannerResourceCapabilityChanged(object sender, ICapabilities newCapabilities)
{
    var id = ((IResource)sender).Id;
    if (((ScannerCapabilities) newCapabilities).CanContinuousScan)
    {
         // Do something
    }
}
~~~~

External:

~~~~{.cs}
private void ResourceCapabilityChanged(object sender, CapabilitiesChangedEventArgs args)
{
    var id = args.ResourceId;
    if (((ScannerCapabilities) args.Capabilities).CanContinuousScan)
    {
         // Do something
    }
}
~~~~
***
#### Data Converter
The ResourceInteraction service uses DataConverter instances to convert an entities extension data to an object and back (you need this to store the properties defined in ResourceConfig in database). 
A sample converter might look like this:

~~~~{.cs}
[DataConverterRegistration]
public class CustomDataConverter : IExtensionDataConverter
{
    ///
    public virtual string[] SupportedTypes 
    { 
        get { return new[] { "MyResourceType" }; } 
    }

    ///
    public virtual object Prototype
    {
        get { return new MyExtension(); }
    }

    ///
    public string ToExtensionData(object obj)
    {
        return ResourceContext.ConvertExtension((MyExtension)obj);
    }

    ///
    public object FromExtensionData(string extensionData)
    {
        return ResourceContext.ReadExtension<MyExtension>(extensionData);
    }
}
~~~~
***
## How to use the Resource in a custom module
If you want to use the new resource from a custom module, you need to request the resource from the ResourceManagement. Inject the ResourceManagement into the ModuleControler and pass the object to the inner container of your custom module.
~~~~{.cs}
    [ServerModule(ModuleName)]
    public class ModuleController : ServerModuleBase<ModuleConfig>
    {
        //Let the component be injected from the external container
        [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
        public IResourceManagement ResourceManagement { get; set; }

        public override string Name
        {
           ///
        }

        #region state transition
        protected override void OnInitialize()
        {
            //pass the component to the inner container
            Container.SetInstance(ResourceManagement);
        }

        protected override void OnStart()
        {
            ///
        }

        protected override void OnStop()
        {
           ///
        }
        #endregion

    }
~~~~
Let the ResourceManagement be injected into your module and get the Id of your resource from the ResourceManagement. The Id is used to identify the resource and send ResourceMessages to this resource.
~~~~{.cs}
....
    public IResourceManagement ResourceManagement { get; set; }
....
    public void Start()
    {
        _mstbPrinter = ResourceManagement.GetIdByCapabilities(new PrinterCapabilities());
        ResourceManagement.MessageOccured += OnResourceMessageReceived;            
    }
...
~~~~
The Resource can only be retrieved by its capabilities. There is no way to get it by name. 
If you have the Id of your resource, you can send ResourceMessages to the Resoure and receive Resource Messages from this resource.

### Receive ResourceMessages

### Send a ResourceMessage to a resource
If you already have the Id (ResourceManagement.GetIdByCapabilities(new PrinterCapabilities());) of the resource you want to send the Message to, call the handle method of the ResourceManagement.
~~~~{.cs}
    ResourceManagement.Handle(new ResponseProductDataMessage(_mstbPrinter));
~~~~
Each message gets the Id of the destination resource as parameter.
***
##Create your own ResourceMessage
In some cases it might be necessary to define custom resource messages since no existing message fit to the needs of your application. A resource message is derived from ResourceMessage and the implementation needs to overwrite the Type field. Since this is a custom message, you can add all properties you might need.
~~~~{.cs}
public class RequestProductDataMessage: ResourceMessage
    {
        private string _type = "RequestProductDataMessage";

        public string Article { get; set; }
        public string Revision { get; set; }

        public string SolutionId { get; set; }

        public RequestProductDataMessage(long resourceId, string article, string revision) : base(resourceId)
        {
            Article = article;
            Revision = revision;
        }

        public override string Type { get { return _type; } }
    }
~~~~