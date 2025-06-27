# Cell Selector

The process engine determines an activities targets at runtime based on the activities `RequiredCapabilities` and the capabilities provided by instances of `ICell` with the ResourceManager. Without any customization all cells that provide the `RequiredCapabilities` are set as targets in the order they were returned by the `ResourceManagement`. To exclude cells or reorder their priority you can implement `ICellSelector` and configure it in your applications process execution module. It is recommended to derive from `CellSelectorBase` to reduce boiler plate code.

In the `SelectCells` method you can remove or reorder cells from the available cells. Through the `activity` argument you logic can be activity or parameter specific and you can access a processes history. All configured selectors are called in a chain based in their configured `SortOrder`. Keep that in mind to either filter based on activity type or combine different selectors.

The example below shows the simplest implementation of a load balancer. It shuffles the targets on every call:

````cs
[Plugin(LifeCylce.Transient, typeof(ICellSelector), Name = nameof(LoadBalancer))]
internal class LoadBalancer : CellSelectorBase<CellSelectorConfig>
{
    public override IReadOnlyList<ICell> SelectResources(IActivity activity, IReadOnlyList<ICell> availableCells)
    {
        // Shuffling the list every time creates load balancing
        availableCells.ToList().Shuffle();
        return availableCells;
    }
}
````

If your selector needs additional configuration data, you can derive the config class. Make sure to add `[ExpectedConfig(typeof(LoadBalancerConfig))]` on your class.

````cs
internal class LoadBalancerConfig : CellSelectorConfig
{
    public override string PluginName 
    {
        get { return nameof(LoadBalancer); }
        set { }
    }

    [DataMember, PossibleTypes(typeof(Activity))]
    public string TargetType { get; set; }
}
````