
# Using the Mqtt Driver
## The class DriverMqtt
The DriverMqtt is a resource which allows sending and receiving messages via MQTT. Its children (called Channels) are all derived from the abstract class MqttTopic and represent the topics, which the driver subscribes to and publishes on.

The following parameters are configurable in the UI:
- **Root**: This is written in front of the Identifiers of all MqttTopic-Resources. The MqttClient in the driver subsribes only to the specific topic defined in a MqttTopic-Resource, which is a child of the driver
- **BrokerURL**: IP oder URL or the MQTT Broker
- **Port**: Port of the MQTT Broker
- **MqttVersion**: MQTT protocol version. Possible versions are 3.1.0, 3.1.1 and 5.0.0. In versions lesser than 5 the driver will receive its own messages.

The main responsibility of a DriverMqtt is to manage the connection to the MQTT-Broker and allocate received messages to the corresponding MqttTopic-Resources. While sending messages, the driver uses the Identifier, if its an IIdentifierMessage, or the MessageType to finde the corresponding TopicResource. If you're using placeholders or wildcards, it's recommended to use unique Messagetypes for each MqttTopic-Resource and the driver for sending and receiving. That way the concrete topicstructure must not be known. 
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
// Both calls will return topicSensorA
var channel = driver.Channel<IByteSerializable>(_topicBoolMqtt.Identifier);
var channelA = driver.Channel<IByteSerializable,IByteSerializable>   (_topicBoolMqtt.Identifier);
```
 