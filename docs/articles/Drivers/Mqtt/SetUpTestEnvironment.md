# Set-up Test environment
## MQTT-Broker mosquitto
- single-threaded Broker
- written in C
- supports up to 5000 connections
- open source, supported by the eclipse foundation
- https://mosquitto.org/

You can start the mosquitto-broker using the Taskmanager. After that you can publish and subcribe topis using mosquitto and the cmd.

## How to publish a message using mosquitto
1. open cmd
2. go to mosquitto folder using cd
3. mosquitto_pub -t *topicName* -m *message*

## How to subscibe a message using mosquitto 
1. open cmd
2. go to mosquitto folder using cd
3. mosquitto_sub -h *BrokerIp* -p *Brokerport* -t *topicName* 

If everthing is on the same PC, you don't need a Brokerport or ip while subscribing. <br/>
To subscribe a topic including all subtopics use # (for example deathstar/# subsribes the topic deathstar and all its subtopics).