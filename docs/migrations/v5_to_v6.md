# Migrate to Abstraction Layer 6
For this major release the main motivation was switching from our own MORYX Runtime to ASP.NET Core with MORYX extensions.
It includes replacing EntityFramework 6 with EntityFramework Core as well as the replacement of the First Level DI-Container. 
We also removed our own implementation of an injectable logger and will use the very similar logger API from Microsoft from now on.
Regarding the configuration of our module, we are able to simplify things here as well. Tbc...
Lastly, we remove the support for .NET Framework with the step to MORYX Core v4.
For more information, please refer to the respective paragraphs below.

## .NET Framework
MORYX Abstraction Layer no longer supports the legacy .NET Framework and is only available for .NET 6.0 and above. There is also no more WCF support or embedded kestrel hosting. Instead standard ASP.NET API-Controllers can be used that import MORYX components (kernel, modules or facade).

## Changed Interfaces
Several interfaces created for minor confirm updates got merged or removed throughout the update
- `INamedTaskStep` (removed)
- IActivityTracing, ITracing -> Tracing
- IRecipeTemplating -> IRecipe
- IActivityProgress -> Tracing
- IProductTypeEntityRepository -> IProductTypeRepository
- IProductRecipeEntityRepository -> IProductRecipeRepository
- `IProductStrategyConfiguation` was renamed to `IProductStrategyConfiguration`

## Package Changes

## Namespace changes

## Renamed Classes
- `SkipArticleStrategy` was renamed to `SkipInstanceStrategy`

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
- The following classes and interfaces moved from `Moryx.Products.Management.Importers`to  `Moryx.AbstractionLayer.Products`
  - `ProductImporterConfig` 
  - `FileImportParameters`
  - `IProductImporter`
  - `ProductImportContext`
  - `ProductImporterBase`
  - `ProductImporterResult
  - `PrototypeImportParameters`
