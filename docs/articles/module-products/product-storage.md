---
uid: ProductsStorage
---
# Product Storage

Section [Product Definition](product-definition.md) describes how applications can create their own product structures in form of custom classes. In order to save and load those to and from the database it is also necessary to configure the product storage. The product model defines [`IGenericColumns`](/src/Moryx.Products.Model/Entities/IGenericColumns.cs) for all entities that represent business objects, which are usually derived and extended in applications. The product management comes with a range of plugins to store and load user defined product types, instances, partlinks and recipes in these structures. They can be configured through the maintenance web UI or using the modules configure console command. The configure command attempts to automatically map a product type, its part links, properties and instance to database columns using a range of conversion strategies.

Make sure that all your product definitions have properly configured mappings. Refer to the [Generic Strategies Documentation](generic-strategies.md) on how to configure the generic strategies for product types, instances, part links and recipes.

## Product Type Strategy

For each product of the application the storage must provide an [IProductTypeStrategy](/src/Moryx.Products.Management/Components/IProductTypeStrategy.cs). That can be either a custom implementation or the GenericProductStrategy for most products. The strategy defines different properties and methods that need to be implemented. To avoid redundant code it is recommended to derive all implementations from [TypeStrategyBase](/src/Moryx.Products.Management/Implementation/Storage/TypeStrategyBase.cs).

### Target Type

The target type property defines the scope of the strategy. It must return the same value that is returned by `product.Type` and should usually be defined by a constant field on the product definition.

### HasChanged

The storage saves each version of a product as a separate entity. To avoid duplicates the strategy needs to determine of anything has changed.

````cs
// In class WatchStrategy
public override bool HasChanged(IProduct current, IGenericColumns dbProperties)
{
    var watch = (WatchProduct) current;
    return Math.Abs(watch.Weight - dbProperties.Float1) > 0.01 
        || Math.Abs(watch.Price - dbProperties.Float2) > 0.01;
}
````

### Load Product

Loading products refer to the conversion from product entity to typed object of type [ProductType](/src/Moryx.AbstractionLayer/Products/ProductType.cs). The implementation usually follows a standard pattern.

* (optional) fetch extended repo and entity
* copy properties to product

An implementation for our watch would look like this:

````cs
// In class WatchStrategy
public void LoadType(IGenericColumns source, ProductType target)
{
    var watch = (WatchProduct)target;
    watch.Weight = source.Float1;
    watch.Price = source.Float2;
}

````

## Archive Product

The `SaveType`-methods save the typed `IProduct` object to the database. Similar to loading a product from the database, the base conversions are provided by the storage and only custom properties need to be saved manually:

* copy properties to entity

The code to archive our watch and watchface looks like this:

````cs
// In class WatchStrategy
public override void LoadType(IGenericColumns source, IProduct target)
{
    var watch = (WatchProduct)target;
    watch.Weight = source.Float1;
    watch.Price = source.Float2;
}
````

## Part Links

Just like product types the part link strategies need to be configured. For part links without any properties the `SimpleLinkStrategy` can be used, for easy types the `GenericLinkStrategy` and otherwise a custom strategy. Each link strategy represents a product part or collection of product parts.

The implementation of the NeedleLinkStrategy is implemented below. The `PartCreation` property defines whether the instance is constructed from the `ProductPartLink`-property of the type or restored only from the entities. Per default the product definition is used to avoid redundancy and improve object creation.

````cs
private class NeedleLinkStrategy : LinkStrategyBase<NeedleProduct>
{
    public override void LoadPartLink(IGenericColumns linkEntity, ProductPartLink target)
    {
        var needleLink = (NeedlePartLink)target;
        needleLink.Role = (NeedleRole)linkEntity.Integer1;
    }

    public override void SavePartLink(ProductPartLink source, IGenericColumns target)
    {
        var needleLink = (NeedlePartLink)source;
        target.Integer1 = (int)needleLink.Role;
    }
}
````

### Load Instances

As explained in [Product Definition](product-definition.md) product instances are supposed to be limited to instance attributes and part link attributes. This limitation should be extended and even increased for the instance storage. In a regular industry or production environment an application may have houndreds of different products, but it will soon have thousands or millions of instances. When it comes to instances every byte of wasted memory quickly turns into wasted storage in the dimensions of MegaBytes or GigaBytes. To reduce the required instance storage to an absolute minimum the [ProductStorage](/src/Moryx.Products.Management/Implementation/Storage/ProductStorage.cs) recreates instance objects from their products instead of trying to recreate them from the entity. This approach covers all instance attributes, that can be derived from the type definition like part link attributes - e.g. the role of our needle in a watch. All the strategy implementation has to do is copy instance properties from the entity to the created and typed object.

#### Sample Code

We only have to configure the instance strategy for the root instance. Watchface instances use the empty `GenericProductStrategy` implementation while needles are not saved at all by configuring the `SkipInstanceStrategy`. Not having to persist needles is one of the benefits of recreating the instance from the product instead from the entity. The only instance information `Role` is restored by creating an instance of the watch product. Keeping the previous two sections in mind this example shows how instance storage is supposed to be efficient and not pretty or even human readable.

````cs
public void LoadInstance(IGenericColumns source, ProductInstance target);
{
    var watchInstance = (WatchInstance)instance;

    // Restore instance attributes
    watchInstance.DeliveryDate = DateTime.FromBinary(source.Integer1);
    watchInstance.TimeSet = source.Integer2 > 0;
}
````

### Save Instances

The recommendations for instance storage obviously apply to `SaveInstance` as well. When writing the instance to the database keep in mind what information need to be stored and which can be recreated from the product and its parts. Once you identified those attributes split them into three groups:

* can be stored in the generic columns

#### Sample Code

````cs
public void SaveInstance(ProductInstance source, IGenericColumns target);
{
    var watchInstance = (WatchInstance)source;

    target.Integer1 = watchInstance.DeliverDate.Ticks
    target.Integer2 = watchInstance.TimeSet ? 1 : 0;
}
````
