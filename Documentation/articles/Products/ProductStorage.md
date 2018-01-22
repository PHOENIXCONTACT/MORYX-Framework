# Product Storage

Section [Product Definition](@ref productDefinition) describes how applications can create their own product structures in form of custom classes. In order to save and load those to and from the database it is also necessary to provide a custom storage. This task involves two steps - creating a merged model that holds the custom attribues and implementing [IProductStorage](@ref Marvin.Products.IProductStorage). Implementations of `IProductStorage` should derive from [ProductStorageBase](@ref Marvin.Products.Management.ProductStorageBase) because the base class handles recursive structures and provides methods for reoccuring patterns.

# Implement BuildMap
By deriving from [ProductStorageBase](@ref Marvin.Products.Management.ProductStorageBase) it is mandatory to implement the method `BuildMap()`. It returns an array of [IProductTypeStrategy](@ref Marvin.Products.Management.IProductTypeStrategy). Each strategy implementation handles all type specific operations like loading and saving products and articles as well as exporting an array of [ILinkStrategy](@ref Marvin.Products.Management.ILinkStrategy). Each strategy must also declare if it includes articles or if the are saved as part of their parent. Examples for the latter are small parts in great amounts like screws.

For simple products and articles without additional attributes the [DefaultProductStrategy](@ref Marvin.Products.Management.DefaultProductStrategy) can be used. Its constructor expects the `TypeName`-string as well as a boolean flag if articles shall be skiped. If only some of the default implementations should be used the class can also be derived and partially overridden.

If we wanted to implement [IProductStorage](@ref Marvin.Products.IProductStorage) for our watch product, our `BuildMap()` and file skeleton would look like the example below. Each storage must override the `Factory` property to define access to the merged model. If the model was not merged the `UseChild`-attribute must be removed. In our example we will not provide the article methods for the watch needles for reasons that are explained later.

````cs
[Plugin(LifeCycle.Singleton, typeof(IProductStorage))]
public class WatchProductStorage : ProductStorageBase
{
    [UseChild(WatchStorageConstants.Namespace)]
    public override IUnitOfWorkFactory Factory { get; set; }

    protected override IProductTypeStrategy[] BuildMap()
    {
        return new IProductTypeStrategy[]
        {
            new WatchStrategy(),
            new WatchfaceStrategy(), 
            new DefaultProductStrategy<NeedleProduct>(true, false)
        };
    } 
}
````

# Product Type Strategy

For each product of the application the storage must provide an [IProductTypeStrategy](@ref Marvin.Products.Management.IProductTypeStrategy). That can be either a custom implementation or the DefaultProductStrategy for simple products. The strategy defines different properties and methods that need to be implemented. To avoid redundant code it is recommended to derive all implementations from [ProductStrategyBase](@ref Marvin.Products.Management.ProductStrategyBase). If some of the methods shall use the default behavior it is recommended to derive from [DefaultProductStrategy](@ref Marvin.Products.Management.DefaultProductStrategy). 

## Target Type

The target type property defines the scope of the strategy. It must return the same value that is returned by `product.Type` and should usually be defined by a constant field on the product definition.

## IncludeParent

This flag indicates that the parent shall be included as well of instances of this product type are loaded. This is usually the case for sets with 1-on-1 relationships between a product and a part. In our example we assume that a watchface is only used for one watch because the model name is printed on the watchface. Needles however can be included in numerours watches and therefor do not activate this flag.

## Parts

The parts property must return an array of [ILinkStrategy](@ref Marvin.Products.Management.ILinkStrategy). Each link strategy represents a product part or collection of product parts. For links without custom properties the [DefaultLinkStrategy](@ref Marvin.Products.Management.DefaultLinkStrategy) can be used while custom propertiers require a custom implementation of the interface. Instances of the default strategy should be created by using the static `CreateLink`-method.

For the `WatchProduct` strategy the parts property is implemented as shown below:

````cs
public class WatchStrategy : ProductStrategyBase, IProductTypeStrategy
{
    public WatchStrategy()
    {
        Parts = new ILinkStrategy[]
        {
            new DefaultLinkStrategy<WatchfaceProduct>(nameof(WatchProduct.Watchface)),
            new NeedleLinkStrategy()
        };
    }

    public ILinkStrategy[] Parts { get; private set; }
````

The implementation of the NeedleLinkStrategy is implemented below. The `PartCreation` property defines whether the article instance is constructed from the `ProductPartLink`-property of the type or restored only from the entities. Per default the product definition is used to avoid redundancy and improve object creation:

````cs
private class NeedleLinkStrategy : DefaultLinkStrategy<NeedleProduct>
{
    protected internal NeedleLinkStrategy() : base(nameof(WatchProduct.Needles))
    {
    }

    //public override PartSourceStrategy PartCreation => PartSourceStrategy.FromEntities;

    public override IProductPartLink Load(IUnitOfWork uow, PartLink linkEntity)
    {
        return new NeedlePartLink(linkEntity.Id);
    }

