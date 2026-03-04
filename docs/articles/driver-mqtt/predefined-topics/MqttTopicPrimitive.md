# MqttTopicPrimitive

The primitive topic type allow serializing and deserializing primitives like integers, strings, floats and so on to bytes. It creates binary output and not a text representation. For example when sending then int16 38943 it won't send '3','8','9','4','3' but just the two bytes that represent it in memory. '00011111' and '10011000' on little endian systemss.
Strings will be UTF-8 Encoded.

It only has one configuration option besides those inherited from MqttTopic and that's **MessageNameEnum**.
It holds the Code for the expected message type.
