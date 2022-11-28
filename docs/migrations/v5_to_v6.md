# Migrate to Abstraction Layer 6

## Changed Interfaces
Several interfaces created for minor confirm updates got merged or removed throughout the update
- `INamedTaskStep` (removed)
- IActivityTracing, ITracing -> Tracing
- IRecipeTemplating -> IRecipe
- IActivityProgress -> Tracing
- IProductTypeEntityRepository -> IProductTypeRepository
- IProductRecipeEntityRepository -> IProductRecipeRepository
- `IProductStrategyConfiguation` was renamed to `IProductStrategyConfiguration`


## Changed Property names
- In `PartSourceStrategy` `FromPartlink` was renamed to `FromPartLink`
- In the `ProductModel` `FileModels` were renamed to `Files`. The former `Files` property was deleted

## Changed Facades or Interfaces of Facades
- `INotificationPublisher` was removed and the `INotificationPublisherExtended` was renamed to `INotificationPublisher`
- The interfaces `IproductManagementTypeSearch` and `IProductManagementModification` are now included in `IProductManagement` and obsolete
- The interfaces `IProductSearchStorage` and `IProductRemoveRecipeStorage` are now included in `IProductStorage` and obsolete
- The interfaces `IProductTypeSearch` is now included in `IProductTypeStrategy` and obsolete
- The interfaces `IResourceModification` and `IResourceModificationExtended` are now included in `IResourceManagement` and obsolete 
- There is an extra Facade implementing `IResourceTypeTree` called `ResourceTypeTreeFacade`

## Other Changes
- The `Type` in the `ProductQuery` must be a assembly qualified name. Use `typeof(TType).AssemblyQualifiedName` instead of `nameof(TType)`
- The Type in the `ProductRecipeEntity`, `ProductInstanceEntity` and `ProductTypeEntity` is the FullName including AssemblyName
- The ProductStorage uses the FullName of the classes to organize strategies, constructors and so on