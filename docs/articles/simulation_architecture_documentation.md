# Architecture

The following image shows the architecture of the simulation.

![alt text](../articles/simulated_driver_architecture.jpg)

The architecture is divided in 3 parts : MORYX Cells ,MORYX Drivers and PLCs.

In the architecture we have : Cells that are digital twins of a physical cell; Drivers that the digital twin (cell) uses to communicate with the physical device (PLC).

The architecture shows the following interaction for a real production case:
1. The Process Engine receives a ReadyToWork from `Cell-1` and `Cell-2`, which was triggered by `Driver-1` and `Driver-2`.
2. The Process Engine sends/calls a StartActivity to `Cell-1` and `Cell-2`.
3. `Cell-1` uses the `Driver-1`'s `Send` method to send the message to the PLC, likewise `Cell-2` uses `Driver-2`.
4. The `Driver-1` uses communication protocols like `Mqtt`,`OPCUA` etc.. to send data to the physical device (PLC).
5. The physical device (PLC) send's back data to the driver using the same communication protocols like `Mqtt`,`OPCUA` etc.. .
6. The `Driver-1` notifies the digital twin `Cell-1` using `Driver.Received` event, that a message has arrived from the physical device (PLC).
7. When the digital twin `Cell-1` receives the message,it sends an `ActivityCompleted` to the Process Engine.
8. The Process Engine checks the next activity, and sends back a `SequenceCompleted` to the `Cell-1`.

The architecture shows the following interaction for a simulated production case:
1. The Process Engine receives a ReadyToWork from `Cell-1` and `Cell-2`, which was triggered by `Simulated Driver-1` and `Simulated Driver-2`'s `Ready()` methods.
2. The Process Engine sends/calls a StartActivity to `Cell-1` and `Cell-2`.
3. `Cell-1` uses the `Simulated Driver-1`'s `Send` method to send the message to the Simulator. 
4. The Simulator uses the ExecutionTime to delay and simulate an execution; Afterward calls/sends `Driver.Result()` and passes the activity result.
6. The `Simulated Driver-1` notifies the digital twin `Cell-1` using `Driver.Received` event, that a message has arrived from the simulator.
7. When the digital twin `Cell-1` receives the message, it sends an `ActivityCompleted` to the Process Engine.
8. The Process Engine checks the next activity and sends back a `SequenceCompleted` to the `Cell-1`, which in turn sends that message to the `Simulated Driver-1`.
9. The `Simulated Driver-1` sends back a `ReadyToWork` to the cell with a processId.

