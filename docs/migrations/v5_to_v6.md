# Migrate to Abstraction Layer 6
From this major release the main motivation was to harmonize and merge Facade interfaces, as well as tackling the issue raised in #59 by replacing some interfaces with their already provided implementations.
For more information, please refer to the respective paragraphs below.

## .NET Framework
At the beginning, however, we start by continuing the story told in the [MORYX-Core Migration Guide](https://github.com/PHOENIXCONTACT/MORYX-Core/blob/future/docs/migrations/v3_to_v4.md#migrate-to-core-v4).
MORYX Abstraction Layer no longer supports the legacy .NET Framework and is only available for .NET 6.0 and above. There is also no more WCF support or embedded kestrel hosting. Instead standard ASP.NET API-Controllers can be used that import MORYX components (kernel, modules or facade).

## Renamed Classes and Interfaces
- `SkipArticleStrategy` was renamed to `SkipInstanceStrategy`
- `IProductStrategyConfiguation` was renamed to `IProductStrategyConfiguration`
- `RuntimePlatform` was removed, instead `Platform` can be used directly now

## Changed Interfaces
Several interfaces created for minor confirm updates got merged or removed throughout the update
- `INamedTaskStep` (removed)
- `IProductInteraction` (removed)
- `IRecipeTemplating` -> `IRecipe`
- `IActivityProgress` -> `Tracing`
- `IProductTypeEntityRepository` -> `IProductTypeRepository`
- `IProductRecipeEntityRepository` -> `IProductRecipeRepository`
- The interfaces `IActivityTracing` and `ITracing` were removed. Instead use the class `Tracing` directly
- The previous `INotificationPublisher` interface was removed and the `INotificationPublisherExtended` interface was renamed to `INotificationPublisher` to replace it
- The interfaces `IProductManagementTypeSearch` and `IProductManagementModification` are now included in `IProductManagement` and obsolete
- The interfaces `IProductSearchStorage` and `IProductRemoveRecipeStorage` are now included in `IProductStorage` and obsolete
- The interfaces `IProductTypeSearch` is now included in `IProductTypeStrategy` and obsolete
- The interfaces `IResourceModification` and `IResourceModificationExtended` are now included in `IResourceManagement` and obsolete 
- There is an extra Facade implementing `IResourceTypeTree` called `ResourceTypeTreeFacade`
- `INotification` and `IManagedNotification` were removed. Use `Notification` instead.
  * Note: The previous structure including two interfaces kept developers from accidently overriding properties by providing getter only properties within `INotification`. With a similar intend, the relevant properties of `Notification` throw an `InvalidOperationException` when trying to override already defined values. You can circumvent this functionality, by overriding the respective properties in a derived class as you see fit.

## Moved Classes and Interfaces
Several interfaces and classes moved to other namespaces
- The following classes and interfaces moved from `Moryx.Products.Management.Importers` to `Moryx.AbstractionLayer.Products`
  - `ProductImporterConfig` 
  - `FileImportParameters`
  - `IProductImporter`
  - `ProductImportContext`
  - `ProductImporterBase`
  - `ProductImporterResult`
  - `PrototypeImportParameters`
- The following classes and interfaces moved from `Moryx.Products.Management.Modification` to `Moryx.AbstractionLayer.Products.Endpoints` (located in the Moryx.AbstractionLayer.Products.Endpoints package)
  - `PartConnector`
  - `PartModel`
  - `ProductCustomization`
  - `ProductDefinitionModel`
  - `ProductFileModel`
  - `ProductImporter`
  - `ProductInstanceModel`
  - `ProductModel`
  - `RecipeClassificationModel`
  - `RecipeDefinitionModel`
  - `RecipeModel`
  - `WorkplanModel`
  - `PartialSerialization`
- The following classes and interfaces moved from `Moryx.Resources.Interaction.Converter` to `Moryx.AbstractionLayer.Resources.Endpoints` (located in the Moryx.AbstractionLayer.Resources.Endpoints package)
  - `ModelToResourceConverter`
  - `ResourceQueryConverter`
  - `ResourceToModelConverter`
- The following classes and interfaces moved from `Moryx.Resources.Interaction` to `Moryx.AbstractionLayer.Resources.Endpoints` (located in the Moryx.AbstractionLayer.Resources.Endpoints package)
  - `ReferenceFilter`
  - `ReferenceTypeModel`
  - `ResourceModel`
  - `ResourceQuery`
  - `ResourceReferenceModel`
  - `ResourceTypeModel` 

## Package Changes
From the change to Web interfaces with REST APIs follow: 
 - `Moryx.Resources.Interaction` removed
 - `Moryx.AbstractionLayer.UI`removed
 - `Moryx.Products.UI` removed
 - `Moryx.Products.UI.Interaction` removed
 - `Moryx.Resources.UI` removed
 - `Moryx.Resources.UI.Interaction` removed
 - `Moryx.AbstractionLayer.Resources.Endpoints` added
 - `Moryx.AbstractionLayer.Products.Endpoints` added
 - `Moryx.WebShell` -> `Moryx.Launcher` (renamed)
 - `Moryx.Maintenance.Web` -> `Moryx.CommandCenter.Web` (renamed)

## Namespace changes
- `Moryx.Workflows` was renamed to `Moryx.Workplans`

## Changed Property and Method names
- In `PartSourceStrategy` `FromPartlink` was renamed to `FromPartLink`
- In `ProductModel` `FileModels` was renamed to `Files`. The former `Files` property was deleted
- In `Tracing` `Resource` was renamed to `ResourceId`
- The method `RecipeStorage.SaveWorkplan` was renamed to `RecipeStorage.ToWorkplanEntity`, since nothing is saved in this method and the name was quite confusing

## Other Changes
- The `Type` in the `ProductQuery` must be the full name.
- The Type in the `ProductRecipeEntity`, `ProductInstanceEntity` and `ProductTypeEntity` is the FullName including AssemblyName
- The ProductStorage uses the FullName of the classes to organize strategies, constructors and so on