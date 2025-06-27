# Simulation Driver

If none of the existing, commercial drivers from `Moryx.Drivers.Simulation` fits your requirements, you can easily implement your own driver. All you need to do is implement the interface `Moryx.Simulation.ISimulationDriver` as well as the interface of the driver you want to replace like `IMessageDriver<object>` or `IInOutDriver`.

A module like the commercial simulator will call `Ready` and `Result` on your instance as a replacement for the events usually received from real drivers and sensors. You need to translate them into values or events of you driver interface like `IMessageDriver.Received`. The methods references the activity which you can use to extract the relevant attributes like process id, carrier or product identifiers as needed by your cell. Process execution initiated by the drivers API like parameter values or `Send` methods need to be represented by the `SimulatedState`.

## Example of simulated driver

So here is an example of how your mock driver can implement `Moryx.Simulation.ISimulationDriver`:
``` csharp
    [ResourceRegistration]
    public class TestMockDriver : Driver, IMessageDriver<object>,ISimulationDriver {

        public bool HasChannels => false;

        public IDriver Driver => this;

        public string Identifier => Name;

        private SimulationState _simulatedState;
        // simulated state of the driver
        public SimulationState SimulatedState
        {
            get => _simulatedState;
            private set
            {
                _simulatedState = value;
                SimulatedStateChanged?.Invoke(this, value);
            }
        }

        // cell referenced by the driver
        [ResourceReference(ResourceRelationType.Driver, ResourceReferenceRole.Source)]
        public AssemblyCell Cell { get; set; }

        public IEnumerable<ICell> Usages => new[] { Cell };

        protected override void OnStart()
        {
            base.OnStart();
            // initial simulated state of the driver
            SimulatedState = SimulationState.Idle;
        }

        public IMessageChannel<TChannel> Channel<TChannel>(string identifier)
        {
            throw new NotImplementedException();
        }

        public IMessageChannel<TSend, TReceive> Channel<TSend, TReceive>(string identifier)
        {
            throw new NotImplementedException();
        }

        // Message received from the cell
        public void Send(object payload)
        {
            switch (payload)
            {
                case AssembleProductMessage assemble:
                    SimulatedState = SimulationState.Executing;
                    break;
                case ReleaseWorkpieceMessage release:
                    SimulatedState = SimulationState.Idle;
                    break;
            }
        }

        public Task SendAsync(object payload)
        {
            Send(payload);
            return Task.CompletedTask;
        }

        public void Ready(IActivity activity)
        {
            SimulatedState = SimulationState.Requested;

            Received?.Invoke(this, new WorkpieceArrivedMessage { ProcessId = activity.Process.Id });
        }

        public void Result(SimulationResult result)
        {
            Received?.Invoke(this, new AssemblyCompletedMessage { Result = result.Result });
        }

        public event EventHandler<object> Received;

        public event EventHandler<SimulationState> SimulatedStateChanged;

        //... rest of your codes//
    }
```

Note: The `Send()` method might differ from cell to cell.You can override the `Send()` and do your implementation of what needs to happend when your "mock" driver receives something from the cell. In case your driver doesn't have a `send()` method, you can use your own method and react upon the type of message you received just like described inside the `TestMockDriver.Send()` method above.

 # How does my Cell use my `MockDriver` ?

In your cell definition/Class you can use the following example based on the type of driver you're trying to simulate. Let's say we are trying to "mock" an `MQTT driver` since Moryx MQTT driver implements from `IMessageDriver<object>` in your cell you can have the following case:
```csharp
    // SolderingCell.cs
    
   [ResourceRegistration]
    public class SolderingCell : Cell
    {
        //... rest of your codes//

        [ResourceReference(ResourceRelationType.Driver, IsRequired = true)]
        public IMessageDriver<object> Driver { get; set; }

        //... rest of your codes//
    }
```

Since the our "mock" driver has the following property defined:
```csharp
    //... rest of the codes//

    /// <summary>
    /// Cell linked to the driver
    /// </summary>
    [ResourceReference(ResourceRelationType.Driver, ResourceReferenceRole.Source)]
    public AssemblyCell Cell { get; set; }

    //... rest of the codes//
```
Moryx will take care of the relationship once you assign the driver to the cell.
 To start the communication you can define message types in your resources folder. For starter we can define some message types. You can also use already existing messages
 ```csharp
    //AssembleProductMessage.cs

    /// <summary>
    /// Message specificto start an activity
    /// </summary>
    public class AssembleProductMessage
    {
        public long ActivityId { get; set; }
    }

    //AssemblyCompletedMessage.cs
    public class AssemblyCompletedMessage
    {
        public int Result { get; set; }
    }

    //ReleaseWorkpieceMessage.cs
    public class ReleaseWorkpieceMessage
    {
        //rest of the code...
    }

    //WorkpieceArrivedMessage.cs
    public class WorkpieceArrivedMessage
    {
        public long ProcessId { get; set; }
    }
 ```
 In the cell when you need to send a data to the driver you can do as described bellow:
 ```csharp
 //AssemblyCell.cs
    //... rest of the codes//

        public override void StartActivity(ActivityStart activityStart)
        {
            Driver.Send(new AssembleProductMessage { ActivityId = activityStart.Activity.Id });
        }

    //... rest of the codes//
 ```
 As you can see inside the `StartActivity` method we want the cell to send data to the driver.

 Now in our driver `TestMockDriver` class above. Inside the `public void Send(object payload)` method we are filtering the payload based on the message type that we just define. We can act upon the type of message received. For instance:
 ```csharp
    //... rest of the codes//

       public void Send(object payload)
        {
            switch (payload)
            {
                case AssembleProductMessage assemble:
                    SimulatedState = SimulationState.Executing;
                    break;
                case ReleaseWorkpieceMessage release:
                    SimulatedState = SimulationState.Idle;
                    break;
            }
        }
        
    //... rest of the codes//
 ```
