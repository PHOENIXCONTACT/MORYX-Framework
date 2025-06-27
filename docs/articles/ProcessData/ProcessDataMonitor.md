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


