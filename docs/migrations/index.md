# Migrations

Like the other MARVIn layers the Abstraction Layer follows the Semantic Versioning standard, which means all MINOR/PATCH versions are compatible. However we do have breaking changes from one major version to the next. Depending on your current version you will find a short summary to migrate to the next major version as well was a database migration script.

# Abstraction Layer 3.x to 4.0

AL3 introduced the resource type tree and resource graph. The new major version AL 4.0 mainly bringes changes to the product and resource service, as well as their UIs. Inspired from user feedback and known limitations of the two dominant AL UIs, both interfaces were redesigned, both in code/architecture and user experience. Consequently the service and (in part) server side architecture were refactored to meet the new UIs requirements and enable additional features/UIs in the future without breaking changes.

## Database changes

As we removed unused features from the UI, we no longer needer their associated columns either. Therefore the database script only renames or drops colums and their constraints within the products model.

````sql
ALTER TABLE public."ProductEntity"
   RENAME COLUMN "MaterialNumber" TO "Identifier";
   
DROP TABLE public."RevisionHistory";
   
ALTER TABLE public."ProductProperties"
   DROP COLUMN "State";

ALTER TABLE public."ProductRecipeEntity"
   ALTER COLUMN "WorkplanId" DROP NOT NULL;
````

## Resource changes

Within the resource management a few deprecated interfaces and members were removed, as well as limitations for resolving resources through the graph or facade. Public resources no longer need capabilities to be resolved through the facade and within the resource management resources can now be resolved during boot phase, without the state limitation. All changes that require your attention are listed below:

- **Graph** replaces **Creator**: After `IResourceGraph` was introduced we declared the `IResourceCreator` obsolote and finally removed it. The `Instantiate` and `Destroy` APIs have moved and you simply need to replace all referancs to the creator with the graph. This also applies to implementations of `IResourceInitializer`.
- **DefaultTracing** is replaced by **Tracing**: The abstract base class `Tracing` was the reason for different inheritances branches and caused data-loss when converting/upgrading tracing information. We made `Tracing` non-abstract to replace `DefaultTracing` and allow for a straight inheritance structure.
- **Required References**: Resources can declare references as `Required`. The validation makes `null` checks redundant **and** reduces configuration errors.
- **Aspect UIs** replace custom details views: Inspired by a design discussion several years ago we introduce the resource and product UI aspects. Simply put, they are tabs within the details view of a certain resource or product. They offer the possibility to offer a user friendly UI for some properties of a certain type without to replace the other generic controls and UIs. They can be freely combined and reused across different types.


## Product Changes

- Introduction of the **ProductQuery**: While refactoring the product client we noticed different use cases and APIs to retrieve products, throught the service and the facade. Some filter rules were hard coded, others could be customized by plugins. There were APIs to load all the top revision for each product, all revisions for a product, all possible parts for a part link, etc. We decided to replace all of this APIs with a single `GetProducts` that accepts a [`ProductQuery`](xref:Marvin.AbstractionLayer.ProductQuery). The query allows to filter for revision, type, recipe existence and many more. It also allows to specify wether to load the query results or its parents/part links. We hope to cover as many uses cases as possible with this approach and if we forgot one, we can simply extend the `ProductQuery` without having to release a new major version.
- **ProductState** was removed: We identified, that the previous requirement of managing product states within MARVIN did not really exist. No customer manages his portfolio in MARVIN. The MARVIN AL is not an ERP system, nor a PLM system - managing states and lifecycle within MARVIN has no use.
- Revision comment was removed: As explained above customers do not manage their products and revision in MARVIN. The user creating a new revision does not make the decision to do so, he simply enters the new information. Commenting that process is useless.
- Splitting `IWorkplanRecipe` and `IProductRecipe`: We decided that recipes not necessarily need a workplan. Therefore we split the inheritance and placed them besides one another. The product management now operates on `IProductRecipe` as a baseline and additional takes care of workplans if `IWorkplanRecipe` is also implemented. As a replacement the ControlSyste will offer a `IProductionRecipe` that combines both interface.
- **DuplicateProduct** bringes cloning and replaces revisions: We indentified that the long standing request of cloning and creating a revision were in fact the same thing. Duplicating a product, its part links and recipes.
- `ICustomization` was removed: Most people never really understood the point of the component in the first place and the few use cases it had, could be replaced with reflection and config-options.
- **Aspect UIs** replace custom details views: Inspired by a design discussion several years ago we introduce the resource and product UI aspects. Simply put, they are tabs within the details view of a certain resource or product. They offer the possibility to offer a user friendly UI for some properties of a certain type without to replace the other generic controls and UIs. They can be freely combined and reused across different types.
