---
uid: ProcessEngine.JobManagement
---
# JobManagement

The job management is a composition of components. The components are grouped together with the name _JobManagement_.
The JobManagement is a top level infrastructure of the ProcessEngine. As its name anticipates, its main task is the management of jobs. Usually for each order one or more production jobs will be created. If the order refers to more than one product, for each product at least one separate job is needed, because the job references the recipe needed for the production. Other types of jobs are setup and maintenance jobs. A setup job is used to change the configuration of the facility for a product to be produced.

The main task of the job management is to create one or more processes for each job to be executed by the [Process Controller](xref:ProcessEngine.ProcessController).

## Provided Facades

The JobManagement's public API is defined by the facade [IJobManagement](xref:Moryx.ControlSystem.Jobs.IJobManagement). The facade will forward requests from outside to internal components. A description of its main functionality can be found within the interface documentation.

## Components

As described, the job management is a composition of components. The internal architecture is represented by the following components.
Each component's implementation may either be part of the server module or is loaded on the server module's start up to allow customization of behavior. In the following figure, the components are shown:

![Job Components](images/job-components.png)

| Component name | Implementation | Description |
|---------------|----------------|--------------|
| JobManager | Internal | Central component managing the job behaviors |
| JobList | Internal | The component which holds the jobs of the system. Manages adding, removing and sorting. |
| JobHistory | Internal | The component which can load completed jobs from the database. |
| JobScheduler | Internal or External | Component to start jobs for the available slots.  |
| JobDispatcher | Internal | Component to encapsulate the process handling with the process controller for each job |
| JobsWcfServiceManager | Internal | Manages the WCF clients |

## JobManager

The JobManager is the central component of the JobManagement. It is responsible for all behaviors while handling jobs. This component does not differentiate between jobs for production, setup of other types.

**JobHandler:** To seperate the different behaviors for jobs into different components there is a `IJobHandler` interface for components that are involved in job creation and handling.

**Adding production jobs:** If a external component (e.g. [OrderManagement](xref:OrderManagement)) adds a production job it is only possible to do it with a structure called [JobCreationContext](xref:Moryx.ControlSystem.Jobs.JobCreationContext) which is responsible to give a fix structure how production jobs should be added. This context holds up to multiple templates for creating it later. Each template consists of a `IProductRecipe` and a `amount` to produce. It is also possible to reference a target position in the list of jobs. The target can only be another production job. Possible positions are documented in the related enum [JobPositionType](xref:Moryx.ControlSystem.Jobs.JobPositionType).

**Handling of possible next job:** The JobManager is listening to `SlotAvailable` event of the `JobScheduler`. Whenever the `JobScheduler` decides to have a slot for a job available, the JobManager tries to find the next possible production job from the `JobList`. If there is one or more possible next found, it will be passed through all `IJobHandler` components to enrich this list of schedulable jobs with more information. An example for a `JobHandler` is the [SetupManagement](xref:ProcessEngine.SetupManagement) which adds jobs around production jobs for setting up the facility.

**Reload after restart:** If the ProcessEngine will be restarted, the JobManager also loads uncompleted jobs from the datbase. It may happen that a job will be completed after loading it from the database. Therefore loaded jobs will be instantly saved again to filter the completed one's.

**Storage:** The `JobList` publishes an event that for a job is a storage required. This happens whenever a job changed their state, will be resorted in the list or was completed. The manager is responsible to store these changes.

**Destruction:** If a job was completed, the `JobList` also publishes an event, that the `Destruction` of the related `JobData` object is necessary. The manager will destroy the object for the system.

## JobList

The [JobList](xref:Moryx.ControlSystem.ProcessEngine.Jobs.IJobList) is the basic element to handle all jobs and it is used to get, update and delete jobs during the production.

**IEnumerable:** The component implements the `IEnumerable<T>` interface and can be handled as normal `Collection`. Internally it guarantees a safe locking structure that no raise conditions can occure. With this interface it is possible to use the default .NET extensions like `FirstOrDefault`, `Where` or `Last`.

