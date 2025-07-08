# MORYX MQTT

<p align="center">    
    <a href="https://stackoverflow.com/questions/tagged/moryx">
        <img src="https://img.shields.io/badge/stackoverflow-ask-orange.svg" alt="Stackoverflow">
    </a>
    <a href="https://gitter.im/MORYX-Industry/Development?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge">
        <img src="https://badges.gitter.im/MORYX-Industry/Development.svg" alt="Gitter">
    </a>
</p>

This repository is based on the the [Moryx-Template](https://github.com/PHOENIXCONTACT/MORYX-Template) and contains tools for communication with resources via MQTT. <br/>
[MQTT](docs/MqttGeneralInformation.md) is a leight weight protocol using the publish/subscribe-archtitecture. 

### Add a MQTT-Driver
In order to communicate with a resource using MQTT, first you need a [MQTT-Broker](docs/SetUpTestEnvironment.md). Then you have to create a [DriverMqtt](docs\DriverMqtt.md):
1. Run *Application.sln* and *Application.UI.sln*
2. Open the tab *Resources* in the UI
3. Create a new DriverMqtt 
4. Add topics to the driver by selecting it and pressing the button *Add*

### Select the right topic
The [topic](docs\MqttTopics.md) you have to use depends on the datatype of your messages. The already existing topic-types can serialize and deserialize the following datatypes:
- **MqttTopicIByteSerializable**: for classes derived from IByteSerializable
- **MqttTopicJson**: for classes which will be send as Json objects
- **MqttTopicPrimitive**: for primitive datatypes and strings

When creating a topic, you have to chose the datatype of the sending and receiving messages.

## Code Example
Using the driver:
```C#
public class ExampleResource: IResource{
    IMessageDriver<IIdentifierMessage> Driver {get; set;}

    [DataMember, EditorBrowsable]
    public string SensorTopic { get; set; }

    private IMessageChannel<IConvertible> _channel;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        // Subscribe all topics, which receive IIdentifiermessage
        Driver.Received += OnDriverReceived;
        // Or just a specific topic with all its messages
        _channel = Driver.Channel<IByteSerializable>(SensorTopic);
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
```C#
public class MqttSensorResource : PublicResource
{
    public IMessageChannel<IConvertible> SensorTopic { get; set; }

    protected override void OnInitialize()
    {
        base.OnInitialize();

        SensorTopic.Received += OnSensorReceived;
    }

    private void OnSensorReceived(object driver, IConvertible msg)
    {
       var value = (bool)msg;
    }
}
```

For more code examples, look into the documentation of the [DriverMqtt](docs\DriverMqtt.md) and the [MqttTopic](docs\MqttTopics.md).
## Trouble Shooting

If you run into problems with this project or MORYX development in general, feel free to join our Gitter channel, ask on StackOverflow using the [`moryx`](https://stackoverflow.com/questions/tagged/moryx) tag or open an issue. In case your back-end application closes directly after start, this is mostly caused by lack of rights, reserved ports or missing libraries. MORYX creates crash log before exiting which can be find in the subdirectory *CrashLogs* in the *StartProjects* execution directory.

## Contribute

If you have an idea to improve the project or can think of a new useful topic, please make your changes based this project and open a pull request. 
