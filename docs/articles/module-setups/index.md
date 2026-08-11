---
uid: SetupProvider.SetupManagement
---
# SetupManagement

The SetupManagement generates the jobs that are required for the setup of a production system. A distinction can be made between preparatory setup jobs and follow-up cleanup jobs. Running a setup / cleanup job sends setup activities to the resources of the system. These activities inform the resources that a setup is required for the next job and provide the required information.

Both types, before and after the production job, are evaluated as late as possible. They are created when a production job is flagged ready by the scheduler and the prepare job is started. The clean-up evaluation is only a place-holder and reevaluated when clean-up is started by the scheduler, to achieve the most accurate behavior.

## SetupTriggers

[SetupTriggers](xref:Moryx.ControlSystem.Setups.ISetupTrigger) inform the SetupManagement that the system needs a setup. `SetupTrigger` are plugins which implement the `ISetupTrigger` interface and can be customized for the needs of the different resources. When starting a new job (for example, form the OrderUI), the [SetupManager](xref:Moryx.ControlSystem.ProcessEngine.Setup.SetupManager) calls the `Evaluate` method of all registered SetupTriggers. Unless this method returns `SetupEvaluation.Empty`, the corresponding `CreateStep`-method of the SetupTrigger is executed, which returns a [IWorkplanStep](xref:Moryx.Workflows.IWorkplanStep). Now the [SetupManager](xref:Moryx.ControlSystem.ProcessEngine.Setup.SetupManager) uses the returned WorkplanSteps to put it together to create [Workplans](xref:Moryx.Workflows.IWorkplan). The order of steps is determined by the triggers sort order. If two triggers have the same sort order, their steps are executed in parallel by using split and join.

See [SetupTriggers](./setup-triggers.md) for more details.

## Setup Job Creation

The SetupManagement is called by the JobManager when a new production job shall be started and checks if new SetupJobs are necessary. This depends on the JobScheduler which determines schedulable jobs from the remaining idle jobs.
This is application specific or depends on the configuration of the [DefaultScheduler]{@ ref Moryx.ControlSystem.Jobs.IJobScheduler}.
