# Setup Triggers

A SetupTrigger is used to determine if a changeover is required before or after a jobs by its recipe and creates the necessary `WorkplanStep`.

By implementing the `ISetupTrigger` interface or deriving the `SetupTriggerBase` class, specific setup triggers can be created for different cells of a production system.

The interface provides one property and two methods:

```cs
SetupExecution Execution { get; }
```

Determines whether the trigger is relevant **before** or **after** a production job.

```cs
SetupEvaluation Evaluate(IProductRecipe recipe);
```

`SetupEvaluation` represents the result of the evaluation by the trigger. It is possible to assign a classification like `Manual` or `MaterialChange` or combine different classifications. Derived types represent capability changes or reservations. Those evaluations also include capabilities which are used to determine setup necessity as well as machine change projections.

For example if you want to indicate a manual material change with modified capabilities, it would look like the example below
```cs
public override SetupEvaluation Evaluate(ProductionRecipe recipe)
{
    return SetupEvaluation.Provide(new InsertHousingCapabilities{ Identifier = recipe.Product.Housing.Identitiy}, SetupClassification.Manual | SetupClassification.MaterialChange);
}
```

Because of defined implicit casts it is also possible to directly return `bool` or `SetupClassification` instead of the evaluation object.

```cs
IWorkplanStep CreateStep(ProductionRecipe recipe);
```

Provided that `Evaluate` indicates the need for a machine change, this method creates a `WorkplanStep` to alter a target resource in a way that it provides the necessary capabilities afterwards.

**Return multiple setup steps**

With the extended interface `IMultiSetupTrigger.CreateSteps` it is possible to return an array of steps.

### Example of Setup Trigger using the WatchProduct example

#### Preconditions

- There is a Machine which is able to produce digital watches as well as mechanical watches
- There is a cell which is responsible for the mounting of watchfaces on the watch.
- This cell is called WatchFaceMountingCell.
- The first order has a mechanical watch to content, the follow up order has a digital watch to content and the WatchFaceMountingCell needs a retooling.

To determine that the cell needs a retooling, the SetupManagement asks the WatchFaceMountSetupTrigger if a Setup is required.
The SetUp Manager calls the Required Method to indicate whether it needs to retool it self. In our sample implementation we see that no setup is required if the passed product on `IProductRecipe` is none of our concerns.
Otherwise a retooling of the WatchFaceMountingCell is required.

The SetupManager now knows whether he has to call the CreateStep Method of the trigger to extend the workplan with the needed Workplan steps to create a prepare setup job.

```cs
[ExpectedConfig(typeof(WatchFaceMountSetupTriggerConfig))]
[Plugin(LifeCycle.Transient, typeof(ISetupTrigger), Name = nameof(WatchFaceMountSetupTrigger))]
internal class WatchFaceMountSetupTrigger : SetupTriggerBase<WatchFaceMountSetupTriggerConfig>, IMultiSetupTrigger
{
    public override SetupExecution Execution => SetupExecution.BeforeProduction;

    public override SetupEvaluation Evaluate(ProductionRecipe recipe)
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

    // Optional method for more than one setup task
    public IReadOnlyList<IWorkplanStep> CreateSteps(IProductRecipe recipe)
    {
        return new IWorkplanStep[] 
        { 
            new WatchFaceMountSetupTask
            {
                Parameters = new WatchFaceMountSetupParameters()
                {
                    FinishedOrderNumber = ((WatchRecipe)recipe).OrderNumber
                }
            };
        }
    }
}
```