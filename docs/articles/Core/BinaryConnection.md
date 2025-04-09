---
uid: BinaryConnection
---
# BinaryConnection

Moryx.Communication.Sockets provides two implementations of IBinaryConnection to communicate via TCP/IP as a server or a client.

## Usage

Below are two examples of protocol implementations using the BinaryConnection APIs. The first one is the IPS header based protocol and the second one is an Artificial delimiter based protocol. Apart from the `IMessageInterpreter` and `IMessageValidator` implementations the API usage is similar.

## Protocol implementation

To implement your protocol you must create two classes: a validator and an interpreter. The interpreter is your protocol parser used to extract messages from the byte stream. The validator will check the messages and is also used to assign a socket connection to a listener in case of multiple device connections on one port. If your protocol is based on headers or delimiters you must simply derive from the base implementation.

### MessageInterpreter for Header-Protocols

For header based protocols simply derive from `HeaderMessageInterpreter<T>` where `T` is the type of your header class. For the IPS-header please have a look at IpsHeader.cs(info). The base class will first read and parse the header. Then it will extract the payload length from the header and read the payload. If all is complete the binary connection publishes the message of type BinaryMessage<THeader>.

````cs
public class MyHeaderInterpreter : HeaderMessageInterpreter<MyHeader>
{
    protected override int HeaderSize => return MyHeader.HeaderLength;

    protected override int FooterSize => 0;

    private static MyHeaderInterpreter _instance;
    public static MyHeaderInterpreter Instance
    {
        get { return _instance ?? (_instance = new MyHeaderInterpreter()); }
    }
}
````

### MessageInterpreter for Delimiter-Protocols

For delimiter based protocols derive from `DelimiterMessageInterpreter` and provide an appropriate `BufferSize` and `ReadSize` as well as the byte sequence of your delimiters. Since you might have more than one device it is good practice to provide a singleton instance of your protocol. To avoid double instantiation you should make the default constructor private. When selecting BufferSize and ReadSize please keep the following in mind.

**ReadBuffer considerations:**

| Value | Formula | Explaination |
|-------|---------|--------------|
| Minimum size | (BiggestMessage - 1) + ReadSize | The buffer must be able to store a full message. In case you read the biggest message except for one byte. If the next message is already available you must be able to fit the ReadSize into the remaining buffer. |
|Recommended size (small messages) | AverageMessage * (Messages/Second) | If you receive multiple small messages in a short time it might make sense to try reading a lot of them in  at once. |
|Recommended size (big messages) | BiggestMessage * 2 | To use the benifit of the interpreters message splitting of sequential messages it makes sense to leave space for a second message in the buffer. |
|Maximum size | FreeIdleMem / (20 * DeviceCount) | Because the buffer allocation is static the tcp buffer should at most consume 5% of the machines available memory. FreeIdleMem refers to the available memory once when all applications are running in idle state. For example a 8GB system might consume 3GB with idle Runtime and PostgreSQL. This leaves 5GB free memory of which 5% are ~268MB. For 5 devices this comes to a maximum buffer size of ~54MB. |

**ReadSize considerations:**

| Value | Formula | Explaination |
|-------|---------|--------------|
|Minimum size | 1 | Well, I guess thats obvious. |
|Recommended size (low latency) | AverageMessage * 2 | For low latency you want to publish received messages as soon as possible. Therefor try to avoid reading multiple messages at once. Just leave enough space to start reading a follow up message while completing the current message.|
|Recommended size (high latency) | BiggestMessage | For low latency make sure to read each message in a single cycle if possible. That makes transmission for the sending side easier. |
| Maximum size | (BufferSize / 2) | In case the first read did not result in a full message you must have enough space to store full read size in the buffer. |

````cs
public class HtmlInterpreter : DelimitedMessageInterpreter
{
    private static HtmlInterpreter _instance;

    /// <summary>
    /// Singleton instance
    /// </summary>
    public static HtmlInterpreter Instance => _instance ?? (_instance = new HtmlInterpreter());

    /// <summary>
    /// 1 MB read buffer size for each connection
    /// </summary>
    protected override int BufferSize => 1048576;

    /// <summary>
    /// Number of bytes to read in each iteration
    /// </summary>
    protected override int ReadSize => 10240;

    /// <summary>
    /// Byte sequence for start of message
    /// </summary>
    protected override byte[] StartDelimiter => Encoding.UTF8.GetBytes("<html>");

    /// <summary>
    /// Byte sequence for end of message
    /// </summary>
    protected override byte[] EndDelimiter => Encoding.UTF8.GetBytes("</html>");
}
````

## Message validation

Validation is similar for both types of protocols. The only difference is that header based protocols must cast the message first to `BinaryMessage<THeader>`. Make sure to return the singleton instance of your protocol in the validator.

````cs
public class MyMessageValidator : IMessageValidator
{
    public IMessageInterpreter Interpreter => IpsHeaderInterpreter.Instance;

