---
uid: Workflows
---
Workflows
==========

As part of the _Marvin-package_ the namespace _Marvin.Workflows_ and its static class API entry-point `Workflow` provide a petry-net based implementation of a workflow engine. The namespace and its components can be grouped into workflow creation/representation and execution/monitoring.

## Workflow Modeling

The model of a workflow is called `Workplan` and implemented as an object graph of two types of classes - steps ([IWorkplanStep](xref:Marvin.Workflows.IWorkplanStep)) and connectors ([IConnector](xref:Marvin.Workflows.IConnector)). The workplan declares a collection for each type of object, namely `Steps` and `Connectors`. The two `IConnector`-arrays `Inputs` and `Outputs` on `IWorkplanStep` create the workflow graph as two steps referencing the same connector (one as output and one as input) are implicitly connected.

### Workplan Nodes

The previously mentioned connectors and steps share the common interface [IWorkplanNode](xref:Marvin.Workflows.IWorkplanNode). A workplan node declares two properties `Id` and `Name`. The `Name` is set manually while the `Id` is assigned when the node is added to the workplan. The `Id` is only unique to the workplan the node was added to but remains unchanged for its entire lifecycle.

**Connectors** are rather plain objects that, besides inherited properties, only declare their [classification](xref:Marvin.Workflows.NodeClassification). The _classification_ distinguishes between _Entry_ and _Exit_ points of the workplan or simple connections between two steps.

**WorkplanSteps** represent actions that are executed within the workflow. The workflow engine, as most of MARVIN, was created in the context of industrial automation where the steps represent manufacturing steps of a product. Independent from its origin the workflow engine was designed to be domain-indepent. For this purpose users of the workflow engine need to create implementations of `IWorkplanStep` for the actions of their domain.

To implement `IWorkplanStep` it is recommended to derive from `WorkplanStepBase`, which already implements most of the interface. Per default this defines a step with one input and one output. Besides the `Name` of the step the most important part to implement is the definition of outputs and creation of an executable `ITransition` from the step. While `IWorkplanStep` instances only represent an action in the workplan object graph, `ITransition` implement the executable code to perform the action. 

### Creating a workplan
Workplans can be created from code by creating the object graph or using the [IWorkplanEditing API](xref:Marvin.Workflows.IWorkplanEditing). For the latter there is also bundle for graphical editing of workflows.

Creating workplans from code is shown in the example below. The used `DummyStep` takes output count and name from its constructor, but for most steps these parameters will be determined by the action the step represents and its possible results.

````cs
var workplan = new Workplan();

var initial = Workflow.CreateConnector("Start", NodeClassification.Start);
var complete = Workflow.CreateConnector("End", NodeClassification.End);
workplan.Add(inital, complete);

var step = new DummyStep(2, "A");
step.Inputs[0] = initial;
workplan.Add(step);

var left = Workflow.CreateConnector("Left");
var right = Workflow.CreateConnector("Right");
workplan.Add(left, right);
step.Outputs[0] = left;
step.Outputs[1] = right;

var rightOnly = new DummyStep(1, "B");
rightOnly.Inputs[0] = right;
workplan.Add(rightOnly);
rightOnly.Outputs[0] = left;

var merge = new DummyStep(2, "C");
merge.Inputs[0] = left;
merge.Outputs[0] = merge.Outputs[1] = complete;
workplan.Add(merge);

return workplan;
````

## Workflow Execution

For execution the above mentioned _Workplans_ are instantiated to _Workflows_, which can than be executed by an instance of the [workflow engine](xref:Marvin.Workflows.IWorkflowEngine). The internal architecture is based on [the concept of petri-nets](https://en.wikipedia.org/wiki/Petri_net) with its transition and places. The execution is performed by tokens that are moved by transitions from one place to another. 

````cs
var workplan = MethodThatReturnsWorkplan();

var engine = Workflow.CreateEngine(workplan, new NullContext());
engine.TransitionTriggered += OnTransitionTriggered;
engine.Completed += OnEngineCompleted;
engine.Start();
````

 It is possible to pass a [context](xref:Marvin.Workflows.IWorkplanContext) to the workflow creation. This makes it possible to create workplans in a more universal way and add details during the creation of an individual instance. The context, like steps and transitions, are created domain specific. The context can contain instance specific parameters likes names or ids to avoid including them in the workplan. During creation of `ITransition` those values can be written to the new object from the context.

### Path Prediction

Sometimes the result of a workflow execution is foreseeable during execution even though the engine has not completed yet because the workplan defines final steps before the workflow was truly completed. However in order to save time it might make sense to process the expected result without awaiting the engines completion. Path prediction refers to the ability to analyze a workplan and identify paths that lead to only one possible outcome. Once the workflow engine enters that path during execution the result can be predicted **before** the workflow was completed.

In _Marvin.Workflows_ this feature is available in through the [PathPredictor](xref:Marvin.Workflows.IPathPredictor). An instance of the path predictor can be created per workplan and then used to monitor all engines executing an instance of afore-mentioned workplan. During creation the workplan is analyzed for predictable paths and once an instance enters that paths an event is published that contains the expected result in the form of a `NodeClassification`. 

````cs
var workplan = MethodThatReturnsWorkplan();
// Create predictor
var predictor = Workflow.PathPrediction(workplan);
predictor.PathPrediction += OnPrediction;
// Monitor a running engine
var engine = Workflow.CreateEngine(workplan, new NullContext());
predictor.Monitor(engine);
// Register events and start engine

private void OnPrediction(object sender, PathPredictionEventArgs eventArgs)
{
    var result = eventArgs.PredictedOutcome;
}
````