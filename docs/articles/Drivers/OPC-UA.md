# Drivers OpcUa

An OpcUa Driver, which can browse, read, write and subscribe nodes.
Because of th maximum number of layers given by json, the driver can only show up to 5 node layers in the UI. 
All nodes will still be browsed and can be written or read.  

## Configuration
In order to use the OPC UA driver, the server has to accept all certificates. The same goes for the driver.
- **OpcUaServerUrl**: Contains the IP Address of the server. Do not add `opc.tcp:\\`, since it will be added automatically. 
- **OpcUaServerPor**: Contains the port of the server. If the path of the opc ua isn't `opc.tcp:\\<IpAdress>:<Port>`, then fill `-1` for the server port and write the whole address into the field *OpcUaServerUrl*
- **Log on**: Right now the OPC UA server only supports log on via user name and password

## How to handle nodes
The nodes are organized using the expanded node id. If you want to find the right node, please always use the expanded nodeId instead of the simple one. You can write and read nodes by using the interface `IInOutDriver`.
```
var input = driver.Input[<extendedNodeId>];
driver.Output[<extendedNodeId>] = value;
```
