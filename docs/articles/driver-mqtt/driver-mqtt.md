
# Using the Mqtt Driver
## The class DriverMqtt
The DriverMqtt is a resource which allows sending and receiving messages via MQTT. Its children (called Channels) are all derived from the abstract class MqttTopic and represent the topics, which the driver subscribes to and publishes on.

The following parameters are configurable in the UI:
- **Root**: This is written in front of the Identifiers of all MqttTopic-Resources. The MqttClient in the driver subscribes only to the specific topic defined in a MqttTopic-Resource, which is a child of the driver
- **BrokerUrl**: IP or URL of the MQTT Broker
- **Port**: Port of the MQTT Broker
- **MqttVersion**: MQTT protocol version. Possible versions are 3.1.0, 3.1.1 and 5.0.0. In versions lesser than 5 the driver will receive its own messages.
- **UseTls** Allows using MQTTs. The encrypted variant of MQTT
- **Username** and **Password** Are currently the only implemented authentication type to authenticate to the MQTT Broker
- **QualityOfService** MQTT Quality of Service level applied to each message send by the driver
- **ReconnectWithoutCleanSession** When not using a clean session an old session to the broker can be reused. This leads to retained messages that have already been received not being redelivered after a reconnect.
- **TraceMessageContent** the MQTT driver is one of the first MORYX components with Open Telemetry support. When enabling this flag, the content of each message is stored on the Open Telemetry span that represents processing the message. Only advisable for debug purposes
- **ReconnectDelayMs** interval between try attempts when the connection is lost
- **HasLastWill**, **LastWillTopic** and **LastWillContent** describe if a LastWill message should be registered on the broker that get's automatically send to all other subscribers when the driver disconnects. (LastWill messages are currently always marked as retain, because it's usally used as a connection state flag)


The main responsibility of a DriverMqtt is to manage the connection to the MQTT-Broker and allocate received messages to the corresponding MqttTopic-Resources. While sending messages, the driver uses the Identifier, if its an IIdentifierMessage, or the MessageType to find the corresponding TopicResource. If you're using placeholders or wildcards, it's recommended to use unique message types for each MqttTopic-Resource and the driver for sending and receiving. That way the concrete topic structure must not be known. 
<br/>IIdentifierMessage contains the property Identifier, which represents the topic relative to the root topic. If for example the root topic is *machine1/* and the identifier of the message is *sensorA*, then the corresponding topic, on which the message will be published, is *machine1/sensorA*.

```C#
var driver = new DriverMqtt
{
    Identifier = "machine1",
    Id = 4,
    BrokerURL = "127.0.0.1",
    Port = 1883,
    MqttVersion = MqttProtocolVersion.V500,
    Channels = new ReferenceCollectionMock<MqttTopic> 
        { topicSensorA, topicSensorB}
};
```

## Adding and getting a Channel
Channels can be used to directly send and receive messages, even if they're not derived from IIdentifierMessage or two topics use the same MessageType. To add a channel to a driver you can use the UI or add it directly to the property `public IReferences<MqttTopic> Channels { get; set; }`.

The following example shows, how to get an existing Channel:
```C#
var topicSensorA = new MqttTopicIByteSerializable
{
    Identifier = "sensorA",
    ReceivedMessageName = nameof(BoolMqttMessage),
    SentMessageName = nameof(BoolMqttMessage),
};
...
var channel = driver.Channel(_topicBoolMqtt.Identifier);
```
 
