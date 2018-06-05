---
uid: BinaryConnection
---
# BinaryConnection

# QA - Quick Introduction
### Where can I find the Assembly ?
-	BinaryConnection is part of the Assembly **Marvin.dll**.
-	If you want to use binary connection functionalities in your client, you should use the Assembly from **"..\Build\ClientRuntime"**.
-	If you want to use the Assembly in a BackEnd component, then you should use the Assembly from **"..\Build\ServiceRuntime"**.
### Where can I find the .Net-Solution ?
-	Solution	:	**Toolkit.sln**
-	Project		:	**Marvin**
-	Namespace	:	**Marvin.Communication**
-	If you open **Marvin.Communication** you will see a Folder Structure which also describes the main components of binary connection.
	-	**Marvin.Communication.Connection** includes all interfaces needed to implement BinaryConnection Objects (Config, Connection, State, ConnectionFactory)
	-	**Marvin.Communication.Interpreter** includes all base classes to parse/interpret a Transmission Object (Interpreter, Context)
	-	**Marvin.Communication.Transmission** includes all base classes and interfaces needed to create the Transmission Object (Message, Header, Payload, Transmission, Serialize).
	-	**Marvin.Communication.Validation** includes an interfaces to validate a Message
### Where to find the Unit-Tests
- You can find the Tests in the Project `Marvin.Communication.Sockets.IntegrationTests` in the Solution `Toolkit`.
### What is Binary Connection?
-	Binary Connection is supposed to enable **all** kind of binary communication to a physical device (like a printer), a server or any other 3rd party entity.
-	Supports Header and Delimiter based Transmissions via TCP Payload.
-	Binary Connection works with a Header and a Payload. To prevent misunderstandings Header and Payload does not equate fully to the Header and Payload definition from TCP. BinaryConnection (can) uses the TCP Payload to store Header and Payload information. If our Transmission processing is asynchronous and we can easily identify the caller of a transmission by storing the reference of the caller into the Header.
-	Binary Connection also provides an event where an interested component can register to get notified when the connection state has changed.
### Where is the Protocol Implementation?
-	The Protocol defines a System of Communication Rules inside the Communication System.
-	To Translate a Byte Stream from the Transmission to a Message we use an Implementation of `IMessageInterpreter`.
-	To Validate if the Transmission is correct, we use an Implementation of `IMessageValidation`.
### What is Binary Message?
-	The Binary Message is the class which is used to send Transmissions using Binary Connection to a Device. 
### How to create an own Binary Connection Implementation?
-	Implement `IBinaryConnection` to your Connection Class (Namespace : `Marvin.Communication`).
-	Create a BinaryConnectionConfiguration Class.
-	Extend your Configuration Class with `BinaryConnectionConfig`. (Namespace : `Marvin.Communication`)
-	**Here we go!** Now Implement further, necessary functionalities you need to establish a proper Communication  or add additional Configuration Parameters to you Configuration
### How to create an own Binary Message Implementation?
-	Create a Validator class by implementing IMessageValidator
-	Create a Interpreter class by implementing IMessageInterpreter
	-	If you want to use a Header based Transmission then simply just extend your Message by HeaderMessageInterpreter
	-	If you want to use a Delimiter based Transmission simply extend your Message by DelimitedMessageInterpreter
### How did others implement?
-	[Marvin.Communication.Sockets](xref:Marvin.Communication.Sockets)
# Usage
Below are two examples of protocol implementations using the BinaryConnection APIs. The first one is the IPS header based protocol and the second one is an Artificial delimiter based protocol. Apart from the IMessageInterpreter and IMessageValidator implementations the API usage is similar.

## Protocol implementation
To implement your protocol you must create two classes: a validator and an interpreter. The interpreter is your protocol parser used to extract messages from the byte stream. The validator will check the messages and is also used to assign a socket connection to a listener in case of multiple device connections on one port. If your protocol is based on headers or delimiters you must simply derive from the base implementation.

### MessageInterpreter for Header-Protocols

For header based protocols simply derive from HeaderMessageInterpreter<T> where T is the type of your header class. For the IPS-header please have a look at IpsHeader.cs(info). The base class will first read and parse the header. Then it will extract the payload length from the header and read the payload. If all is complete the binary connection publishes the message of type BinaryMessage<THeader>.

````cs
public class IpsHeaderInterpreter : HeaderMessageInterpreter<IpsHeader>
{
    protected override int HeaderSize { get { return IpsHeader.HeaderLength; } }
    protected override int FooterSize { get { return 0; } }

    private static IpsHeaderInterpreter _instance;
    public static IpsHeaderInterpreter Instance
    {
        get { return _instance ?? (_instance = new IpsHeaderInterpreter()); }
    }
}
````

### MessageInterpreter for Delimiter-Protocols

For delimiter based protocols please derive from DelimiterMessageInterpreter and provide an appropriate BufferSize and ReadSize as well as the byte sequence of your delimiters. Since you might have more than one device it is good practice to provide a singleton instance of your protocol. To avoid double instantiation you should make the default constructor private. When selecting BufferSize and ReadSize please keep the following in mind. 

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
public class MetronicInterpreter : DelimitedMessageInterpreter
{
    private MetronicInterpreter()
    {
    }
    
    private static MetronicInterpreter _instance;
    /// <summary>
    /// Singleton instance
    /// </summary>
    public static MetronicInterpreter Instance
    {
        get { return _instance ?? (_instance = new MetronicInterpreter()); }
    }
    
    /// <summary>
    /// 100 kB read buffer size for each connection
    /// </summary>
    protected override int BufferSize
    {
        get { return 102400; }
    }

    /// <summary>
    /// Number of bytes to read in each iteration
    /// </summary>
    protected override int ReadSize
    {
        get { return 10240; }
    }

    /// <summary>
    /// Byte sequence for start of message
    /// </summary>
    protected override byte[] StartDelimiter
    {
        get { return Encoding.UTF8.GetBytes("<GP>"); }
    }

    /// <summary>
    /// Byte sequence for end of message
    /// </summary>
    protected override byte[] EndDelimiter
    {
        get { return Encoding.UTF8.GetBytes("</GP>"); }
    }
}
````

## Message validation

Validation is similar for both types of protocols. The only difference is that header based protocols must cast the message first to BinaryMessage<THeader>. Make sure to return the singleton instance of your protocol in the validator.

````cs
public class IpsMessageValidator : IMessageValidator
{
    public IMessageInterpreter Interpreter { get { return IpsHeaderInterpreter.Instance; } }

    public bool Validate(BinaryMessage message)
    {
        var ipsHeader = ((BinaryMessage<IpsHeader>)message).Header;
        return ipsHeader.ModeID == 401;
    }
}
````

## Putting it all together

Finally all you have to do is create a connection instance with the IBinaryConnectionFactory and register to the events. Basic structure looks like this:

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