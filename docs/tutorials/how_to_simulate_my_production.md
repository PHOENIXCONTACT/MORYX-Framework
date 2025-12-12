# Architecture

The following image shows how the simulation driver works.

![alt text](../articles/simulated_driver_architecture.jpg)

Architecture Design decision :
The Execution time and and Success rate are now inside the Module Configuration, it gives the flexibility to extend it for more accurate simulation.
We could feed in the list of previous executions and take the average execution time.


# What do i need?
For simulation driver to work, you need `Moryx.Simulation.ISimulationDriver`.

# Where do i find `Moryx.Simulation.ISimulationDriver` ?
`Moryx.Simulation.ISimulationDriver` exist in the Moryx.Simulation Repository/package.

# How do i use `Moryx.Simulation.ISimulationDriver` ?
1. `Moryx.Simulation.ISimulationDriver` has 2 methods `Ready()` and `Result()` that the Simulator uses to interact with your driver.
2. `Moryx.Simulation.ISimulationDriver` has 1 method `Send()` that you can use to send data from the cell through your "mock" driver.
``` csharp
/// ISimulationDriver.cs
 public interface ISimulationDriver : IDriver
    {
        /// <summary>
        /// Currently simulated state
        /// </summary>
        SimulationState SimulatedState { get; }

        /// <summary>
        /// Cells that reference this simulation driver
        /// and expect process events from it
        /// </summary>
        IEnumerable<ICell> Usages { get; }

        /// <summary>
        /// Send a message to the cell through the driver to simulate that the
        /// physical cell is ready to execute the next step.
        /// </summary>
        /// <param name="activity">The activity objects gives access to all relevant information</param>
        void Ready(IActivity activity);

        /// <summary>
        /// Send a message to the cell through the driver to simulate that the
        /// physical cell is ready to execute the next step.
        /// </summary>
        /// <param name="processId">The process id</param>
        void Ready(long processId);

        /// <summary>
        /// Send a result to the cell about the activity that just finished
        /// </summary>
        /// <param name="result">Simulated execution result</param>
        void Result(SimulationResult result);

        /// <summary>
        /// Event raised when the value of <see cref="SimulatedState"/> has changed
        /// </summary>
        public event EventHandler<SimulationState> SimulatedStateChanged;
    }

    /// <summary>
    /// Simulation result object
    /// </summary>
    public class SimulationResult
    {
        /// <summary>
        /// Numeric result of the activity, mappable to the result enum
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// Activity the result is for
        /// </summary>
        public IActivity Activity { get; set; }
    }
```

So here is an example of how your mock driver can implement `Moryx.Simulation.ISimulationDriver`:

````cs
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

        protected override async Task OnStartAsync(CancellationToken cancellationToken)
        {
            await base.OnStartAsync();

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
````

Note: The `Send()` method might differ from cell to cell.You can override the `Send()` and do your implementation of what needs to happend when your "mock" driver receives something from the cell. In case your driver doesn't have a `send()` method, you can use your own method and react upon the type of message you received just like described inside the `TestMockDriver.Send()` method above.

 # How does my Cell use my `MockDriver` ?

In your cell definition/Class you can use the following example based on the type of driver you're trying to simulate. Let's say we are trying to "mock" an `MQTT driver` since Moryx MQTT driver inherit from `IMessageDriver<object>` in your cell you can have the following case:
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
