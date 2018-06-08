# ResourceManagement

## Description # {#ResourceManagement-description}

The [ResourceManagement](xref:Marvin.Resources.Management) is a server module providing access to Resources and their Drivers. Although Devices and Resources are combined within one Module, the functional parts are separated. The DriverController is the accesspoint to the typecasted driver-api.

## Provided facades # {#ResourceManagement-provided}

ResourceManager's public API is provided by the following facades:
 
* [IResourceManagement](xref:Marvin.Resources.IResourceManagement)

## Dependencies # {#ResourceManagement-dependencies}

## Referenced facades ## {#ResourceManagement-references}

Plugin API | Start Dependency | Optional | Usage
-----------|------------------|----------|------
[INotificationPublisher](xref:Marvin.Notifications.INotificationPublisher)|Yes|Yes|The NotificationPublisher is used to publish error messages from the devices.

## Used DataModels ## {#ResourceManagement-models}
The [data model](xref:Marvin.Resources.Model) uses the approach of generic entities that can be mapped to different usage scenarios. Resource entities represent everything from a production site to an exchangeable screw driver bit.
Because all entities are technically identical they are classified by a hierarchical type model. While the types define a strict tree, relations between resources are modeled as an attributed
graph. Each relation is represented by a dedicated relation entity that defines source, target and classification enum. In addition to the generic resource model there is also the concept of resource
items. A resource item represents something that is temporarily on or in a resource. For details on each entity please refer to the subsections of this page.

### [Resource Type](xref:Marvin.Resources.Model.ResourceType) ###
Resource types serve to purposes. One is the functional purpose of representing the different types of resources present in the domain models. The second purpose is a technical one. Resource types
are used to select plugins and strategies that shall be applied to interpret the data of a resource entity. This not only enables custom logic and UIs but also drastically reduces redundant type 
information in each entity. For example instead of always storing fully typed JSON to the extension data the custom converter strategies can reduce the extension data to a minified JSON numeric
array.

### [Resource Entity](xref:Marvin.Resources.Model.ResourceType) ###
Each resource instance is represented by a resource entity. Resource entities are modeled as a directed, attributed graph. Ideally the instance only carries information that can not be derived from
the type. 

### [Resource Relation](xref:Marvin.Resources.Model.ResourceType) ###


### [Resource Item](xref:Marvin.Resources.Model.ResourceItem) ###
Resource items are a generic entity without a direct domain model representation. Resource items were created when we noticed that many resources required merged models to store instances of data
in relation to a resource instance. However on the business layer those objects could not have been more different. Serial numbers, workpieces and staff personell numbers are only a few examples.
But even though the objects were different, the required field types were quite similar. Therefore we created a generic entity that shall be mapped to business object in each class that somehow
represents are resource.

Basically a resource item defines the following fields:

|Field       | Type     | Usage         |
|------------|----------|---------------|
| Unit       | String   | Units are used to define the content of the value field. It is not bound to metric units, but can also be a *Piece*, *Person* or *SerialNumber*. |
| Value      | String   | Mandatory value of the item. It can be anything from a simple number to a JSON string. The owner decides how to read and write the value. |
| Identifier | String   | Optional identifier of the item. |
| IsPublic   | Boolean  | Determines wether the item shall only be used by the owning resource or visible to every resource via the [ResourceItemMapper](xref:Marvin.Resources.IResourceItemMapper). |

**Examples:**
- The [value buffer](xref:Marvin.Resources.SerialNumberProvider.SerialValueBuffer) of the number provider maps buffered serial numbers and MAC adresses onto resource items by storing the number format
to the unit field and the serial number itself to the value field.
- The workpiece carrier handling defined in the [WPC handling package](xref:Marvin.Resources.WpcHandling) creates items with unit *Piece* and stores the running process id to the value field. The items
are assigned to individial positions of the carrier.

# Architecture # {#ResourceManagement-architecture}

## Overview ## {#ResourceManagement-architecture-overview}

Component name|Implementation|Desription
--------------|--------------|----------
IResource|Internal/External|The Resources provided by the ResourceManager
IDevice|Internal/External|The devices used by the resources.
 *TBD* | *TBD* | *TBD*

## Diagrams ## {#ResourceManagement-architecture-diagrams}

![](images\ResourceManagement.png "EA:MARVIN.MARVIN 2.0.ControlSystem.Level-2.Implementation.ReverseEngineering.ResourceManagement")

# Configuration # {#ResourceManagement-configuration}

# Samples #

## Sample: IScanner Interface (e.g. DataLogic) ##
~~~~{.cs}
/// <summary>
/// Common interface for barcode / QR-Code scanners
/// </summary>
public interface IScannerDriver : IDriver
{
    /// <summary>
    /// Event raised when a code was read
    /// </summary>
    event EventHandler<string> CodeRead;
}
~~~~

## Sample: Specialized IScanner for the CognexScanner:  ##
~~~~{.cs}
/// <summary>
/// Interface for the cognex scanner
/// </summary>
public interface ICognexScannerDriver : IScannerDriver
{
    /// <summary>
    /// Determines whether the button of the scanner can be used to initiate a scan or whether it is triggered by the system
    /// </summary>
    bool UseButton { get; set; }

    /// <summary>
    /// When set to scan automatically, then it can continue with the scan when one value was read in.
    /// </summary>
    bool ContinousScan { get; set; }

