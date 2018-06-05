Marvin.Communication.Sockets {#platform-CommunicationSockets}
========

Marvin.Communication.Sockets provides two implementations of IBinaryConnection to communicate via TCP/IP as a server or a client.

# Configuration

- To open a server-socket and start listening on incoming connections the user has to use the TcpListenerConfig
- To start a client-connection the user has to use the TcpClientConfig

## Server Mode
The IBinaryConnection will represent a TcpListener-Object. TcpListener-objects are created as transient objects by the DI-Container. TcpListeners are generic objects that use an IBinaryHeader as the generic (e.G. the SumsHeader for the PhoenixPlcDevice). By knowing the header, the TcpListeners can request the TcpPortListener to read the whole header at once. It is not necessary to read byte-wise from the connection. Once a header is read, the payload can also be read blockwise.
The TcpListener will then instruct the TcpServer to open a TCP-Socket and start listening. If the TCP-Socket has not already been opened, the TcpServer will create a TcpPortListener which opens the socket, creates a list of interested TcpLister-objects and starts listening. 
The TcpPortListener and the the TcpServer are also generic and use an IBinaryHeader as the generic (again: e.G. the SumsHeader for the PhoenixPlcDevice)
Therefore the TcpServer and the TcpPortListener know what byte-structure they have to expect, when a new client connects. They are able to read a whole header blockwise and may validate the header.
If a Client connects to the socket with a complete an valid header that is assigned to this TcpPortListener, the TcpPortListenr will prompt all its listed TcpListeners to see who is responsible for handling the new connection. To do this, each TcpListner provides a validator that will validate the bytestream and may also interpret it.
Because the TcpServer-class is a generic, that uses an implentation of IBinaryHeader as the generic, a TCP-Port can only be used of one specific TcpServer<IBinaryHeader>-implementation. That means, if a Tcp-Port has been assigned to a PhoenixPlcDevice it my not be assigned to another class of devices.

## Client Mode
The IBinaryConnection will represent an TcpClientConnection-Object. TcpClientConnections are created as transient objects by the DI-Container. They will open a connection to the configured server. If the connection disconnects the TcpClientConnection can be configured to immediately reconnect.
TcpClientConnections are generic objects that use an IBinaryHeader as the generic (e.G. the SumsHeader for the PhoenixPlcDevice). By knowing the header, the TcpClientConnections can try to read the whole header at once from a Tcp connection. It is not necessary to read byte-wise from the connection. Once a header is read, the payload can also be read blockwise.

# Byte-Order

The byte-order for communicating with other devices, e.g. PhoenixPlc-devices, shall be the Network-Byte-Order.
- The Network-Byte-Order is similar to Big-Endian or the Motorola byte-order. It is used an [Motorola](https://en.wikipedia.org/wiki/Motorola_6800) - or [ARM](https://en.wikipedia.org/wiki/ARM_architecture)-based systems
- Intel-Systems e.g. use Little-Endian and thereof have to convert data to the Network-Byte-Order before sending.
- See also https://en.wikipedia.org/wiki/Endianness or https://de.wikipedia.org/wiki/Byte-Reihenfolge

The following table illustrates differences between Little-Endian and Big-Endian if the number 1.234.567.890 (0x499602D2) is transformed into a bytearray (Table from [MSDN](https://msdn.microsoft.com/en-us/library/vstudio/system.bitconverter%28v=vs.100%29.aspx))

| Byte-order | Platformsamples | HEX-Representation |
|------------|-----------------|--------------------|
| Little-Endian | Intel | D2-02-96-49 |
| Big-Endian/Network-Byte-Order | Motorola, PhoenixPlc | 49-96-02-D2 |

# .NET Conversion-Methods
The .Net-Framework offers serveral Helper Methods and Classes and should be used for the conversion of data before sending it via network. To make sure, that multibyte values have the right byte order before being converted in to a byte-stream use [IpAdress.HostToNetworkOrder](https://msdn.microsoft.com/de-de/library/653kcke1%28v=vs.110%29.aspx) For incoming data [IpAddress.NetworkToHostOrder](https://msdn.microsoft.com/de-de/library/system.net.ipaddress.networktohostorder%28v=vs.100%29.aspx) maybe used.

To create Byte-Arrays the .Net-class [BitConverter](https://msdn.microsoft.com/en-us/library/vstudio/system.bitconverter%28v=vs.100%29.aspx) may be used after the multibyte values have been converted into the right order.