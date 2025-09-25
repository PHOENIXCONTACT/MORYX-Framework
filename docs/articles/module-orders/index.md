---
uid: OrderManagement
---
# OrderManagement

The OrderManagement is a ServerModule and one of the top elements in the overall architecture.
It contains a bunch of components and plugins to manage the processing of Orders.

This includes the responsibility for ...

- providing orders including operations and their changes to the UI.
- accepting created orders and operations by the user.
- adding product and material related data to an Operation.
- choosing the recipe and adding order specific data for the production.
- validation of operations.
- loading documents of a product for an operation
- providing access to the JobManagement for creating, starting and completing Jobs for an operation.
- providing persisting abilities for operations.
- accepting orders from Hydra and reporting of progress updates to Hydra.
- accepting advices with a given loading equipment and an amount of articles
- managing the state of the machine

## Provided Facades

This modules exports the `IOrderManagement` facade.

## Referenced facades

 Plugin API | Start Dependency | Optional | Usage
-----------|------------------|----------|------
[IJobManagement](xref:Moryx.ControlSystem.Jobs.IJobManagement)|Yes|No|For each valid order one or more jobs will be created to execute it.
[IUserManagement](xref:Moryx.Users.IUserManagement)|Yes|No|For each order action a user is required
`IProductManagement`|Yes|No|The product management is used to validate the order and load the product.

## Used DataModels

- [Moryx.Orders.Model](xref:Moryx.Orders.Model)

## Structure

![Components](images\OrderManagementComponents.png)

The internal structure of the OrderManagement is shown in the component diagram. On the top is the OrderWcf which uses the OperationPool to get any update of each `OperationData` or perform some operations on each one. The `OperationPool` is an implementation of the Object Pooling Pattern which holds all known OperationData. The `OperationData` is the main component which handles all operation related functionalities. There is the `OperationAssignment` to assign all needed Information to a new or reloaded OperationData to make it producible. The `JobHandler` will be used to communicate with the `JobManagement` of the ProcessEngine to dispatch Jobs for the production.

Each of the components is described separately in the next sections.

## OperationDataPool

The [IOperationDataPool](xref:Moryx.Orders.Management.IOperationPool) is responsible for:

- Adding new operations to the pool and add them to a given order
  - If the order is not existent then a new one will be created
  - If the the order is already created then it will be loaded and the operation will be added
- Restore the operations from the database after a restart
  - Only not completed operations will be loaded after a restart to reduce the amount of operations to restore

## OperationPool

The [IOperationPool](xref:Moryx.Orders.IOperationPool) is used for all module plugin api implementations. It is also implemented by the same OperationPool but exports only the public API.

## OperationData

### State Machine

![OperationData State Machine](images\OperationsStates.png)

#### Initial

The state machine of the OperationData is designed to fit the needs of Hydra and provide some flexibility to the user. The first part of the StateMachine is the application specific part. The states `Initial`, `InitialAssign` and `InitialAssignFailed` is handled by the OperationAssignment which includes strategies to import an OperationData and make it `Ready` for production. This first part is classified as [Initial](xref:Moryx.Orders.OperationClassification).

#### Ready

The OperationData is in the ReadyState after the first assignment. So the OperationData has an assigned product and recipe. So it is ready for the production which can be started with the `Begin` transition.
At this point it is also possible to complete the OperationData if the OperationData should not be produced for some reason.

#### Running

A job will be dispatched by the OperationDispatcher with the given Amount from the BeginContext. This job can be started and produced. After finishing the job the OperationData switches to the AmountReached state were the worker has all possibilities to go further with the OperationData. It is possible to

- begin a new job and go back to the RunningState
- interrupt the OperationData and perform a partial report about the current SuccessCount and ScrapCount
- complete the OperationData and perform a final report about the current SuccessCount and ScrapCount
- just perform a partial report about the current SuccessCount and ScrapCount

The OperationData is still running in this state until the User decides to start the production again, interrupt or complete the OperationData.

#### Interrupted

The OperationData can be Interrupted if the it is in the RunningState or AmountReachedState (both of the states have the state classification [Running](xref:Moryx.Orders.OperationClassification)). In both case all jobs will be completed and the worker can add some report information which is a partial report. The OperationData switches from the RunningState to the InterruptingState first to wait until all jobs are completed. After all jobs are completed the state switches to the InterruptedState which leads to a Interrupted event with the user from the last partial report. In the AmountReachedState all jobs are already completed so there is no reason to wait for some jobs.

#### Completed

The `CompletedState` is the final state of the OperationData. The OperationData is finished at this state and can't be started anymore.

#### Aborted

An operation can be technically aborted. This is only possible if the operation is `Ready`.

#### AmountReached- and Interrupted assign

