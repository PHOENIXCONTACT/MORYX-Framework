# Using MqttTopics
## MqttTopic<TMessage>
This abstract class represents a MQTT-Topic and implements the interface `IMessageChannel<TMessage>`. It is derived from the abstract class MqttTopic, which implements `MessageChannel<object>`. The topic serializes and deserializes messages. Different data types need different implementations of `MqttTopic<TMessage>`.

## Already existing topic types
The already existing topic-types can serialize and deserialize the following data types:
- **MqttTopicIByteSerializable**: for classes derived from IByteSerializable
- **MqttTopicJson**: for classes which will be send as Json objects
- **MqttTopicPrimitive**: for primitive data types and strings
  
## Sending Messages via the topic
```C#
MqttDriver driver;
var _mqttTopicInt = new MqttTopicPrimitive()
{
    Identifier = "humiditySensor",
    ReceivedMessageName = "System." + nameof(Int32),
    SentMessageName = "System."+nameof(Int32),
};
_mqttTopicInt.Parent = driver;
_mqttTopicInt.Send(7);
```

## Receiving Messages
If a message is received, the received-Event will be raised.
```C#
_mqttTopicInt.Receive += OnReceived;
```
## Creating a new Topic
In order to create a new topic, the abstract class `IMqttTopic<TSend, TReceive>` has to be implemented. <br/>
If a dropdown menu for SentMessageName and ReceivedMessageName is wanted, the appropriate attributes have to be added to the properties:
```C#
[DataMember, EntrySerialize]
[PossibleTypes(typeof(IByteSerializable))]
public override string ReceivedMessageName { get; set; }
```
The methods `protected internal abstract byte[] Serialize(object payload)` and `protected internal abstract TReceive Deserialize(byte[] messageAsBytes)` are used to serialize and deserialize messages. <br/>
The base class will send the messages using the driver and raise the appropriate events. <br/>
The following code snippet is an exemplary implementation for JSON.
```C#
protected internal override byte[] Serialize(object payload)
{
    return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(payload));
}
```
```C#
protected internal override object Deserialize(byte[] messageAsBytes)
{
    var msg = Constructor();
    JsonConvert.PopulateObject(Encoding.UTF8.GetString(messageAsBytes, 0, messageAsBytes.Length), msg);
    return msg;
}
```

## Defining the MQTT Topic Name
It is possible to use placeholders (marked with {}) and Wildcards (+ or #) in topics. If you're using placeholders, TMessage must have a property with the same name. If your topic is for example *root/{PcName}/temperature/{SensorNumber}*, TMessage must have a property *PcName* and a property *SensorNumber*. <br/>
MqttTopic-Resources, which use wildcards in their topics, can only use implementations of `IIdentifierMessage` as TMessage.


