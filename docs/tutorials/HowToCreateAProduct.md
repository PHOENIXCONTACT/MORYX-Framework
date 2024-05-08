---
uid: HowToCreateAProduct
---
# How to create a product

This tutorial shows how [Products](../../src/Moryx.AbstractionLayer/Products/ProductType.cs) should be implemented. Look [here](../articles/Products/Concept.md) if you are not firm with the concept of a `Product`. 

For products we differentiate between [ProductType](../../src/Moryx.AbstractionLayer/Products/ProductType.cs) and [ProductInstance](../../src/Moryx.AbstractionLayer/Products/ProductInstance.cs). The `ProductType` is what you can order in a catalog, while the `ProductInstance` is what you received after ordering: an instance of the product with its unique serialnumber. So that a `ProductType` can be produced it needs a corresponding `ProductInstance`. If your application isn't used for production, you can skip the `ProductInstances`. 

## Create a basic ProductType and ProductInstance
All ProductTypes are derived from `ProductType`. The methode `Instantiate()` returns an object of the correspoding `ProductInstance`.

With the attibute `DisplayName` you can change the name displayed on the UI. Using `Description` adds descriptions to a property on the UI. All public properties of a `ProductType` will be displayed on the UI.
```cs
[DisplayName("Watch Needle")]
public class NeedleType : ProductType
{
    // Properties of the ProductType
    [Description("Length of the needle")]
    public int Length { get; set; }

    // Creates the corresponding instance of the productType
    protected override ProductInstance Instantiate()
    {
        return new NeedleInstance();
    }
}
```
All ProductInstances are derived from `ProductInstance<TProductType>`. Instance specific properties can be added. Depending on the size of the watch it could happen that the needle used for hours in one watch is used for minutes in another one (although this example is quite unlikely).
```cs
public class NeedleInstance : ProductInstance<NeedleType>
{
    // Instance specific properties
    public NeedleRole Role { get; set; }

    ...
}
```

## Use PartLinks
Some `ProductTypes` have components. These can be modeled using [PartLinks](../../src/Moryx.AbstractionLayer/Products/ProductPartLink.cs). If no additional properties are needed, use the basic `ProductPartLink`. This is done for the part *WatchFace* in the example.

```cs
[DisplayName("Watch")]
public class WatchType : ProductType
{
    // References to product
    [DisplayName("Watch face")]
    public ProductPartLink<WatchFaceType> WatchFace { get; set; }

    // References to product using costum part link
    [DisplayName("Watch needle")]
    public List<NeedlePartLink> Needles { get; set; } = new List<NeedlePartLink>();

    protected override ProductInstance Instantiate()
    {
        return new WatchInstance
        {
            //All parts need to be instantiated
            WatchFace = (WatchFaceInstance)WatchFace.Instantiate(),
            //The PartlinkExtension instantiates every NeedleType in Needles and adds them to a List
            Needles = Needles.Instantiate<NeedleInstance>()
        };
    }
}
```
If you want to add properties to the `PartLink`, you can create custom ones. In our example we used the costum part link `NeedlePartLink` to specify the role of the needle in the watch.
```cs
public class NeedlePartLink : ProductPartLink<NeedleType>
{
    public NeedlePartLink()
    {
    }

    public NeedlePartLink(long id) : base(id)
    {
    }

    // Partlink specific property
    [Description("Which role does the needle have")]
    public NeedleRole Role { get; set; }

    public override ProductInstance Instantiate()
    {
        var needle = (NeedleInstance) base.Instantiate();
        // Copy the partlink specific property to the instance
        needle.Role = Role;
        return needle;
    }
}
```
In `Instantiate()` we set the role of the instance to the role speficied in the PartLink by the user.