**Forward and Backward:** The list provides more APIs to iterate the list from a defined starting point in forward or backward direction.

**Moving:** The list also provides possibilities to move jobs before or after another existing job. It is possible to move one job to a specific position in the list.

**Events:** Update information will be forwarded to every connected event listener for `StateChanged`, `Completed`, `ProgressChanged` or `SortOrderChanged`.

**ThreadSave:** All methods are are thread safe. Every change will be locked until it is finished. So every connected user can work on the job list safely.

## JobHistory

Jobs which are completed are removed automatically by the job list because it is not needed in the system anymore.
The `JobHistory` is the component which can load old jobs.
Some jobs are not loadable by the `JobList` because it only includes the active jobs and does not have any of the old jobs.
The `JobHistory` has a simple API to just load one job.

## JobScheduler

The [JobScheduler](xref:Moryx.ControlSystem.ProcessEngine.Jobs.IJobScheduler) provides an API to to check if the machine has a slot for a job or informs the JobManager if a slot is available. The scheduler is an exchangeable part of the ControlSystem and can be implemented application specific. It also includes job state change handlers called by the job manager to avoid race conditions from listening to job changes directly on the job list

The idea of this component is that different machines / customers may need a different behavior how jobs (and how many jobs) will be started or stopped for production.

**_Important facts_**

1. The scheduler is the only component which starts jobs
2. Will be used to start jobs
3. If there is no slot then no job can be started even if the operation was started

**Scheduler Base**
The base class defines the job list import and specialized config cast. Apart from that all methods are defined abstract and must be implemented in the derived scheduler

**SchedulableJobs** Every Scheduler must implement the method `IEnumerable<IProductionJobData> SchedulableJobs(IEnumerable<IProductionJobData> jobs, JobPosition<IJobData> position)` which should return those jobs that could be started from the given list of possible jobs.

**JobSlots**
The [JobSlots](xref:Moryx.ControlSystem.ProcessEngine.Jobs.JobSlots) class is a helper class to handle the slot size for a machine. Therefore it needs an count for possible parallel runable jobs. It is also possible to define more then one JobSlot object in a Scheduler for example for different production jobs. May a machine can run four production jobs but only job for the mounting at the first resource. The helper class provides some helpfull properties like HasSlots or AvailableSlots or methods like TryResize or TryAssign for the slot handling.
So the base slot handling is encapsulated in this class and it is possible to concentrate only on implementing a scheduler behavior which you need for an application.

