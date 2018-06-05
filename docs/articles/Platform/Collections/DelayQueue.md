---
uid: DelayQueue
---
# DelayQueue

The `DelayQueue` is an instrument to ensure always a correct message timing.

As an example you want to communicate via a serial port with an external device. But while testing you fastly experienced that you couldn't send messages to it as fast as your program or code can. The device needs a time of i.e. *100 ms* between each message, because otherwise it executes only the first message and doesn't understand any message that follows.

Wouldn't it be nice if there exists a simple component that ensures the correct timing between the message for you? Sure it is and that's what the `DelayQueue` is for.

## Usage

In this section the usage of the `DelayQueue` will be explained in reference to the example we mentioned in the introduction: Sending data through a serial port.

Let's assume you have got a component (which may can be configured) that handles the connection with the serial port and that sends your data through the port. This kind of component is shown in the code below.

Before you can start you'll need an IParallelOperations DI-object. Of course you'll need a message queue. For the queue you have to mention the type of the messages you want to send in between the diamond-operator: `<FooMessageData>`. After this you can create a new DelayQueue for your messages. Therefore you have to hand in the injected ParallelOperations object. The queue will be created as `IDelayQueue<FooMessageData> _queue = new DelayQueue<FooMessageData>(ParallelOperations);`.

After you created the queue you should better register to it's `Dequeued` event. The queue will automatically throw this event if your message is ready to be send. This is always the case (even it is the first message in the queue) if enough time has gone.

We will come to this later.

To make the queue work correctly, you have to call it's `Start` method. Otherwise it will never hand out any message to being send through the port. As a parameter you have to insert the waiting time (**in ms!**) between two messages. If the component finishes, don't forget to stop the queue.

````cs
public DelayDataSender : IConfiguredModulePlugin<FooConfig>
{
    #region DependencyInjection

    public IParallelOperations ParallelOperations { get; set; }

    #endregion

    private SerialPort _port;
    private IDelayQueue<FooMessageData> _queue;

    #region LifeCycle

    public void Initialize(FooConfig config)
    {
        _port = new SerialPort("PortName");
        _queue = new DelayQueue<FooMessageData>(ParallelOperations);
        _queue.Dequeued += OnMessageReadyToSend;
    }

    public void Start()
    {
        if(!_port.IsOpen) { _port.Open(); }
        _queue.Start(100);
    }

    public void Dispose()
    {
        _queue.Stop();
        if(_port.IsOpen) { _port.Close(); }
        _port.Dispose();
    }

    #endregion
}
````

Always if you want to send your `FooMessageData` through your data sender component, you have to call the method shown below. It enqueues your data into the `DelayQueue`.

````cs
public void SendFoo(FooMessageData data)
{
    _queue.Enqueue(data);
}
````

Below is the method which is called if a message is dequeued by the `DelayQueue`, because we registered it to the event when we wrote `_queue.Dequeued += OnMessageReadyToSend;`. Now we can directly send the data through the `SerialPort` (or at least whatever), because the `DelayQueue` ensured that we waited long enough between sending of two messages. That's all the magic.

````cs
private void OnMessageReadyToSend(object sender, FooMessageData message)
{
    _port.Write(message);
}
````