# Drivers OPC UA

The OPC UA driver can browse, read, write and subscribe nodes.
Because of the maximum number of layers given by json, the driver can only show up to 5 node layers in the UI. 
All nodes will still be browsed and can be written or read.  

## Configuration

- `OpcUaServerUrl`: Contains the address of the server. 
  - Default protocol is `opc.tcp://` 
- **Log on**: Right now the OPC UA server only supports log on via user name and password
- `FilePathClientConfig`: A config `.xml` is required. By default it is expected to be
  `Config/Opc.Ua.Default.Config.xml`. The default file is provided by the package.

### Certificate

In order to use the OPC UA driver, the server has to accept all certificates. The same goes for the driver.


## How to handle nodes

The nodes are organized using the expanded node id. If you want to find the right node, please always use the expanded nodeId instead of the simple one. You can write and read nodes by using the interface `IInOutDriver`.
```
var input = driver.Input[<extendedNodeId>];
driver.Output[<extendedNodeId>] = value;
```