**Simple Scheduler**
It is derived from the [JobSchedulerBase](xref:Moryx.ControlSystem.ProcessEngine.Jobs.JobSchedulerBase`1).
The `SimpleScheduler` starts jobs depending on the count of the available slots.
The slot count is configurable and will be set with the [MaxActiveJobs](xref:Moryx.ControlSystem.ProcessEngine.Jobs.SimpleSchedulerConfig) property.
The jobs will be scheduled by the position in the JobList. The first one will be scheduled first and so on.

## JobDispatcher

The `IJobDispatcher` helps jobs to handle actions after the start by the JobScheduler. The dispatcher will be used by the job to interrupt, resume, compelte, cleanup and abort the production. It is the chain to the process controller. The job dispatcher has also to listen on the process state changes from the activity pool and raises the changes to the referenced job. The referenced job decides to dispatch a new process or not or handle the information for example to switch to different state.

It also provides a cleanup method to reunite jobs with their running processes before trying to cleanup these processes. This is triggered by the _jobData_ object.

## WcfConnector

The `IJobsWcfConnector` is the standard implementation of a WCF connector for NetTCP based WCF services. `IJobsWcfServiceManager` manages the client connections. It listens on JobList events, forwards changes to the clients, and forwards requests from the clients commands to the job management.

`IJobsWcfService` is the definition of the WCF service itself. The implementation forwards all requests to the `ServiceManager` and all data changes to the callback. `IJobsWcfCallback` is the definition of the callback interface of the clients.

## JobData

The component *JobData* is the base class of all job types. It implements a abstract base state machine for the basic job transitions. It also implements the generic [IJobData](xref:Moryx.ControlSystem.ProcessEngine.Jobs.IJobData)

The _JobData_ is the internal business object which represents a job in the Process Engine. The only dependency is the `IJobDispatcher` to handle the communication with the process controller. The internal state machine handles all the logic for dispatching processes, starting, aborting, interruption, cleanup and so on. The state machine is also implemented abstract and will be extended within the different job types.

There are currently two implementations of jobs `ProductionJobData` and `SetupJobData`.

**State Machine**
Each job is following a clean defined state machine. The state machine is quite complex regarding the flexibility of the process controller.

**Classification**
The classification helps to understand the whole state machine. It will be also used by depending modules to verify a job by its classification instead of each state.

| Classification | Description |
|----------------|-------------|
| Idle | Job is currently awaiting a slot by the scheduler |
| Waiting | Job has received a slot and is now waiting to be started by the scheduler |
| Running | Running job which currently executes processes |
| Completing | The job will be completed. No new processes will be started. |
| Completed | The job is completed and cannot be started anymore. No new processes will be started. |

**Display Name**
There exists merged synonyms which represent better understandable names for the internal states which will be used for example in the UI.

## ProductionJobData

A production job is usually the most common type. It is used to produce products according to the product description provided by the production recipe.

**Amount**
The amount of a job is the number of processes which should be executed. In a production job, the number represents the yield amount.

**Endless**
A job can be endless or can be defined with a specific amount. An endless job have usually an amount of 0.
When a job has no amount then it will dispatch processes endless until the job is completed manually.

**ProcessCount**
The ProcessCount is the summary of the running, success and failed processes.

**SuccessCount**
The SuccessCount is an amount of processes which are successful.

**FailureCount**
The FailureCount is an amount of processes which are failed.

**ReworkedCount**
The ReworkedCount is an amount of reworked processes. A reworked process was a failed process in the past and was reloaded to try the production again.
The reworked flag is independent from the state but will only be count if the state switches to success or failure.

### ProductionJob States

![ProductionJob StateMachine](images/productionjob-statemachine.png)

**_Important facts_**

1. There are four states which are classified with _Initial_. These are the only one which should be present after a restart of the ControlSystem.
2. There are also four corresponding states which are classified with _Waiting_ which also can handle a restart because no process was dipatched or resumed at this point
3. If a job was reloaded with a state which is classified as _Running_ or _Completing_ then something went wrong during the shut down. These jobs cannot handle a restart and will be removed after a clean up. So the current production of the affected jobs will be aborted and all articles will be removed from the WPC´s with unmount activities.
4. There are four states which are classifies with _Completing_ this means that the job will finish its production soon because only the started processes will be finished and no new processes will be dispatched

**Initial**
This state is the first one which a job can achieve. When the job is created, then it is in the initial state.
It is waiting for a _Ready_ to change to the next State "Waiting" or a _Complete_/_Abort_ to change to the next state "Completed".
An _Interrupt

**Waiting**
Jobs in the waiting state are waiting for a _Start_ to switch to the "Dispatched" state to dispatch processes.
A waiting job can also be directly switch to "Completed" with _Complete_ or an _Abort_ call without dispatching any processes.
So the production was not started.
It can also go back to the "Initial" state when an _Interrupt_ occurs, or when a _Load_ is executed after a ControlSystem restart.

**Dispatched**
In this state, a first process was dispatched by the job. The state is listening on process changed events.
If the process changed to running (activity was dispatched) the amount of the job will be checked. If the amount is reached, the job will be switch to the "Completing" state to finish the last process.
If the amount is not reached, the state will be swicth to "Running" and a new process will be dispatched (a process for the queue to have a better performance).

**Running**
In most case the job will stay in this state until the amount was reached and then will switch to "Completing" to finish the last process.
This state is also listening to process state changes.

- If a process is completed, the JobData will reduce the count of running processes.
  - If the state is _Success_ then the SuccessCount will be increased
  - If the state is _Failure_ then the FailureCount will be increased.
- If process is switched to _Running_ and the amount of the job was not reached a new process will be dispatched.
- If process is switched to _Running_ and the amount of the job was reached then the state will change to completing.

**Completing**
This state is waiting for the processes to be completed or interrupted and is listening to process state changes.

- If a process is completed, the JobData will reduce the count of running processes.
  - If the state is _Success_ then the SuccessCount will be increased.
  - If the state is _Failure_ then the FailureCount will be increased.
- If all running processes of the job are finished, the state will switch to "Completed".
- If the process is interrupted, the JobData will reduce the count of running processes.

If the restart should be restarted and an _Interrupt_ call occurs then the job switches to the state "CompletingInterrupting** to interrupt the current process to have a resumable state after the restart.

**CompletingInterrupting**
The job switches in the "CompletingInterrupting" state if the ControlSystem should be restarted and the job is completing its work.
If the processes are interrupted then the job switches to the "CompletingInterrupted" state.

**CompletingInterrupted**
The "CompletingInterrupted" state is classified as initial which waits for a _Ready_ call to switch to the "CompletingInterruptedWaiting" state to wait for resuming the interrupted process. A job in this state can also be directly completed and switches to the "Completed" state if the process was completed.

**CompletingInterruptedWaiting**
A job in the "CompletingInterruptedWaiting" state waits for a _Start_ call to resume the process and switches to the "Completing" state to complete it.

**Completed**
This is the final state of a job. The job is now completed and can not be started again. There exist no unfinished processes for this job and it will be not loaded again.

**Suspended**
The job reaches the "Suspended" state if a _Stop_ was called at a "Running" job.
All running processes will be finished but no new processes will be dispatched.
If the last process switch the its runnint state then the state of the job switches to "Completing" to complete the suspended job.

**Interrupting**
The job will switch to the "Interrupting" state if the production was started (processes were dispatched) and _Interrupt_ was called.
This will only be called if the ControlSystem should be restarted.
This call will lead to an interrupt of all running processes. Each process can finish its current activity and no new activity will be dispatched.
After all processes was interrupted then the job switches to the "Interrupted" state and the ControlSystem can should down.

**Interrupted**
This state indicates that the job is now ready for a restart of the ControlSystem.
The jobs can be started again after the restart.

**InterruptedWaiting**
After a restart the successfull interrupted job will be loaded in the "Interrupted" state and waiting for the _Ready_ call.
The interrupted job will switch to the "InterruptedWaiting" state after the _Ready_ call and waits for a start to switch to the "Runnging" state to resume the production.
If a _Complete_ call occurrs then the job switches to the "CompletingInterruptedWaiting" state to wait for a _Start_ call to resume the last process without dispatching a new one.

**CleanUp**
After the ControlSystem restart all jobs in the JobList will get a _Load_ call to dispatch new process.
Those jobs that cannot be continued cannot handle the _Load_ call and will go into the "CleanUp" state.
If this happens then something went wrong during the restart. So the not continuable jobs must be removed.
The job switches to the state "CleanUpWaiting" if the job is ready for a clean up.

**CleanUpWaiting**
A job in the "CleanUpWaitingState" waits for a _Start_ call to start the clean up and switches to the "CleaningUp" state.
It is also possible that an other restart occures to the job can switch back to the "CleanUp" state and wait for a clean up after the restart.

**CleaningUp**
A job switches to the "CleaningUp" state if the clean up must be started.
This meand the no new processes will started and for running processes only unmount activities will be dispatched to remove articles from all depending WPC´s to have a clear production state at the machine.
If everything was cleaned up then the state switches to "Completed".

**Aborting**
This state can be reached from the states which handles processes to abort the current production.
A job in this state is cannot be reloaded so a restart will lead to a clean up. An _Abort_ or _Complete_ call will not affect this state. It will abort the process.

## SetupJobData

A setup job will be created by the [SetupManagement](xref:ProcessEngine.SetupManagement) to change the setup of the facility. In comparison to the production job, the setup job only dispatches a single process with generated setup activitities. Further reading can be found within the [SetupManagement](xref:ProcessEngine.SetupManagement). The state machine is explained in the chapters below. The setup job provides the following properties to show to progress:

**RunningCount**
As described, the setup job creates just a single process to setup the facility. This values describes the currenlty running setup activities for this job.

**CompletedCount**
This values describes the completed setup activities of this job.

### SetupJob States

![SetupJob StateMachine](images/setupjob-statemachine.png)

**Initial**
This state is the first one which the job can achieve. When the job is created, then it is in the initial state. It is waiting for a _Ready_ to change to the next State "Waiting" or a _Complete_/_Abort_ to change to the next state "Completed". An _Interrupt_ will also cause completion.

**Waiting**
The job was handled by the *JobScheduler* and is ready for executing the setup activities. If the job will be started then the single process will be dispatched and the job switches to the next state *Running*.

**Running**
With this state, the job have a running process dispatched to the ProcessController. If the process will be finished it will descided how the job will end.

- ProcessState Success
  The process and all setup activities were success. The setup was completed and then also the job can be completed.
- ProcessState Failue
  If the process was failed, an activity was also failed. The job will move to the *RetrySetup* state where it will be started again by the SetupManagement. The setup job will only contain the remaining setup activites.

**RequestRecipe**
In this state a setup job requests a new setup recipe. This can either be, because the previous setup failed or because a clean-up job is requesting an updated recipe for the current maschine state. The setup management will then update the recipe and jobs returns to the running state. If a setup fails more than the retry limit of *3*, it will go into the *RetrySetupBlocked*.

**RetrySetupBlocked**
In this state it was tried to run the setup several times without success. SO a notification will be presented to the user, prompting for action. When the user acknowledges the notification the setup will transit to the state *running* again.

**Interrupting**
If a setup job will be interrupted, the running process will also be interrupted. If this will be successfull, the job will be *Completed*. It can happen that a setup process will not be interrupted because of many reasons. If this happens, the setup job will be cleaned after restart of the control system and is not available. In this case a new one will be created.

**Aborting**
If a setup is aborted and was already started, it enters the AbortingState. Processes are then aborted and the job completed.

**Completed**
Completed jobs should directly removed from the *JobList* if they are completed.

## Sequence of adding a ProductionJob

In following there is a description about the sequence of adding a job beginning from the `JobManagementFacade`. The picture shows whole sequence including the user and the `OrderManagement`.

**JobManagementFacade**
An `Add` call with a `JobCreationContext` will lead to a validation of all containing recipes which will be used for the job creation.
A valid recipe must not be null and should have an origin where this recipe was created. Then the `Add` method of the `JobManager` will be called with the `JobCreationContext`.

The facade will get a list of created `JobData` which will be filtered by `ProductionJobs` and used as the return value for the caller like the `OrderManagement`.

**JobManager**
The JobManager takes the `JobCreationContext` and creates `ProductionJobs` for each recipe and its amount. If the `JobCreationContext` has a defined Position then the `JobManager` tries to get the `JobData` which is will be used for the positioning inside of the `JobList`. If there is no Position then the new jobs will be just add at the end of the list.

Before the adding each job will be checked with the `JobScheduler` if there is a free slot for producing this job. If there is a free slot then this job is part of the schedulable jobs. Each schedulable job will send to every so called `JobHandler` to have the possibility to adjust the jobs which will be added. After the handling by the `JobHandler` the schedulable jobs will be add to the `JobList`.

**JobHandler**
Each job handler can adjust the list of schedulable jobs **BEFORE** they will be added to the joblist and be ready for the production.
For example the `SetupManagement` can add setup jobs if necessary. After every `JobHandler` is done with its work the jobs will be add to the `JobList`. After that each event will be active like `Updated`, `ProgressChanged` or `Removed`.

![Adding a ProductionJob Sequence](images/AddingProductionJob.png)