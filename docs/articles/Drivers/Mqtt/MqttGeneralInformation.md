# Mqtt
MQTT is a protocol developed by IBM in the end of the 1990s. It is often used in the context of IoT.

## The Protocol
- binary: alle messages will be sent as byte-arrays
- push-based communication
- is based on a publish/subscribe-achitecture

## Topic
- hierarchical structure
- delimiter: `/`
- in order to subscribe a topic including all its subtopics use `#`: For example `Deathstart/#`

## Quality of Server Lever (QOS)
The QOS represents the importance of a message. It defines how often a sent transmitted message is received by the receiver.
  > QOS 0: at most once <br/>
  > QOS 1: at least once <br/>
  > QOS 2: exactly one <br/>

Since QOS 2 needs a lot of overhead in order to be implemented, excessive use hinders the leightweighness of the protocoll.
 
## Retained Messages
If a message is marked as retained, the broker will save the last retained message on its topic. If someone new subscribes to this topic, he will immediatly get the last retained message on the topic.

## Last Will and Testament
Clients can leave a will with the broker. If a client unintentionally loses the connection, the broker will publish a specific message on a specific topic. Message and topic have to be defined in the will.

## Persistent Session
If a client requested a persistent session and unwillingly disconnects, the broker will save all messages published on the topics the client subscribed. On reconnection the client will get all missed messages.

## Further information
- https://inductiveautomation.com/resources/article/what-is-mqtt
- https://www.hivemq.com/mqtt-essentials/