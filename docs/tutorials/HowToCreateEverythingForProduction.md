---
uid: HowToCreateEverythinForProduction
---
# How to create a process including parameters, capabilities, activities and tasks?
When you have a product and the needed resources, this tutorial explains how to create everything else that is required in order to produce the product.

## Create the steps needed for production
First you have to define the steps needed for your use case. In this example, we will try to bake a cake. Considering the steps needed, we will simplify the process a lot: 
1. Putting everything together --> Assemble
2. Mix everything --> Mix
3. Bake the cake --> Bake
4. Pack the cake into a box --> Pack

Let's start by creating a new project in your assembly solution called `Moryx.Cake`. Then create the following project structure:
First add a folder for the steps called *Activies*. In this folder, create subfolders for each step including a task, an activity, parameters and if needed results. The task is the step in the workplan. The activity will be generated from the task during the production. With the parameters you can add additional parameters to the step. Results represent the outputs of a step. The [DefaultActivityResult](../../src/Moryx.AbstractionLayer/Activities/DefaultActivityResult.cs) contains the result *Success*, *Failure* and *Technical Error*. If you need different results, please create a custom one.

````fs
-Moryx.Cake
|-Activities
    |-Assemble
        |-AssembleActivity.cs
        |-AssembleTask.cs
        |-AssembleParameter.cs
        |-AssembleResult.cs
    |-Mix
        |-MixActivity.cs
        |-MixTask.cs
        |-MixParameter.cs
    |-Bake
        |-BakeActivity.cs
        |-BakeTask.cs
        |-BakeParameter.cs
    |-Pack
        |-PackActivity.cs
        |-PackTask.cs
        |-PackParameter.cs
````
Let's continue by implementing the paramters. Parameters have to implement the abstract class [Parameters](../../src/Moryx.AbstractionLayer/Activities/Parameters.cs). If the corresponding resource should show VisualInstructions during the activity, usw `VisualInstructionParameters` instead.

In the method `Populate()`, `instance` is the parameter of the just created activity in the current process. In this method you copy information from general parameters configured in the workplan to the specific ones.
```cs
public class AssembleParameters: Parameters{

    [EntrySerialize, DataMember]
    public string FlourType {get;set;}

    protected override void Populate(IProcess process, Parameters instance)
    {
        base.Populate(process, instance);

            var parameters = (AssembleParameters)instance;
            parameters.FlourType = FlourType;
    }
}
```

For the result create a simple enum.

```cs
public enum AssemblyResults
{
    [OutputType(OutputType.Success)]
    Success,
    [OutputType(OutputType.Failure)]
    Failed
}
```
An activity implements [Activity](../../src/Moryx.AbstractionLayer/Activities/Activity.cs). On the top it has the class attribute [ActivityResults](../../src/Moryx.AbstractionLayer/Activities/ActivityResult.cs), which defines the results of the step. 

Then we have to decide, if our activity needs a process. Putting everything together needs us to be in the possession of the goods carrier, i.e. a bowl. Otherwise the ingredients would be all over the place but not in the bowl they belong and which will be carried to the next step. In order for the process engine to match our activity to a suitable `Resource`, we have to define our required `Capabilities`. The `Resources` provide `Capabilities`, which define their abilities. And if needed and provided `Capabilities` match and the resource is ready to work, the activity will be routed to it.

```cs
[ActivityResults(typeof(AssembleResults))]
public class AssembleActivity: Activity<AssembleParameters>{
    public override ProcessRequirement ProcessRequirement => ProcessRequirement.Required;

    public override ICapabilities RequiredCapabilities => new AssembleCapabilities();

    protected override ActivityResult CreateFailureResult()
    {
        return ActivityResult.Create(AssembleResults.Failed);
    }

    protected override ActivityResult CreateResult(long resultNumber)
    {
        return ActivityResult.Create((AssembleResults)resultNumber);
    }
}
```
Tasks implement [TaskStep](../../src/Moryx.AbstractionLayer/Tasks/TaskStep.cs). The `DisplayName` is shown on the steps of a workplan in the Workplan UI.

```cs
[Display(Name = "AssembleTask", Description = "Task, which puts everythig together needed for a cake")]
public class AssembleTask : TaskStep<AssembleActivity, AssembleParameters>
{
    // Most tasks don't need any additional code
}
```

## Capabilities
In our example we said that in order to put everything together for a cake, we need a resource, which has `AssembleCapabilities`. Now let us take a look at how they are written. Capabilities always have to implement [CapabilitiesBase](../../src/Moryx.AbstractionLayer/Capabilities/CapabilitiesBase.cs). The method `ProvidedBy()` checks, if the capabilities provided by the resource (`provided`) meet the needs of the activity. If it's simple you just check if the class of the provided capability is correct. But if you have different resources which can put a different number ingredients together, the needed capabilities depend on the characteristics of the cake.

```cs
public class AssembleCapabilities : CapabilitiesBase
{
    protected override bool ProvidedBy(ICapabilities provided)
    {
        return provided is AssemblyCapabilities;  
    }
}
```