    public bool Validate(BinaryMessage message)
    {
        var ipsHeader = ((BinaryMessage<MyHeader>)message).Header;
        return ipsHeader.ModeID == 401;
    }
}
````

## Putting it all together

Finally all you have to do is create a connection instance with the IBinaryConnectionFactory and register to the events. Basic structure looks like this:

### Configuration

- To open a server-socket and start listening on incoming connections the user has to use the TcpListenerConfig
- To start a client-connection the user has to use the TcpClientConfig

### Server Mode

The `IBinaryConnection` will represent a `TcpListener`-Object. `TcpListener`-objects are created as transient objects by the DI-Container. `TcpListeners` are generic objects that use an `IBinaryHeader` as the generic. By knowing the header, the `TcpListeners` can request the `TcpPortListener` to read the whole header at once. It is not necessary to read byte-wise from the connection. Once a header is read, the payload can also be read blockwise.
The `TcpListener` will then instruct the TcpServer to open a TCP-Socket and start listening. If the TCP-Socket has not already been opened, the `TcpServer` will create a TcpPortListener which opens the socket, creates a list of interested `TcpLister`-objects and starts listening. The `TcpPortListener` and the the TcpServer are also generic and use an `IBinaryHeader` as the generic. Therefore the TcpServer and the `TcpPortListener` know what byte-structure they have to expect, when a new client connects. They are able to read a whole header blockwise and may validate the header.
If a Client connects to the socket with a complete an valid header that is assigned to this `TcpPortListener`, the `TcpPortListenr` will prompt all its listed `TcpListeners` to see who is responsible for handling the new connection. To do this, each `TcpListner` provides a validator that will validate the bytestream and may also interpret it.
Because the `TcpServer`-class is a generic, that uses an implentation of IBinaryHeader as the generic, a TCP-Port can only be used of one specific `TcpServer<IBinaryHeader>`-implementation. That means, if a Tcp-Port has been assigned to a PhoenixPlcDevice it my not be assigned to another class of devices.

### Client Mode

The `IBinaryConnection` will represent an `TcpClientConnection`-Object. `TcpClientConnections` are created as transient objects by the DI-Container. They will open a connection to the configured server. If the connection disconnects the `TcpClientConnection` can be configured to immediately reconnect.
`TcpClientConnections` are generic objects that use an `IBinaryHeader` as the generic. By knowing the header, the `TcpClientConnections` can try to read the whole header at once from a Tcp connection. It is not necessary to read byte-wise from the connection. Once a header is read, the payload can also be read blockwise.

### Sample

````cs
public class BinaryConnectionTester : IDevice
{
    // Injected factory
    public IBinaryConnectionFactory ConnectionFactory { get; set; }

    private IBinaryConnection _connection;

    public void Initialize(DeviceConfig config)
    {
        _config = (BinaryDeviceConfig)config;
        _connection = ConnectionFactory.Create(_config.CommunicatorConfig, new DummyValidator());
        _connection.Received += OnReceived;
    }

    private void OnReceived(object sender, BinaryMessage e)
    {
    }

    public void Send(DeviceMessage message)
    {
        _connection.Send(new BinaryMessage { Payload = message.Payload.ToBytes() });
    }
}
````

## Byte-Order

The byte-order for communicating with other devices, e.g. PhoenixPlc-devices, shall be the Network-Byte-Order.

- The Network-Byte-Order is similar to Big-Endian or the Motorola byte-order. It is used an [Motorola](https://en.wikipedia.org/wiki/Motorola_6800) or [ARM](https://en.wikipedia.org/wiki/ARM_architecture)-based systems
- Intel-Systems e.g. use Little-Endian and thereof have to convert data to the Network-Byte-Order before sending.
- See also [Endianness](https://en.wikipedia.org/wiki/Endianness)

The following table illustrates differences between Little-Endian and Big-Endian if the number 1.234.567.890 (0x499602D2) is transformed into a bytearray (Table from [MSDN](https://msdn.microsoft.com/en-us/library/vstudio/system.bitconverter%28v=vs.100%29.aspx))

| Byte-order | Platformsamples | HEX-Representation |
|------------|-----------------|--------------------|
| Little-Endian | Intel | D2-02-96-49 |
| Big-Endian/Network-Byte-Order | Motorola, PhoenixPlc | 49-96-02-D2 |

### .NET Conversion-Methods

The .Net-Framework offers serveral Helper Methods and Classes and should be used for the conversion of data before sending it via network. To make sure, that multibyte values have the right byte order before being converted in to a byte-stream use [.HostToNetworkOrder](https://msdn.microsoft.com/de-de/library/653kcke1%28v=vs.110%29.aspx) For incoming data [IpAddress.NetworkToHostOrder](https://msdn.microsoft.com/de-de/library/system.net.ipaddress.networktohostorder%28v=vs.100%29.aspx) maybe used.

To create Byte-Arrays the .Net-class [BitConverter](https://msdn.microsoft.com/en-us/library/vstudio/system.bitconverter%28v=vs.100%29.aspx) may be used after the multibyte values have been converted into the right order.