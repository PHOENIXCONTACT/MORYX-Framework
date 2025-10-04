---
uid: ProcessDataMonitor
---
# ProcessDataMonitor

The ProcessDataMonitor is the central module to collect process data from all sources provided in the MORYX process. The module does summarize all process data but does not handle them and there is no persistence. There is also an API to listen to the occured process data and handle them like write it to external systems.

## Provided Facades

This modules exports the `IProcessDataMonitor` facade.

````cs
/// <summary>
/// Central facade to collect process data. Will be implemented by the ProcessDataMonitor module
/// </summary>
public interface IProcessDataMonitor
{
    /// <summary>
    /// Adds a single measurement
    /// </summary>
    void Add(IMeasurement measurement);

    /// <summary>
    /// Adds a full measurand which is containing multiple measurements
    /// </summary>
    void Add(IMeasurand measurand);
}
````

## Referenced Facades

- There is no referenced facade

## Data Models

- There is no datamodel dependency

## Architecture

![ProcessDataMonitor Architecture](images/ProcessDataMonitor.png)

The PDM is implemented as simple as possible. It implements the `IProcessDataMonitor` facade. The actions of the facade are linked to the `IProcessDataCollector`.

### ProcessDataCollector

The `ProcessDataCollector` is the central component within the module. It handles configured listeners and is responsible to publish events for the listeners if data occures. 

### ProcessDataListener

`ProcessDataListener`'s are listening to events of the `ProcessDataCollector` and handle them on their own way. As a sample we can hava a look on a listener which is writing process data to the internal module logger:

````cs
[ExpectedConfig(typeof(ProcessDataListenerConfig))]
[Plugin(LifeCycle.Transient, typeof(IProcessDataListener), Name = nameof(LogListener))]
public class LogListener : ProcessDataListenerBase<ProcessDataListenerConfig>
{
    protected override void OnMeasurandAdded(IMeasurand measurand) =>
        measurand.Measurements.ForEach(OnMeasurementAdded);

    protected override void OnMeasurementAdded(IMeasurement measurement)
    {
        var strBuilder = new StringBuilder();
        strBuilder.Append($"{measurement.TimeStamp}: {measurement.Measurand}: Values (");

        for (var index = 0; index < measurement.Fields.Count; index++)
        {
            var field = measurement.Fields[index];
            strBuilder.Append($"{field.Name}: {field}");

            if (index < measurement.Fields.Count - 1)
                strBuilder.Append(", ");
        }

        strBuilder.Append(") Tags (");

        for (var index = 0; index < measurement.Tags.Count; index++)
        {
            var tag = measurement.Tags[index];
            strBuilder.Append($"{tag.Name}: {tag.Value}");

            if (index < measurement.Tags.Count - 1)
                strBuilder.Append(", ");
        }

        strBuilder.Append(")");
        Logger.LogEntry(LogLevel.Trace, strBuilder.ToString());
    }
}
````

This listener plugin produces the following sample output:

````txt
16.01.2020 14:07:35: controlSystem_processes: Values (id: 6029313, success: True, cycleTimeMs: 72,1918) Tags (productIdent: 123456, productRev: 1)
````

Each listener must be configured for existing measurands. So every measurand can be enabled or disabled. New measurands are **disabled** by default and must be enabled explicit for each listener.

## Data Structure

The implemented data structure is derived from the famous *InfluxDB*. We dont have invented the wheel again, we just implemented the key concepts of it.

**IMeasurement**

The measurement acts as a container for tags, field and the time. The measurement name is the description of the data. It is devided into **fields** and **tags**.

*Field* values are your data. They can be strings, floats, integers, or Booleans. Measurements are always associated with a timestamp because process data has always a reference to time.

Tags are optional. You don’t need to have tags in your data structure, but it’s generally a good idea to make use of them.

When to use fields vs. tags?

- Store data in tags if they are commonly-queried meta data
- Store data in tags if you plan to use them with GROUP BY
- Store data in fields if you plan to use them with an query language function
- Store data in fields if you need them to be something other than a string - tag values are always interpreted as strings

Usefull concept descriptons:

