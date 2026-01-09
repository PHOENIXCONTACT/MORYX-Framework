# Simulation parameters

If you need to influence the simulation behavior of activities you can add `ISimulationParameters` to your parameters and return different execution times either by configuration or through binding on digital twins.

## Configurable execution time

The example below shows how to implement configurable execution time parameters

````cs
internal class ConfiguredSimulationParameter : Parameters, ISimulationParameters
{
    [EntrySerialize(EntrySerializeMode.Always)]
    [Description("Execution time in seconds")]
    public int ExecutionTimeSec { get; set; }

    TimeSpan ISimulationParameters.ExecutionTime => new TimeSpan(0, 0, ExecutionTimeSec);

    /// <inheritdoc />
    protected override void Populate(IProcess process, Parameters instance)
    {
        var parameters = (ConfiguredSimulationParameter)instance;

        parameters.ExecutionTimeSec = ExecutionTimeSec;
    }
}
````

## Binding execution times

You can also determine execution time by binding like any other parameters. You could bind to static values or calculate execution time dynamically.

````cs
internal class BindingSimulationParameters : Parameters, ISimulationParameters
{
    [EntrySerialize(EntrySerializeMode.Never)]
    public TimeSpan ExecutionTime { get; set; }

    /// <inheritdoc />
    protected override void Populate(IProcess process, Parameters instance)
    {
        var parameters = (BindingSimulationParameters)instance;

        var recipe = (MyRecipe)process.Recipe;
        parameters.ExecutionTime = new TimeSpan(recipe.TestTimeSec);
    }
}
````