The operation can be re-assigned in stable states like `Ready`, `Interrupted` and `AmountReached`. The reassignment is only possible if the `RecipeAssignment` reports an update of the master data before. The reassignment only reassigns the recipe based on the change. If only the clone was changed, it will simply be replaced on the operation and will be used for the next job (currently running jobs will be completed). If the template of a recipe was changed, the complete recipe assignment will be executed again.

### Beginning

As mentioned in the ReadyState the production can be started with the Begin-Transition.
To begin an OperationData the following information are necessary:

- SuccessCount: Sum of all successful processes of the jobs of an OperationData
- FailureCount: Sum of all failed processes of the jobs of an OperationData
- ReworkedCount: Sum of all processes which are flagged as reworked of the jobs of an OperationData
- ScrapCount: FailureCount - ReworkedCount. If the ReworkedCount is bigger as the FailureCount then the ScrapCount is zero instead of a negative value.
- TotalAmount: The amount of the OperationData which should be reached.
- TargetAmount: The amount the worker decides to produce
- UserId: The id of the user which was selected to document who has begun the operation

The OperationData provides a [BeginContext](xref:Moryx.Orders.Management.BeginContext) which includes the following data:

- PartialAmount: The amount of the already produced articles.
  - ReplaceScrap = false: SuccessCount + ScrapCount. The produced articles are including the ScrapCount.
  - ReplaceScrap = true: TargetAmount. The goal is to reach the TargetAmount.
- ResidualAmount: Amount to reach the TargetAmount (TargetAmount - PartialAmount).

If the TargetAmount is smaller as the TotalAmount then the difference will also be add to the ResidualAmount.
Now the BeginContext includes all production necessary information which will be used to dispatch a new job.

### Reporting

As mentioned in the AmountReachedState the OperationData can be stay in the this state or can switch to the Interrupted or Completed state.
In all cases a reporting will be performed. The reporting information are encapsulated in the [OperationReport](xref:Moryx.Orders.OperationReport) which includes the following information:

- Id: A report will be stored in the Database to have a reporting history of an OperationData
- [ConfirmationType](xref:Moryx.Orders.ConfirmationType): Type of the report to decide if it is a partial (normal report or interrupt) or final (complete) report.
- SuccessCount and ScrapCount: Success and Scrap articles which should be reported.
- Comment: Optional information
- UserId: The id of the user which was selected to document who has done the report

Each report is depending to the machine state and only possible if the state has the classification `Production`. It is possible that there is are no states or no production state. In this case the reports are independent to the machine state. A state change to the production state will lead to a state change task followed by a report task if there is something to report. If the state switches from the production state to a non production state then this will lead to a report task followed by a state change task to ensure that everything will be reported in the production state. 

### Advice PickParts/Order

An advice is possible after the operation is Ready and until it is Completed. All information are encapsulated in [OperationAdvice](xref:Moryx.Orders.OperationAdvice) which exists in two variants. The first is the `OrderAdvice` which is used to advice *produced parts*. The second is the `PickPartAdvice` which is used to advice *pick parts*. Every advice type needs a tote box number.

An advice will raise an event to inform other components about the advice with the containing information.
There is an [AdviceContext](xref:Moryx.Orders.Management.AdviceContext) available which will be requested for a new advice to provide the already advised amount for `OrderAdvice`s.

### Behavior

There are some behavior implemented to handle counting of parts, interruption, restoring or some events which should be invoked in some case.

#### Counting Behaviors

The implementation of part handling is not complex but it took much time to find a behavior which should support the worker in a best way.
It is implemented as a strategy and can be configured in the module. The order management brings two default strategies.

The first strategy `ReplaceScrap` just reaches the `TargetAmount` with the `SuccessCount` => "Produce articles only with the given material".
With the second strategy `DoNotReplaceScrap` we have to consider that the worker will rework some articles and how to handle it => "Produce the target amount and possibly with more material".

Choosing one of these behaviors will effect the way how the target amount to produce articles is calculated. If there is a need for a different behavior it can be added very simply.

To add a new strategy it is necessary to implement the interface [ICountStrategy](xref:Moryx.Orders.Management.ICountStrategy). It is used as a module strategy and should be `Singleton`.

**With replacing scrap**
The following table shows some data we are using for unit test to ensure a right behavior.
There are basically three cases but in all cases the goal is to reach the `TargetAmount` (plus the difference to the `TotalAmount`) with the `SuccessCount`.

| Total | Target | Success | Failure | Rework | Scrap | Partial | Residual | Description |
|-------|--------|---------|---------|--------|-------|---------|----------|-------------|
| 10    | 9      | 9       | 2       | 5      | 0     | 9       | 1        | There are more rework than failure. |
| 10    | 8      | 8       | 2       | 2      | 0     | 8       | 2        | Rework is equal to failure |
| 10    | 8      | 8       | 2       | 0      | 2     | 8       | 2        | Rework is less than failure |

