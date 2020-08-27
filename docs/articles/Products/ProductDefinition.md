---
uid: ProductsDefinition
---
# Product Definition

The product definition is done on two levels. Level 1 defines the product group and level 2 a concrete variant from this group. Following the type-instance-pattern this is still on the type side. Within MORYX instances of products are called product instances. When defining products and instances, it is important to avoid redundant information. All instances reference their product type definition. Article attributes should
be limited to those of the concrete physical instance like production date, serial number or quality control result.

## Product - Level 1

The level 1 product group definition is done by creating a class used to represent instances of the group. Such a group and its respective class could be a [watch](xref:Moryx.Products.Samples.WatchProduct) or one of its [needles](xref:Moryx.Products.Samples.NeedleProduct). The properties of the class define attributes each product of the group defines. Properties derived from `ProductPartLink` reference other  products that represent parts. The product base class defines standard attributes like database id, name and identity. Each product class also implements a method `CreateInstance()` that acts as a strategy to create instances for this type of product.

## Product Parts

In most cases it makes sense to model complex products as compositions of parts instead of one huge product. Those references to products are not represented as simple properties but with an explicit link object. This structure of [IProductPartLink](xref:Moryx.AbstractionLayer.IProductPartLink) was created because in many cases the reference carriers attributes of its own that are neither part of the product nor its part but rather define the relationship between them. For example a watch could have two or three needles that each indicate a respective part of the time (hours, minutes, seconds, stopwatch). The role of each needle could be a part of the needle product definition, but then it would not be possible to use a certain needle for seconds on one watch and for a stopwatch on another one. Furthermore if you think of the watch needle as an isolated product, its attributes rather include size, color and shape - but not its role in a watch. To define custom attributes for relationships between products a class derived from [ProductPartLink<TProduct>](xref:Moryx.AbstractionLayer.ProductPartLink) must be created and used within the product.

## Create Instance

Each product must provide a method to create typed instances of itself. Those instances are represented by classes derived from [ProductInstance](xref:Moryx.AbstractionLayer.ProductInstance) as described below. To create instances of complex products a recursive approach is used. Each product only creates an instance of itself and fills its parts by creating instances of its parts. Unlike products, instances do not define link objects for references but the link attributes become part of the instance attributes. This makes sense if you think about our previous example of watch needles. While the needle product may be used in different roles in different watches the one concrete needle used to assemble a single watch most definitly has one and only one role.

### Sample code

The level 1 definition for a watch, its watchface and needles is shown below. This is not the complete definition. The full definition can be found [here](xref:Moryx.Products.Samples).

**The watch itself:**

````cs
public class WatchProduct : ProductType
{
    // Watch attributes
    public double Price { get; set; }
    public double Weight { get; set; }

    // References to product
    public ProductPartLink<WatchfaceProduct> Watchface { get; set; }
    public List<NeedlePartLink> Needles { get; set; }

    protected override ProdectInstance Instantiate()
    {
        return new WatchInstance
        {
            Watchface = (WatchfaceInstance)Watchface.Instantiate(),
            Neddles = Needles.Instantiate<NeedleInstance>()
        };
    }
}
````

**The link between a watch and its needles defines the role of the referenced needle:**

````cs
public enum NeedleRole
{
    Hours,
    Minutes,
    Seconds,
    Stopwatch
}

public class NeedlePartLink : ProductPartLink<NeedleProduct>
{
    public NeedlePartLink()
    {
    }

    public NeedlePartLink(long id) : base(id)
    {
    }

    public NeedleRole Role { get; set; }

    public override ProductInstance Instantiate()
    {
        var needle = (ProductInstance) base.Instantiate();
        needle.Role = Role;
        return needle;
    }
}
````

## Product Level 2

Level 2 of the product definition defines concrete product variants within a group. Speaking in code those are instances of the classes. These variants differ in their values for the attributes defined in the first step and the products referenced as parts. For example different watches have different prices, analog or digital watchfaces and two or three  needles. The product here is only an example and a real watch product would probably be a lot more complex. If level 1 is achieved by writing a class definition, then level 2 is achieved by creating instances of this class. Each instance also carriers its product identity that can be any implementation of [IIdentity](xref:Moryx.AbstractionLayer.Identity.IIdentity). We only use [ProductIdentity](xref:Moryx.AbstractionLayer.ProductIdentity) that contains an identifier and a revision.

### Sample Code

The example below illustrates how the object tree for a product might look like. While the object tree might represent a real product, the way it is created does not. Normally product objects are fetched from the [ProductStorage](xref:Moryx.Products.Management.IProductStorage).

````cs
var tagHeuer = new WatchProduct
{
    Name = "TagHeuer Premium",
    Identity = new ProductIdentity("12345", 02),
    Price = 1299.99,
    Weight = 123.5,
    Watchface = new ProductPartLink<WatchfaceProduct>
    {
        Product = new WatchfaceProduct
        {
            Name = "TagHeuer Analog Classic",
            Identity = new ProductIdentity("334455", 01),
            IsDigital = false,
            Numbers = new[] { 12, 3, 6, 9 }
        }
    },
    Needles = new List<NeedlePartLink>
    {
        new NeedlePartLink
        {
            Role = NeedleRole.Hours,
            Product = new NeedleProduct
            {
                Name = "FatNeedle",
                Identity = new ProductIdentity("111111", 1),
                Length = 15
            }
        },
        new NeedlePartLink
        {
            Role = NeedleRole.Minutes,
            Product = new NeedleProduct
            {
                Name = "ThinNeedle",
                Identity = new ProductIdentity("22222222", 2),
                Length = 22
            }
        }
    }
};
````

## Instance Definition

Instances of products are defined as classes and in most cases there is a 1:1 relation between an instance and its product. In some exceptions two product types create the same instance type and in theory a product may create instances of different types based on its properties. Below are fictional examples for both cases that not necessarily make sense in real world applications:

* Analog and digital watches are represented by two product classes `AnalogWatchProduct` and `DigitalWatchProduct` to avoid having an empty collection of needles in digital watches, but both create `Watch` instances * The `WatchfaceProduct` creates instances of `AnalogWatch` or `DigitalWatch` depending on the value of `IsDigital` of the watch face.

All instance class definitions must be derived from [ProductInstance](xref:Moryx.AbstractionLayer.ProductInstance) and only define instance properties valid for the single physical instance. The base class already defines instance properties like state, production date or identity (serial number). Articles also define references to instances of its parts.

### Sample Code

For our watch example the instance definition is shown below. Possible instance attributes for a watch could be the delivery date in addition to the production date for the warranty times. Another instance field is the `TimeSet`-flag if the time was preset to the current local time.

````cs
public class WatchInstance: ProductInstance<WatchProduct>
{
    public bool TimeSet { get; set; }

    public DateTime DeliveryDate { get; set; }

    public WatchfaceInstance Watchface { get; set; }

    public ICollection<NeedleInstance> Neddles { get; set; }
}
````