    /// <summary>
    /// Read a single bar code
    /// </summary>
    void SingleRead(DriverResponse<SingleReadResult> resultCallback);
}

/// <summary>
/// Result of a single read operation
/// </summary>
public class SingleReadResult : TransmissionResult
{
    /// <summary>
    /// Initialize a result object with a given barcode
    /// </summary>
    /// <param name="barCode"></param>
    public SingleReadResult(string barCode)
    {
        BarCode = barCode;
    }

    /// <summary>
    /// Barcode that was read from the device
    /// </summary>
    public string BarCode { get; private set; }
}
~~~~

## Sample: INumberProviderDriver for all drivers that provide some sort of serialnumber ##
Implemented by the OracleDatabaseDriver.
~~~~{.cs}
/// <summary>
/// Interface for number providing drivers
/// </summary>
public interface INumberProviderDriver : IDriver
{
    /// <summary>
    /// Requests the numbers with request parameters and a callback.
    /// </summary>
    void RequestNumbers(NumberProviderRequest request, DriverResponse<NumberProviderResult> callback);
}
~~~~

## Sample: IPlcDriver ##
The Phoenix-Plc will be represented by the generic IPlcDriver that accepts simples DTOs and converts them into specific Plc-Messages. 
~~~~{.cs}
/// <summary>
/// Driver for programmable logic controllers that support object communication
/// </summary>
public interface IPlcDriver : IDriver
{
    /// <summary>
    /// Send message to plc and don't care for a response
    /// </summary>
    void Send(IQuickCast message);

    /// <summary>
    /// Send object to the PLC and await a transmission confirmation
    /// </summary>
    void Send(IQuickCast message, DriverResponse<PlcTransmissionResult> response);

    /// <summary>
    /// Register a message hock at the plc driver
    /// </summary>
    /// <param name="hook">Instance of a message hook</param>
    void Register(IBinaryMessageHook hook);

    /// <summary>
    /// Event rasied when the plc driver receives a message
    /// </summary>
    event EventHandler<PlcMessageEventArgs> Received;
}
~~~~

## Sample: DriverNamesAttribute ##
The config attribute DriverNames(Type driverType) can be used, to allow only certain drivers to be configured on a resource. The config-editor on the MaintenanceWeb-Gui will only list those drivers with a matching type. This reduces the possibility of errors. 
~~~~{.cs}
/// <summary>
/// Possible drivers to fetch serial numbers.
/// </summary>
[DataMember, DriverNames(typeof(INumberProviderDriver))]
[Description("Driver to fetch serial numbers")]
public string Driver { get; set; }
~~~~

The Config-Value can be used within the resource to access the specific driver to use the specific API of this driver.

## Sample: Usage of a driver within a resource:##
~~~~{.cs}
public override void Initialize(IUnitOfWork uow, ResourceContext context)
{
    base.Initialize(uow, context);

    CurrentMode = ResourceMode.Production;

    _connectedPlc = DriverController.Get<IPlcDriver>(Config.DriverName);
    _connectedPlc.StateChanged += OnPlcStateChanged;
    _connectedPlc.Received += OnPlcMessageReceived;
}
~~~~

# LifeCycle and making sure that no DeviceMessages are lost #

The planned common lifecycle for devices and resources is now reality.

~~~~{.cs}
public void Initialize()
{
    DriverController.Initialize();

    // Instanciate resources
    using (var uow = UowFactory.Create())
    {
        var actives = ResourceStorage.LoadActive(uow).ToList();
        // Create instances
        foreach (var active in actives)
        {
            _resources[active.Id] = WrapperFactory.Create(active.Type, this);
        }

        LinkResources(uow);

        // Boot resources
        foreach (var context in actives)
        {
            try
            {
                _resources[context.Id].Initialize(uow, context);
            }
            catch (Exception e)
            {
                ErrorReporting.ReportWarning(this, e);
            }
        }
    }
}

private void LinkResources(IUnitOfWork uow)
{
    var relRepo = uow.GetRepository<IResourceRelationRepository>();
    foreach (var pair in _resources)
    {
        var id = pair.Key;
        var relations = (from relation in relRepo.Linq
                            let sourceRel = relation.SourceId == id  // Standard source to target reference
                            let targetRel = relation.TargetId == id && relation.Bidirectional  // Bidirection target to source reference
                            where sourceRel && relation.Target.Type.HasPlugin || targetRel && relation.Source.Type.HasPlugin
                            select new ReferencedResource
                            {
                                ReferenceId = sourceRel ? relation.TargetId : relation.SourceId,
                                RelationType = (ResourceRelationType)relation.RelationType,
                                RelationName = relation.RelationName
                            }).ToList();

        foreach (var relation in relations)
        {
            relation.Reference = _resources[relation.ReferenceId].Resource;
        }
        pair.Value.SetReferences(relations);
    }
}

///
public void Start()
{
    DriverController.Start();

    // Boot
    foreach (var resource in _resources.Values)
    {
        resource.Start();
    }
}
~~~~

By making sure, that the ResourceLoader initializes all configured resources, before the DriverController starts the drivers, it should be safe to assume that all resources have subscribed to the events of their drivers before the driver receives the first message. It's then the resources responsibility to cache information tht delivered by the driver, until other modules depending on this information register themselves to the resource to fetch it. 