**Without replace scrap**
Not replace scrap is a bit complex because the worker can replace scrap if he wants to. So it should be handled.
Basically the goal is to reach the `TargetAmount` (plus the difference to the TotalAmount) with the `SuccessCount + ScrapCount`.
The worker starts the production with the new `BeginContext` and defines a `TargetAmount`.

`Without replace scrap` means that the `TargetAmount` is a kind of possible tries. Each try can be successful or not so the `SuccessCount` or the `FailureCount` will be increased. Every try represent an successful or failed produced article.

Case three of the table is the usual case for not replacing the scrap. The `TargetAmount` was reached with `SuccessCount + ScrapCount` and the `ResidualAmount` is 2 to reach the `TotalAmount`.

Case two describes the case that the user has reworked failed parts. So he spent two tries for the rework. This means that he not produced the required `TargetAmount`.
That is the reason why the `ReworkedCount` will be subtracted from the `TargetAmount` to get a right `ResidualAmount`.

Case four is similar to case two but the reworking fails. Nevertheless there are also two tries spent for reworking which will be subtracted from the `TargetAmount` to get two extra tries for the next production.

Case one is the case that a worker reworks parts from another operation. So there are more reworked parts as failed.
For example there are two failure parts and 5 reworked. So the user spends only two tries for the reworking. The order reworked parts are just successful produced parts.

| Total | Target | Success | Failure | Rework | Scrap | Partial | Residual | Description |
|-------|--------|---------|---------|--------|-------|---------|----------|-------------|
| 10    | 9      | 7       | 2       | 5      | 0     | 9       | 1        | There are more rework than failure. |
| 10    | 8      | 6       | 2       | 2      | 0     | 8       | 2        | Rework is equal to failure |
| 10    | 8      | 6       | 2       | 0      | 2     | 8       | 2        | Rework is less than failure |
| 10    | 8      | 0       | 4       | 2      | 2     | 8       | 2        | Rework failed |

#### Interrupt Behavior

An interrupt can be performed in the `AmountReachedState` and in the `RunningState`. The main difference is that in the `RunningState` there are running jobs which are still producing.
So if the worker decides to interrupt the OperationData during the `RunningState` then he only gets the current `SuccessCount` and `ScrapCount`. The parts which are not done are not taken into account for the possible reporting and will be finished afterwards and can be reported later.
An interrupt is predefined with the `ConfirmationType` [Partial](@ref Moryx.Orders.ConfirmationType). So only a partial report is possible.

#### Restore Behavior

The OrderManagement should be restartable without restarting the rest of the ControlSystem. So it is necessary to restore each OperationData by its jobs and decide in which state the OperationData is.
Therefore there `OperationAssignment` has a `Restore` method to add the Product and Recipe again. The OperationData will be loaded with the last saved state and each state gets a restore method so each state can decide what is to do for the restore and may switch to another state.

#### Events

The OperationPool is the central point to get information about the OperationData. It is possible to extend the OrderManagement with some plugins which also can be pool user like the HydraCommunication plugin. So it should be possible to get informed about a `Begin`, `Report`, `Interrupt`, `Complete` and an `Advice` of an OperationData.
So each OperationData provides this events and the OperationPool adds himself as a listener to the events of each OperationData. The OperationPool provides the same events and invokes an event it the same event occurs from an OperationData. So a PoolUser can register to the OperationPool events which will be invoked if an OperationData event occurs.

The following events are provided:

- Updated
- Started
- Interrupting
- Interrupted
- Completed
- Aborted
- PartialReport
- Advised

## Operation Manager

Handles actions on operations which have more dependencies than the operation itself. It is the third pillar between the pool and other components. The operation manager uses the rule manager to get the rules for the report and beginning an operation. This will ensure that all corresponding rules will be complied for a report or begin of an operation.

## Advice Manager

The [IAdviceManager](xref:Moryx.Orders.Management.IAdviceManager) is responsible for handling advices done by the user. It executes the advice requests on a strategy of [IAdviceExecutor](xref:Moryx.Orders.Management.IAdviceExecutor). The default is the `NullAdviceExecutor` which is rejecting all advice requests.

## Assignment

The `OperationAssignment` encapsulates several steps to assign needed data to an OperationData. This parts are application specific so they are exchangeable with the configuration of the main module.

### Product Assignment

The `ProductAssignStep` uses the [IProductAssignment](xref:Moryx.Orders.Management.IProductAssignment) which is responsible for requesting the needed product from the product management. A new OperationData contains a product reference to the `Product` property. The reference will be used to restore or to assign the real product.

