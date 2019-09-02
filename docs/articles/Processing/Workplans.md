---
uid: Workplans
---
# Workplans

A [Workplan](xref:Marvin.Workflows.IWorkplan) is the definition of the production flow of a product how it should be produced at a machine and which steps must be executed and in which order. Therefore it contains a list of steps and connectors. The diagram is a model on its own. The workflow control model does not contain any information of a diagram.

## Workplan Step

A [workplan step](xref:Marvin.Workflows.IWorkplanStep) is basically the Task which must be implemented in an application to define the needed step in the production. The task will create the corresponding activity during the processing of the workplan.

## Connector

A [connector](xref:Marvin.Workflows.IConnector) is the connection between two steps to define the possible paths from the start of the workplan over the used tasks until an endpoint is reached.

## Meta-Model

- An activity is the base type for all kinds of activities. An Activity is executed by a resource. An activity references the process it belongs to.
- An activity can have one or more aspects.
- A production activity represents the execution of a production step.
- An production activity can have access to the data of a product.
- A process step can have a material which is available for production. If no material is given, the production activity provides some material, e.g. the worker puts some material on a WPC.
- A process is a series of activities.
- A production process contains a reference to the data of the workpiece the process is physically working on.
- A task is used to define and create an Activity.
- A work plan step is a part of a work plan and references a Task.
- The workflow references a work plan as its defintion and uses tasks to create activities. A workflow is a living object created for each process. For production processes there is one process and one workflow for each workpiece.
- Though workpiece is usually not used for electronic devices, it is at least unambiguous. Therefore the term "Article" shall be replaced with "Workpiece". Workpiece fits very well to the term "Workpiece Carrier".

## Model Semantics

- A process step starts its execution when all available aspects are able to execute.
- An aspect of an activity can perform actions when checking if the activity is able to execute.

## Concept

- Workflows are handled as Coloured Petri Nets
- Every work plan step corresponds to a Petri Net

!["Petri Net](images/petrinet.png)

Unlike MARVIN Classic, there shall be not one single large workplan but separate small workplans to get from one construction level to the next one.
In addition to the work plan steps work plans may contains splits and joins to support parallel processing. Therefore there will be not one single token inside the net as in MARVIN Classic but a group of tokens. 

The ProcessController will be changed to get a list of activities instead of a single one. The list may be empty if there is a join still waiting for other tokens. The workflow finishes if the list of activities is null.

Using reflection, it is quite easy to navigate through the properties of an object tree. The workflow editor could load the product definition from the control system or directly from TeamCenter. The reference to the property could be stored like an XPath.

The work plan editor validates the work plan for soundness.

To support identical (or at least compatible) production facilities, the work plan editor can push a work plan to more than one facility.

The Control System checks the workplan on import for compatibility.

A work plan has a version identifier. There is some kind of release management needed. The process tracing data refers one distinct version of the work plan.

To reduce waist there must be a possibility to change the work plan version while the process is running. To change should be allowed only if the finished and the current steps are equal to the new definition. It should be quite easy to check the tracing data whether the new definition would have led to the same tracing result. It should be forbidden to change the version if the tracing data does not fit to the new one.

For each group of splits and joins leading to parallel threads there must be a bounding box with exactly one entry and one exit. The only exception of this rule is a global abort, if the process shall be terminated in case of an error. Its difficult (or even impossible) to validate termination if loops are allowed, because the termination of the loop cannot be validated at all.


## Create a Workplan

The most comfortable way is to use the workplan editor to draw the workplan with the created task. But it is also possible to create a workplan programmatically to use it for example during the product import. A self created workplan could be looks like that:

```` cs
// Prepare workplan
var workplan = new Workplan { Name = "My Workplan" };

// Boundaries
var start = workplan.AddConnector("StartConnector", NodeClassification.Start);
var end = workplan.AddConnector("End", NodeClassification.End);
var failed = workplan.AddConnector("Failed", NodeClassification.Failed);

// The first input is the start connector
var input = start;
// Output of the success path
var output = workplan.AddConnector("Mounted");
workplan.AddStep(new MountTask(), new AssembleParameters(), input, output, failed);

// The output is the input for the next task
input = output;
output = workplan.AddConnector("First task done");
workplan.AddStep(new MyTask(), new MyParameters(), input, output, failed, failed);

input = output;
output = workplan.AddConnector("Second task done");
workplan.AddStep(new MyTask(), new MyParameters(), input, output, failed, failed);

input = output;
output = workplan.AddConnector("Unmounted");
workplan.AddStep(new UnmountTask(), new AssembleParameters(), input, end, failed, failed);

// Validate the workplan of all parameters fits the task results
workplan.Validate();

// Save the workplan
RecipeStorage.SaveWorkplan(openContext, workplan);
````

The method `AddStep` of the workplan takes a list of connectors which must fit the result enum of the corresponding activity of the used task. If it does not fit then the validation will throw an exception. In the shown example has the MountTask only two outputs where the first one is the `Mounted` connector and the last goes directly to the `Failed` output. The other activities have three possible result where two of them goes directly to the failed output. The `UnmountTask` is the last task and its first output which is the success path goes to the `End` output to close the good path of this workplan.