- [InfluxDB key concepts](https://docs.influxdata.com/influxdb/v1.7/concepts/key_concepts/)
- [InfluxDB schema design and data layout](https://docs.influxdata.com/influxdb/v1.7/concepts/schema_and_data_layout/)

**IMeasurand**

A measurand is more or less unknown within the InfluxDB concept. It is a collector for measurements. It can have a list of measurements.

## Adapter

The PDM does not have any facade dependencies to other module to secure the life cycle under each other and make it fully independent from other modules. Instead, adapter modules are required to map foreign facade events to the process data monitor.

A sample for the process data monitor is a module which is referencing the `IProcessControl` facacde and also the `IProcessDataMonitor` facade. The task for this module is to convert facade events from the `IProcessControl` to measurements for the PDM.

![ProcessEngine Adapter](images/sampleAdapterOrchestration.png)

This architecture makes the PDM fully independet. Adapters are named like the adopted module with the prefix "Pdm". This is just a naming convention and makes it more easy to identify adapter modules.

### PdmControlSystemKernel

The ControlSystem adapter is used to convert `IProcessControl` facade events and convert them into process data monitor measurement structure.

#### Measurements

**controlSystem_processes**

| Name | Type | Sample | Description |
|------|------|--------|-------------|
| id | Field | 4711 | Id of the process |
| success | Field | true/false | Indicator if the process was success or not |
| cycleTimeMs | Field | 4242 | Time in ms from the first activity start and completion of the last activity |

**controlSystem_activities**

| Name | Type | Sample | Description |
|------|------|--------|-------------|
| id | Field | 4711 | Id of the activity |
| success | Field | true|false | Indicator if the activity was success or not |
| process | Field | 8877 | Id of the process |
| runtimeMs | Field | 757 | Time in ms from the activity start to completion |
| type | Tag | BenchmarkActivity | Type name of the activity |

The existing measurments can be extendet with the known binding API. There are two configuration options within the `ModuleConfig`: `ProcessBindings` and `ActivityBindings`. Both are configured the same. The binding engine will be used to configure additional fields and tags. As a sample we add the product identifier to the *controlSystem_processes* measurement:

````json
{
  "ProcessBindings": [
    {
      "Name": "productIdent",
      "Binding": "Product.Identifier",
      "ValueTarget": "Tag"
    }
  ]
}
````

Valid binding root objects are `Process`, `Recipe`, `Product` and `Article`.

Next a sample for tracing information for *controlSystem_activities*:

````json
{
  "ActivityBindings": [
    {
      "Name": "errorCode",
      "Binding": "Tracing.ErrorCode",
      "ValueTarget": "Field"
    }
  ]
}
````

Valid binding root objects are `Activity`, `Tracing`, `Process`, `Recipe`, `Product` and `Article`.

### PdmResourceManagement

The ResourceManagement adapter is really simple. It uses the `IResourceManagementFacade` and also the `IProcessDataMonitor`. Instead of converting facade events to measurements, resources can implement the interface `IProcessDataPublisher` and can throw the `ProcessDataPublished` event. The event arguments are directly transfered to the process data monitor.

### PdmNotificationPublisher

The NotificationPublisher adapter is using the `INotificationPublisher` facade and also the `IProcessDataMonitor`. Notifications which are published and also acknowledged by the publisher.

**notifications_acknowledged**

| Name | Type | Sample | Description |
|------|------|--------|-------------|
| id | Field | 4711 | Guid of the notification |
| duration | Field | 6645 | Time from publishing to acknowledging the notification |
| severity | Tag | Error | Severity of the notification |
| type | Tag | CellStateNotification | Type name of the notification |

The existing measurment can also be extendet with the known binding API. There is one configuration option within the `ModuleConfig`: `NotificationBindings`.
The binding engine will be used to configure additional fields and tags. The only available base key is `Notification`.
As a sample we add the acknowledger to the *notification_acknowledged* measurement:

````json
{
  "NotificationBindings": [
    {
      "Name": "acknowledger",
      "Binding": "Notification.Acknowledger",
      "ValueTarget": "Tag"
    }
  ]
}
````

### PdmOrderManagement

The OrderManagement adapter is listening on events from the `IOrderManagement` and converts them to measurements and give them to the `IProcessDataMonitor`.

Currently the following events are converted and published as measurements: OperationStateChanged, OperationProgressChanged, MachineStateChanged.

**orders_operationProgress**

This measurement contains all operation relevant production values:

| Name | Type | Sample | Description |
|------|------|--------|-------------|
| operation | Tag | 0020 | Number of the operation |
| order | Tag | 3005488566 | number of the order |
| productIdent | Tag | 0020 | Identifier of the produced product |
| productRev | Tag | 1 | number of the revision |
| running | Field | 15 | Running count of the operation |
| success | Field | 15 | Success count of the operation |
| failure | Field | 15 | Failure count of the operation |
| reworked | Field | 15 | Reworked count of the operation |
| scrap | Field | 15 | Scrap count of the operation |
| pending | Field | 15 | Pending count of the operation |

**orders_operationStates**

This measurement contains all state changes of an operation:

| Name | Type | Sample | Description |
|------|------|--------|-------------|
| classification | Field | 2 | Classification of the state |
| pending | Field | 20 | Pending amount of parts to produce |
| name | Tag | Running | Name of the state |
| operation | Tag | 0020 | Number of the operation |
| order | Tag | 3005488566 | number of the order |

## InfluxDb Listeners

There is a InfluxDb listener which is pushing the measurements to an influx database for further processing. The listener requires the following configuration properties:

````cs
[DataMember, DefaultValue("localhost")]
public string Host { get; set; }

[DataMember, DefaultValue(8086)]
public int Port { get; set; }

[DataMember]
public string DatabaseName { get; set; }
````

For communication the default .NET InfluxDb LineProtocol is used. Sample meaurement series:

````sh
> select * from controlSystem_processes LIMIT 10
name: controlSystem_processes
time                application   cycleTimeMs id      productIdent productRev success
----                -----------   ----------- --      ------------ ---------- -------
1579102606824154900 MoryxRuntime 4872.8354   5701635 123456       1          true
1579102608562618800 MoryxRuntime 4807.6647   5701646 123456       1          true
1579102608628793100 MoryxRuntime 5433.1959   5701647 123456       1          true
1579102609242409400 MoryxRuntime 6024.7548   5701648 123456       1          true
1579102610095657100 MoryxRuntime 5113.3532   5701649 123456       1          true
1579102611631702800 MoryxRuntime 6442.8556   5701650 123456       1          true
1579102611684842600 MoryxRuntime 4756.4134   5701651 123456       1          true
1579102613845534900 MoryxRuntime 6168.1316   5701652 123456       1          true
1579102614844550800 MoryxRuntime 6211.245    5701653 123456       1          true
1579102615172939300 MoryxRuntime 7442.3973   5701654 123456       1          true
````
