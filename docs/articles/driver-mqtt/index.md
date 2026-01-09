# MORYX MQTT

This repository contains tools for communication with resources via MQTT. <br/>
[MQTT](MqttGeneralInformation.md) is a light weight protocol using the publish/subscribe-architecture.

### Add a MQTT-Driver
In order to communicate with a resource using MQTT, first you need a [MQTT-Broker](SetUpTestEnvironment.md). Then you have to create a [DriverMqtt](DriverMqtt.md):
1. Run *Application.sln* and *Application.UI.sln*
2. Open the tab *Resources* in the UI
3. Create a new DriverMqtt
4. Add topics to the driver by selecting it and pressing the button *Add*

### Select the right topic
The [topic](MqttTopics.md) you have to use depends on the datatype of your messages. The already existing topic-types can serialize and deserialize the following data types:
- **MqttTopicIByteSerializable**: for classes derived from IByteSerializable
- **MqttTopicJson**: for classes which will be send as Json objects
- **MqttTopicPrimitive**: for primitive data types and strings

When creating a topic, you have to chose the datatype of the sending and receiving messages.

## Code Example
Using the driver:
```cs
public class ExampleResource: IResource
{
    [ResourceReference(ResourceRelationType.Driver)]
    public IMessageDriver Driver { get; set; }

    [DataMember, EntrySerialize]
    public string SensorTopic { get; set; }

    private IMessageChannel _channel;

    protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        await base.OnInitializeAsync(cancellationToken);

        // Subscribe all topics, which receive IIdentifiermessage
        Driver.Received += OnDriverReceived;
        // Or just a specific topic with all its messages
        _channel = Driver.Channel(SensorTopic);
        _channel.Received += OnSensorReceived;
    }

    private void OnDriverReceived(object driver, IIdentifierMessage msg)
    {
    }

    private void OnSensorReceived(object driver, IConvertible msg)
    {
       var value = (bool)msg;
    }
}

```
Using only the topic:
```cs
public class MqttSensorResource : PublicResource
{
    public IMessageChannel SensorTopic { get; set; }

    protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        await base.OnInitializeAsync(cancellationToken);

        SensorTopic.Received += OnSensorReceived;
    }

    private void OnSensorReceived(object driver, IConvertible msg)
    {
       var value = (bool)msg;
    }
}
```

For more code examples, look into the documentation of the [DriverMqtt](DriverMqtt.md) and the [MqttTopic](MqttTopics.md).