To implement a custom product assignment the custom implementation should derive from [ProductAssignmentBase](xref:Moryx.Orders.Management.ProductAssignmentBase`1).

### Recipe Assignment

After the product assignment, the recipe will assigned inside of the `RecipeAssignStep` by using the [IRecipeAssignment](xref:Moryx.Orders.Management.IRecipeAssignment) plugin. The default implementation selects the current recipe of the product. After creation, the recipe will be used for the job creation. After selecting the source recipe, the recipe will be processed. The default implementation don't do any processing but in some applications it is necessary to add some operation specific information like the operation number or some material parameters.

To implement a custom recipe assignment the custom implementation should derive from [RecipeAssignmentBase](xref:Moryx.Orders.Management.RecipeAssignmentBase`1).

### Documents

The `DocumentAssignStep` loads the available documents of a product for the new operation. The loading will be handled during the assignment to ensure that a ready operation has all available documents. The `DocumentProvider` uses a [IDocumentLoader](@ref Moryx.Orders.Management.IDocumentLoader) to load the documents from a specific source which can be configured. The provided documents from the `DocumentLoader` will be used to store the files at the file system. Each document will be represented with the downloaded file and a json file which contains information like the document number or revision to restore the document information. Other components have the possibility to change the source from a document. So the client used the source to provide a filtering by source.

### Material Parameters

The creation context of the operation and order can contain material parameters from the source which creates the creation context. There is a list of [IMaterialParameter](xref:Moryx.Orders.IMaterialParameter) which must be casted to the needed material parameter type. The types are application specific which can depend a MES or other systems. The material parameters will typically used in the product or recipe assignment.

### Validation

The `ValidateAssignStep` is responsible for validating the operation by using the [IOperationValidation](xref:Moryx.Orders.Management.IOperationValidation) plugin. It will be done after the product assign step and after the recipe assign step so that all production relevant data for the validation are present.

To implement a custom validation the custom implementation has to be derived from [IOperationValidation](xref:Moryx.Orders.Management.IOperationValidation).

#### Regex Validation

The implementation `RegexOperationValidation` can be used to:

- Check the operation number with an configurable regex
- Check if the Amount is greater than 0
- Check if there is a recipe assigned

## JobHandler

The [IJobHandler](xref:Moryx.Orders.Management.IJobHandler) is responsible for:

- [Dispatching](xref:Moryx.ControlSystem.Jobs.IJobManagement) jobs to an operation for the production
- [Starting](xref:Moryx.ControlSystem.Jobs.IJobManagement) jobs of an  operations to start the production
- [Completing](xref:Moryx.ControlSystem.Jobs.IJobManagement) jobs of operations if the operation should be interrupted
- [Restoring](xref:Moryx.ControlSystem.Jobs.IJobManagement) jobs of operations when the operation is reloaded

The dispatcher inside of the job handler is implemented as a strategy and can be configured via the Command Center. Implementing new behaviors is also very simple. You just need to implement OperationDispatcherBase.

The job handler uses the [IJobManagement](xref:Moryx.ControlSystem.Jobs.IJobManagement)-Facade to dispatch new jobs. Every job update will be given to the responsible operation. If no operation was found (can happen for test jobs) the update will be ignored. While dispatching a job, the new job will be moved directly under the last job of the operation.

The dispatcher of the job handler is also responsible for completing jobs which are related to an operation. This is needed while manually interrupting an operation.

If the OrderManagement will be restarted, it is necessary to reload the jobs which are referenced to an operation. The job handler will be used to load these job references by a given array of job-ids.

## User Interfaces

### Order UI

The ui of the order management will be used to show all orders to produce on the current machine. It is also possible to create order without an external system like HYDRA or SAP.

**Concept**
The orders ui contains a list of orders for a first overview. The details of each order can be displayed by selecting it. 
The ui has an area for all order details. This contains general information like order number and the due date, information about the product to produce and a list of operations of the selected order.

**Hide completed**
If the checkbox *Hide Completed* is checked, all completed operations are hidden from the list.

**Create an Order**
The order ui provides an order creator which can be access over the *Create Order* button.
The order creator has some text fields for the information which are necessary to create an order.
The *Create* button will activated if all necessary information are provided by the user.
Currently all text fields must be filled and the list of operations must have at least one operation entry.

### Job UI

The job UI mainly will be used to have an overview of all current jobs, there current production state, the sort order and to create some test jobs to test e.g. new workplans or resources.
The job UI provides also the possibility to start, abort and reorder each job.

**Concept**
The job UI is basically a list of all current jobs. There are some buttons on the bottom side to start, abort and moving each job.
To start, abort or moving a job it is necessary to select the job and click on the button you want. 
The buttons will be automatically disabled and enabled depending if the action is executable with the selected job.


