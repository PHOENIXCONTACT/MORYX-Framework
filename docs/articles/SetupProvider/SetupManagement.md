---
uid: SetupProvider.SetupManagement
---
# SetupManagement

The SetupManagement generates the jobs that are required for the setup of a production system. A distinction can be made between preparatory setup jobs and follow-up cleanup jobs. Running a setup / cleanup job sends setup activities to the resources of the system. These activities inform the resources that a setup is required for the next job and provides the required information.

Both types, before and after the production job, are evaluated as late as possible. They are created when a production job is flagged ready by the scheduler and the prepare job is started. The clean-up evaluation is only a place-holder and reevaluated when clean-up is started by the scheduler, to achieve the most accurate behavior.

## Sequence Description

[SetupTrigger](xref:Moryx.ControlSystem.Setups.ISetupTrigger) inform the SetupManagement that the system needs a setup.  `SetupTrigger` are plugins which implement the `ISetupTrigger` interface and can be customized for the needs of the different resources. When starting a new job (for example, form the OrderUI), the [SetupManager](xref:Moryx.ControlSystem.ProcessEngine.Setup.SetupManager) calls the `Evaluate` method of all registered SetupTriggers. Unless this method returns `SetupEvaluation.Empty`, the corresponding `CreateStep`-method of the SetupTrigger is executed, which returns a [IWorkplanStep](xref:Moryx.Workflows.IWorkplanStep). Now the [SetupManager](xref:Moryx.ControlSystem.ProcessEngine.Setup.SetupManager) uses the returned WorkplanSteps to put it together to create [Workplans](xref:Moryx.Workflows.IWorkplan). The order of steps is determined by the triggers sort order. If two triggers have the same sort order, their steps are executed in parallel by using split and join.

## SetupTrigger Interface

A SetupTrigger is used to determine if a changeover is required before or after a jobs by its recipe and creates the necessary `WorkplanStep`.

By implementing the `ISetupTrigger` interface or deriving the [SetupTriggerBase](xref:Moryx.ControlSystem.ProcessEngine.Setup.SetupTriggerBase`1) class, specific setup triggers can be created for different resources of a production system.

The interface provides one property and two methods:

```cs
SetupExecution Execution { get; }
```

Determines whether the trigger is relevant **before** or **after** a production job.

```cs
SetupEvaluation Evaluate(IProductRecipe recipe);
```

[SetupEvaluation] (xref:Moryx.ControlSystem.Setup.SetupEvaluation) represents the result of the evaluation by the trigger. It is possible to assign a classification like `Manual` or `MaterialChange` or combine different classifications. Dervived types represent capability changes or reservations. Those evaluations also include capabilities which are used to determine setup necessity as well as machine change projections.

For example if you want to indicate a manual material change with modified capabilities, it would look like the example below
```cs
public override SetupEvaluation Evaluate(IProductRecipe recipe)
{
    return SetupEvaluation.Provide(new InsertHousingCapabilities{ Identifier = recipe.Product.Housing.Identitiy}, SetupClassification.Manual | SetupClassification.MaterialChange);
}
```

Because of defined implicit casts it is also possible to directly return `bool` or `SetupClassification` instead of the evaluation object.

```cs
IWorkplanStep CreateStep(IProductRecipe recipe);
```

Provided that `Evaluate` indicates the need for a machine change, this method creates a `WorkplanStep` to alter a target resource in a way that it provides the necessary capabilities afterwards.

### Example of Setup Trigger using the WatchProduct example

#### Preconditions

- There is a Machine which is able to produce digital watches as well as mechanical watches
- There is a cell which is responsible for the mounting of watchfaces on the watch.
- This cell is called WatchFaceMountingCell.
- The first order has a mechanical watch to content, the follow up order has a digital watch to content and the WatchFaceMountingCell needs a retooling.

To determine that the cell needs a retooling, the SetupManagement asks the WatchFaceMountSetupTrigger if a Setup is required.
The SetUp Manager calls the Required Method to indicate whether it needs to retool it self. In our sample implementation we see that no setup is required if the passed ```IProductRecipe``` is null or the passed ```IProductRecipe``` is none of our concerns.
Otherwise a retooling of the WatchFaceMountingCell is required.

The SetupManager now,  knows whether he has to call the CreateStep Method of the trigger to extend the workplan with the needed Workplan steps to create a prepare setup job.

```cs
[ExpectedConfig(typeof(WatchFaceMountSetupTriggerConfig))]
[Plugin(LifeCycle.Transient, typeof(ISetupTrigger), Name = nameof(WatchFaceMountSetupTrigger))]
internal class WatchFaceMountSetupTrigger : SetupTriggerBase<WatchFaceMountSetupTriggerConfig>
{
    public override SetupExecution Execution => SetupExecution.BeforeProduction;

    public override SetupEvaluation Evaluate(IProductRecipe recipe)
    {
        if (!((WatchRecipe)recipe).Target.Type.Equals(nameof(WatchProduct)))
            return false;

        return true; // return SetupClassification.Manual -- return SetupEvaluation.Provide(new MountWatchfaceCapabilities())  -- ...
    }

    public override IWorkplanStep CreateStep(IProductRecipe recipe)
    {
        return new WatchFaceMountSetupTask
        {
            Parameters = new WatchFaceMountSetupParameters()
            {
                FinishedOrderNumber = ((WatchRecipe)recipe).OrderNumber
            }
        };
    }
}
```

## Setup Job Creation

The SetupManagement is called by the JobManager when a new production job shall be started and checks if new SetupJobs are necessary. This depends on the JobSchedular which determines schedulable jobs from the remaining idle jobs.
This is application specific or depends on the configuration of the [DefaultScheduler]{@ ref Moryx.ControlSystem.Jobs.IJobScheduler}.