---
uid: ProcessEngine
---
# ProcessEngine

The Process Engine is a server module hosting some of the control system's major components groups:

- [Job Management](xref:ProcessEngine.JobManagement)
- [Process Controller](xref:ProcessEngine.ProcessController)
- [Setup Management](xref:ProcessEngine.SetupManagement)

Its mission is to provide these components with a common life cycle and to export public APIs to be used by other components.
In addition it imports all global components which are needed.

## Provided Facades

The Process Engine exports facades and forwards external requests to the internal components.

- [IJobManagement](xref:Moryx.ControlSystem.Jobs.IJobManagement)
- [IProcessControl](xref:Moryx.ControlSystem.Processes.IProcessControl)

## Dependencies

The Process Engine depends on the following APIs and data-models. Detailed documentation can be found by clicking the dependencies link.

## Referenced Facades

|Plugin API | Start dependency | Optional | Usage |
|-----------|------------------|----------|------ |
| IRecipeProvider | Yes | Yes | Each Job references a recipe by its provider and its ID. |
| IProductManagement | Yes | No | The ProductManagement is used to create the Article data for each article produced by a production process. |
| IResourceManagement | Yes | No | The ResourceManagement is used the get the resources needed to execute an activity. |

## Data Models

- [Moryx.ControlSystem.ProcessEngine.Model](xref:Moryx.ControlSystem.ProcessEngine.Model)

## Architecture

As described above, one of the Process Engines major task is to forward request from the outside to the embedded plugins. 
This is reflected by the module's architecture. The module provides the facades to the outside and holds the plugins.

![ProcessEngine Overview](images\ProcessEngineOverview.png)

![General Class Structure](images\GeneralClassStructure.png)

## Scheduling

When describing the behavior of schedulers as well as comparing them, the concept of scheduling slots is important. A slot holds a group of adjacent jobs of the same recipe enclosed in its pre- and post production setups. Jobs that currently occupy a slot are executed, while others wait for a slot. 

**ParallelScheduler:**
The parallel scheduler allows for a certain number of jobs to dispatch processes in parallel. It will start by executing the initial setup, then the production jobs and any follow-up jobs it might have. If the last job of that recipe is completed the clean-up will be executed. After completion of the Clean-Up the slot is released and can be assigned to the next group of jobs and their setups. Job groups that currently hold a slot can be increased with follow-up jobs anytime until the post production setup is completed.

Setting the number of slots to **1** results in job scheduling without overlap, where the machine is cleared after every group of jobs, before a new one is started. Any value greater than **1** gives true parallel execution. Jobs that currently hold a slot can be increased until the post production setup is started.

This scheduler is intended for single job execution or larger machines with true parallel production, compared to mixed production caused by seamless scheduling explained below.

**SeamlessScheduler:**
The seamless scheduler is intended for assembly lines that should not be cleared between different orders, but only have a single running job (inserting parts) while an unlimited number of jobs might still be completing within the machine. As soon as the last job of a group is completing, a new one **can** be started but until then it keeps the running slot for possible follow-ups. As soon as a new one is started, the previous one receives its own completion slot and can no longer be increased with follow-up jobs. The completion slot is destroyed when the job group completes its post production setup. If a completing job is extended with a follow-up jobs, it takes back the running slot, when it becomes available.