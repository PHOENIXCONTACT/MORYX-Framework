---
uid: Workplans
---
# Workplans

A [Workplan](xref:Marvin.Workflows.IWorkplan) is the definition of the production flow of a product how it should be produced at a machine and which steps must be executed and in which order. Therefore it contains a list of steps and connectors.

## Workplan Step

A [workplan step](xref:Marvin.Workflows.IWorkplanStep) is basically the Task which must be implemented in an application to define the needed step in the production. The task will create the corresponding activity during the processing of the workplan.

## Connector

A [connector](xref:Marvin.Workflows.IConnector) is the connection between two steps to define the possible paths from the start of the workplan over the used tasks until an endpoint is reached.

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