---
uid: Activities
---
# Activities

An [Activity](../../src/Moryx.AbstractionLayer/Activities/IActivity.cs) is the smallest separately executable step of a [Process](Processes.md). Activities are defined as classes derived from [Activity](../../src/Moryx.AbstractionLayer/Activities/Activity.cs) and are always specific to a certain task or application. Activities are instantiated by dedicated modules in their respective domains and then executed by resources. Each activity has a set of required [Capabilities](Capabilities.md) while the [production Resources](../Resources/Overview.md) each have a set of provided capabilities. All activities are stored permanently with the process for tracing purposes.

## Activity Structure

To create an activity it is necessary to derive at least the abstract class [Activity](xref:Moryx.AbstractionLayer.Activity). An implementation could look like the following:

```` cs
[ActivityResults(typeof(DefaultActivityResult))]
public class MyActivity : Activity
{
    public override ICapabilities RequiredCapabilities => new MyCapabilities();

    public override ProcessRequirement ProcessRequirement => ProcessRequirement.Required;

    public override string Type => nameof(MyActivity);

    protected override ActivityResult CreateFailureResult()
    {
        return Fail();
    }

    protected override ActivityResult CreateResult(long resultNumber)
    {
        return Complete(resultNumber);
    }
}
````

### Required Capabilities

This property defines what capabilities a resource must have to handle this activity. Maybe it is just a capability like `MyCapabilities` or a capability with parameters like:

```` cs
...
public override ICapabilities RequiredCapabilities => new ScrewingCapabilities(ScrewHeads.Default);
...
````

Then a resource will be searched which has the capability to handle at least default screw heads.

### Process Requirement

This property defines if a process is necessary to handle this activity.

* `Required` means that there must be a process
* `NotRequired` means that there can be a process
* `Empty` means that there must be no process

### Create Result

There are two methods to create a result for the activity. One to define a failed activity and one to define a result depending on a given number which encodes a result from the result enum of the activity. A more detailed discussion of activity results can be found [later in this article](Activities.md#activity-results).   

## Activity Parameters

Is there some information which is necessary to handle the activity like an adapter number for an electrical test or something like that? Then it is necessary to define a paramater class for the activity and derive the activity from the [Activity<TParam>](xref:Moryx.AbstractionLayer.Activity%601>) class like in the following example:

```` cs
[ActivityResults(typeof(DefaultActivityResult))]
public class MyActivity : Activity<MyParameters>
{
    public override ICapabilities RequiredCapabilities => new MyCapabilities();

    public override ProcessRequirement ProcessRequirement => ProcessRequirement.Required;

    public override string Type => nameof(MyActivity);

    protected override ActivityResult CreateFailureResult()
    {
        return Fail();
    }

    protected override ActivityResult CreateResult(long resultNumber)
    {
        return Complete(resultNumber);
    }
}

internal class MyParameters : ParametersBase
{
    /// <summary>
    /// Number of the needed adapter for the electrical test
    /// </summary>
    public int AdapterNumber { get; set; }

    protected override ParametersBase ResolveBinding(IProcess process)
    {
        // Get the recipe from the current process to get the needed adapter number
        var recipe = (MyRecipe) process.Recipe;

        return new MyParameters
        {
            AdapterNumber = recipe.AdapterNumber
        };
    }
}
````

The method `ResolveBinding` will be called during the creation of the activity and in this case the given process will be used to get the [Recipe](xref:Moryx.AbstractionLayer.Recipes.IRecipe) which contains the needed adapter number for the electrical test. The number will be stored in the parameter class and will be available during the execution inside of the resource:

```` cs
// some resource code
private void OnMyActivtiy(StartActivity activity, MyActivity activity)
{
    var adapterNumber = activity.Paarameters.AdapterNumber;

    // Send needed adapter number to the plc which can check if the needed adapter is currently equipped
    PlcDriver.Send(new StartProcess
    {
        StationId = myId,
        ProgramNumber = adapterNumber
    });
}
// some more resource code
````
For more information on how a Driver works see [this article](../Tutorials/HowToBuildADriver.md).

## Activity Results

An actvity can have different results depending to the defined result enum. The default results are:

```` cs
public enum DefaultActivityResult
{
    Success = 0,
    Failed = 1,
    TechnicalError = 2
}
````

So just implement a new enum to have specific result for the application specific activity like:

```` cs
public enum MyActivityResults
{
    Success = 0,
    Hot = 1,
    Lit = 2,
    NotSoGood = 3,
    Nearly = 4,
    NextTimeItWillBeBetter = 5
}
````

To use the new activity results it is necessary to use the `ActivityResults` attribute like:

```` cs
[ActivityResults(typeof(MyActivityResults))]
public class MyActivity : Activity<MyParameters>
{
    // some activity code

    protected override ActivityResult CreateResult(long resultNumber)
    {
        return ActivityResult.Create((MyActivityResults)resultNumber);
    }

    protected override ActivityResult CreateFailureResult()
    {
        return ActivityResult.Create(MyActivityResults.NextTimeItWillBeBetter);
    }
}
````

## An Example using the VisualInstruction Resource
If the activity is used for [VisualInstruction](xref:Moryx.Resources.Samples.IVisualInstructor), the UI will create a button for each possible result from the result enum. It is also possible to show a different text at the visual instruction or hide a result like in the following example:

```` cs
public enum MyActivityResults
{
    [VisualInstruction(Hide = true)]
    Success = 0,

    [VisualInstruction("Good job")]
    Hot = 1,

    [VisualInstruction("That's nice")]
    Lit = 2,

    [VisualInstruction("Nah")]
    NotSoGood = 3,

    [VisualInstruction(Hide = true)]
    Nearly = 4,

    [VisualInstruction("That's not my fault")]
    NextTimeItWillBeBetter = 5
}
````

By implementing the [IVisualInstruction](xref:Moryx.Resources.Samples.IVisualInstructor) interface we can also define which text to display alongside the possible results giving the required instructions for the user.

## Tracing
For long-term tracibility and to resume interrupted activities it is possible to use [ActivityTracing](xref:Moryx.AbstractionLayer.Activities.ActivityTracing).
Using the `Progress` property of the base class [Tracing](Moryx.AbstractionLayer.Activities.Tracing) derived types can trace intermediate progress during an activities execution. The example below shows how to define an enum for the progress.

````cs
public enum FooProgress
{
    Initial = 0,
    Running = 50,
    Done = 100
}
public class FooTracing : Tracing
{
    public new FooProgress Progress
    {
        get { return (FooProgress)base.Progress; }
        set { base.Progress = (int)value; }
    }

    // Relative progress defined by Tracing
    public double Relative => base.Progress;
}
````

Resources can access and transform an activities tracing information using the fluent API [TracingExtension](xref:Moryx.AbstractionLayer.Activities.TracingExtension). This rather complex approach is taken because the type of the tracing object might change at runtime depending on the resource executing the activity or the circumstances of the execution.
For the example above this would look like the following:

````cs
FooTracing tracing = activity.TransformTracing<FooTracing>()
  .Trace(t => t.Progress = FooProgress.Running);
````
