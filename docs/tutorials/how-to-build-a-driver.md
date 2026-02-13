---
uid: HowToBuildADriver
---
# How to build a driver

A driver is the interface between other Resources and physical parts of the modelled system. This tutorial shows how a driver should be implemented.

## Basic driver files

A driver has this basic solution structure which can be extended for your needs:

````fs
-Moryx.Driver.ExampleDriver
|-IExampleDriver.cs
|-ExampleDriver.cs
````

The interface `IExampleDriver` is the API of the driver and important for registration within the AbstractionLayer. For a loose coupling between the resources it is best to use an existing driver interface or add an a new driver interface instead of using the implementation directly.

### The interface

This interface is simply derived from [IDriver](../../src/Moryx.AbstractionLayer/Drivers/IDriver.cs). No further definitions are needed.

````cs
public interface IExampleDriver : IDriver
{
}
````
If you are implementing a Driver that is sending messages, [IMessageDriver](../../src/Moryx.AbstractionLayer/Drivers/Message/IMessageDriver.cs) is probably the beter choice. An `IMessageDriver<TMessage>` can be used for a specific type of message. It contains several channels, which can represent for example Mqtt-Topics or OPC UA nodes.

### The implementation

Now implement `IDriver`:

````cs
using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;

namespace Moryx.Resources.Samples.DriverTutorial
{
    [ResourceRegistration]
    [DisplayName("Example Driver"), Description("An example driver")]
    public class ExampleDriver : Driver, IExampleDriver
    {
        [DataMember, EntrySerialize]
        public string AStringValue { get; set; }

        [DataMember, EntrySerialize]
        public int AnIntValue { get; set; }

        public string ANonEntrySerializeMember { get; set; }

        [EntrySerialize, DisplayName("Square"), Description("Just multiplies given value with itself")]
        public int Square(int value)
        {
            return value * value;
        }
    }
}
````

The implementation of the `ExampleDriver` derives from the [Driver](../../src/Moryx.AbstractionLayer/Drivers/Driver.cs) base class. It also implements the `IDriver` interface. The next important thing is the [ResourceRegistration attribute](../../src/Moryx.AbstractionLayer/Resources/Attributes/ResourceRegistrationAttribute.csResourceRegistrationAttribute). The AbstractionLayer can now identify this driver as a resource. Additional attributes like `DisplayName` and `Description` are used within the Resource UI.

The two properties `AStringValue` and `AnIntValue` are shown in the ResourceUI and can be edited by the user. The member `ANonEntrySerializeMember` is invisible for the user and is only used inside the AbstractionLayer.
The `Square` function is also visible in the Resource UI. And: It is callable from there.

## Lifecycle and StateMachine

The `ExampleDriver` is just a simple implementation for a driver. As like every [Resource](../../src/Moryx.AbstractionLayer/Resources/Resource.cs) you can `Initialize`, `Start`, `Stop` a driver. Also `State machine` support is built in:

````cs
[ResourceRegistration]
[DisplayName("StateExample Driver"), Description("An example driver that uses the state machine")]
public class StateExampleDriver : Driver, IExampleDriver, IStateContext
{
    ...

    private ExampleStateBase _state;

    private readonly object _stateLock = new object();

    /// <seealso cref="IDriver"/>
    public override async Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        await base.InitializeAsync(cancellationToken);

        StateMachine.Initialize<ExampleStateBase>(this).With<ExampleStateBase>();
    }

    ...

    public override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        await base.OnStartAsync(cancellationToken);

        lock(_stateLock)
            State.Connect();
    }
}
````

In order to create the State machine, first define a base type, the methods you need and your states. States are derived from `SyncStateBase` or `AsyncStateBase`. For drivers there exists a specific `SyncDriverState`/`AsyncDriverState`, which includes the methods `Connect()`/`ConnectAsync()` and `Disconnect()`/`DisconnectAsync()`. Typical States for a Driver are `Disconnected`, `Connecting` and `Connected`.

```cs
internal class ExampleStateBase : SyncDriverState<StateExampleDriver>
{
    protected MyStateBase(MyContext context, StateMap stateMap)
        : base(context, stateMap)
    {
    }

    // Here you will add all needed methods
    ...

    internal virtual void ConnectionLost()
    {
        //In order to change the State, use NextState()
        NextState(StateDisconnected);
    }

    [StateDefinition(typeof(DisconnectedState), IsInitial = true)]
    protected const int StateDisconnected = 10;

    [StateDefinition(typeof(ConnectingState))]
    protected const int StateConnecting = 20;

    [StateDefinition(typeof(ConnectedState))]
    protected const int StateConnected = 30;

}
```
All states are derived from the base state and contain the state specific implementations of the methods. If a state shouldn't be able to call a method, use `InvalidState()`.

```cs
internal class DisconnectedState : ExampleStateBase
{
    ...

    internal override void Send(object payload)
    {
        InvalidState();
    }

    ...
}
```

For further information about the configuration and implementation of the State machine look [here](/docs/articles/framework/state-machine.md).

## When to use a driver

If you want to communicate with the OT Layer in the outside world like a PLC, RFID scanner or bar code reader, implement it as a driver. For the communication with IT infrastructure like ERP systems please use Adapters.