    // ReSharper disable once RedundantOverridenMember <-- For demonstration
    public override PartLink Save(IUnitOfWork uow, IProductPartLink link)
    {
        return base.Save(uow, link);
    }
}
````

## Load Product

Loading products refer to the conversion from product entity to typed object of type [IProduct](@ref Marvin.AbstractionLayer.IProduct). The implementation usually follows a standard pattern.

* (optional) fetch extended repo and entity
* copy properties to product

An implementation for our watch and watchface would look like this:

````cs
// In class WatchStrategy
public IProduct LoadProduct(IUnitOfWork uow, ProductEntity entity)
{
    // Load extended repo and entity here

    // Transform watch
    var watch = new WatchProduct
    {
        Weight = 123.1,
        Price = 1299.99
    };
    CopyToProduct(entity, watch);

    return watch;
}

// In class WatchfaceStrategy
public override IProduct LoadProduct(IUnitOfWork uow, ProductEntity entity)
{
    var watchface = (WatchfaceProduct) base.LoadProduct(uow, entity);
    watchface.Numbers = new[] {3, 6, 9, 12};
    return watchface;
}
````

## Archive Product

The `SaveProduct`-methods save the typed `IProduct` object to the database. Similar to loading a product from the database, the base conversions are provided by the [ProductStrategyBase](@ref Marvin.Products.Management.ProductStrategyBase) and only custom properties need to be saved manually:

* get repo and load/create entity
* copy properties to entity

The code to archive our watch and watchface looks like this:

````cs
// In class WatchStrategy
public ProductEntity SaveProduct(IUnitOfWork uow, IProduct product)
{
    var propRepo = uow.GetRepository<IProductPropertiesRepository>();

    var watch = (WatchProduct)product;

    var watchEntity = GetProductEntity(uow, product);
    if (!VersionEqualsProduct(watchEntity.CurrentVersion, product))
        CreateVersion(propRepo, product, watchEntity);

    return watchEntity;
}

// In class WatchfaceStrategy
public override ProductEntity SaveProduct(IUnitOfWork uow, IProduct product)
{
    var watchfaceEntity = GetProductEntity(uow, product);
    var properties = CreateVersion(uow.GetRepository<IProductPropertiesRepository>(), product, watchfaceEntity);

    var linkRepo = uow.GetRepository<IPartLinkRepository>();
    var watchFace = (WatchfaceProduct)product;

    // save watchNumbers
    //properties.Numbers = watchFace.Numbers;

    return watchfaceEntity;
}
````

## Load Articles

As explained in [Product Definition](@ref productDefinition) articles are supposed to be limited to instance attributes and part link attributes. This limitation should be extended and even increased for the article storage. In a regular industry or production environment an application may have houndreds of different products, but it will soon have thousands or millions of articles. When it comes to articles every byte of wasted memory quickly turns into wasted storage in the dimensions of MegaBytes or GigaBytes. To reduce the required instance storage to an absolute minimum the [ProductStorageBase](@ref Marvin.Products.Management.ProductStorageBase) recreates article objects from their products instead of trying to recreate them from the entity. This approach covers all instance attributes, that can be derived from the type definition like part link attributes - e.g. the role of our needle in a watch. All the strategy implementation has to do is copy instance properties from the entity to the created and typed object.

### Article State

An important instance attribute is the state of the article. By default the abstraction layer defines [ArticleState](@ref Marvin.AbstractionLayer.ArticleState). Applications can define additional states for articles like `PrintComplete` and include them into the same column by classic bit-shifting and logical OR. Only rule is not to override the first 8 bits used for the framework state definition. In the sample code you will see how the `TimeSet`-flag is bit-shifted into the state column. 

### Extension Data

Throughout MaRVIN we follow two different approaches of saving custom attributes on predefined entities - model merge and string columns (e.g. JSON). While products are extended with model merge, benchmarks revealed, that a string column is the smarter choice for articles. However due to column size and  performance the string format should be decided on a per project basis. Everything from a single numeric value to a JSON string is valid.

### Sample Code

As you can see in our definition of `BuildMap()` we only have to implement `LoadArticle` for the root article. Watchface articles use the `DefaultProductStrategy` implementation while needles are not saved at all. Not having to persist needles is one of the benefits of recreating the article from the product instead from the entity. The only instance information `Role` is restored by creating an instance of the watch product. Keeping the previous two sections in mind this example shows how article storage is supposed to be efficient and not pretty or even human readable.

````cs
public void LoadArticle(IUnitOfWork uow, ArticleEntity entity, Article article)
{
    var watch = (WatchArticle)article;

    CopyToArticle(entity, article, true);

    // Restore TimeSet flag
    watch.TimeSet = (entity.State >> 8) >= 1;

    // Restore date
    var binaryDate = long.Parse(entity.ExtensionData);
    watch.ProductionDate = DateTime.FromBinary(binaryDate);
}
````

## Save Articles

The recommendations for article storage obviously apply to `SaveArticle` as well. When writing the instance to the database keep in mind what information need to be stored and which can be recreated from the product and its parts. Once you identified those attributes split them into three groups:

* represents a state and can be bit-shifted into the state column
* can be stored as a string or JSON into `ExtensionData` column
* is best stored by using model merge

For each of the groups make sure to find the best mapping pattern. If you bit-shift multiple values into the state column define the binary layout of the available 56 bits. For the extension data attributes choose a string format that holds all values with as little overhead as necessary. 

### Sampe Code

For our watch I decided to write the `TimeSet`-flag to the state column and serialize the time stamp as a hexadecimal string to extension data. 

````cs
public ArticleEntity SaveArticle(IUnitOfWork uow, Article article)
{
    var watch = (WatchArticle)article;

    var entity = GetArticleEntity(uow, article);
    CopyToArticleEntity(article, entity, true);

    // Include TimeSet-flag in state
    if (watch.TimeSet)
        entity.State |= (1 << 8);

    // Save date as binary
    var binaryDate = watch.DeliveryDate.ToBinary();
    entity.ExtensionData = binaryDate.ToString("X");

    return entity;
}
````