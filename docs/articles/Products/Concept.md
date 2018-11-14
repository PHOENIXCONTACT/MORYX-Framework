---
uid: ProductsConcept
---
# Products

## Products, Articles and the Stuff Involved in the Production Process

The abstraction layer provides just basic interfaces and classes to handle `products` and `articles`.
They are used in derived projects only.

## Architecture

The diagram shows a simplified class diagram of Products and Articles and the stuff around it.

<!--
ToDo: New Image?
 ![](images\ProcessAndActivity.png "EA:MARVIN.MARVIN 2.0.ControlSystem.Level-2.Implementation.ReverseEngineering.ProcessAndActivity") -->

Their collaboration is decribed in the following sections.

## Product

In the context of MaRVIN, the product is mainly the technical description and the rule how to produce an `article`. The rule itself is provided by a `recipe`. A product inside MaRVIN is represented by an object implementing the [IProduct](xref:Marvin.AbstractionLayer.IProduct) interface.

## Article

An article is the produced instance of a `product`. The article data itself is created by [IProduct's](xref:Marvin.AbstractionLayer.IProduct) [CreateInstance()](xref:Marvin.AbstractionLayer.IProduct.CreateInstance) method. An article inside MaRVIN is represented by an object derived from [Article](xref:Marvin.AbstractionLayer.Article).

## Recipe

A recipe is used inside the MaRVIN ControlSystem by the ProcessController to provide a workplan for a `process` and to provide additional parameters related to the `workplan`. All recipes a derived from [Recipe](xref:Marvin.AbstractionLayer.Recipe).

There are three types of recipes:

- [ProductRecipes](xref:Marvin.AbstractionLayer.ProductRecipe) are used to control a production `process`. They belong to a `product` and are used to link the product description to the corresponding workplan. The provided parameter is the product to be produced. The product data should contain the data needed by  production process, eg. the material needed, technical parameters for automatic testing, etc.
- [MaintenanceRecipes](xref:Marvin.AbstractionLayer.MaintenanceRecipe) are used to control a maintenance `process`. The provided parameters are specific for the process.

## Workplan

The workplan defines the steps needed to run a `process`. For each step it defines the successors to be used depending on the result of the step. Each step is used by the ProcessController to create an `activity`. Workplan are represented inside MaRVIN by objects implementing `IWorkplan` and usually just `Workplan` is used.

## Process

A process is a sequence of `activities` defined by a `workplan` and parameterized by a `recipe`. All activities created for the process are stored with the process for tracing purposes. All objects represesting a process implement the [IProcess](xref:Marvin.AbstractionLayer.IProcess) interface.

TODO:

There are three types of processes:

- A [ProductionProcess](xref:Marvin.AbstractionLayer.ProductionProcess) refers the `article` it produces. Every process refers exactly one article. The article may be removed from the facility and reworked somewhere else. If it is inserted later, a new ProductionProcess is created. The article refers all processes involved in its production.

- A [MaintenanceProcess](xref:Marvin.AbstractionLayer.MaintenanceProcess) is used to execute a maintenance job. A maintenance job is created on demand by the ControlSystem's MaintenanceController when some business rule defines that some maintenance is needed quite now.

- A [SetupProcess](xref:Marvin.AbstractionLayer.SetupProcess) is used to change the facility's setup. The process will be used to execute setup jobs. The setup jobs will be created on demand by a SetupController inside the ControlSystem. Currently there is no implementation of a SetupController. The SetupController will comapre the needed setup for a job with the needed setup of the previous job and insert a SetupJob between them to change it as needed.

## Activity

Activities are created by the ControlSystem's ProcessController. Each activity has a set of required `capabilities` while the production resources each have a set of provided capabilities. For each activity the ProcessController searches for a resource with matching capabilities. If there is more than one matching, an optimizer may select there resource fitting best. If there is no optimizer the first one found will be used. If the selected resource is [ReadyToWork](xref:Marvin.AbstractionLayer.Resources.ReadyToWork), the ProcessController sends the activity with a [StartActivity](xref:Marvin.AbstractionLayer.Resources.StartActivity) message to the resource to be executed there. All activities are stored permanently with the process for tracing purposes. An activity inside MaRVIN is represented by an obejct implementing [IActivity](xref:Marvin.AbstractionLayer.IActivity) and is usually derived from [Activity](xref:Marvin.AbstractionLayer.Activity).

## Capability

Capabilities are used to match `activities` and resources. Capabilities are represented by objects implementing the [ICapabilities](xref:Marvin.AbstractionLayer.Capabilities.ICapabilities) interface.

## UI

### Concept

The products ui is structured as a tree navigation. On the left side, the user can see the complete product structure. If the user click on a tree item, a product type specific details view will be opened. If no type specific details view was found, a default implementation will be used. The UI is using the concept of [Type specific detail views](xref:MasterDetailUIs).

### Functions of the base class

The [ProductDetailsViewModelBase](xref:Marvin.Products.UI.Interaction.ProductDetailsViewModelBase) is responsible to load the basic product data. Every known data will be put in protected properites: `Recipes[]`, `CurrentRecipe` and `ProductProperties` as an array of `Entry[]`. A deeper description can be found in the class definition.

### Add specialized detail views

The products ui can be extended with specialized views for specialized product types. The view will be selected with the product structure type.

Create a view model and inherit the [ProductDetailsViewModelBase](xref:Marvin.Products.UI.Interaction.ProductDetailsViewModelBase) and implement the needed methods. To register this new view model, add the [ResourceDetailsRegistrationAttribute](xref:Marvin.Resources.UI.ResourceDetailsRegistrationAttribute) on top of the class.

The [ProductDetailsRegistration](xref:Marvin.Products.UI.ProductDetailsRegistrationAttribute) need a parameter *typeName*. It will define the product type for which the view model should be used for. Additionally add a user control which have the same name as the view model (without the model at the end) to fit the naming convension: TestChildProductView

````cs
[ProductDetailsRegistration("TestChildProduct")]
internal class TestChildProductViewModel : ProductDetailsViewModelBase
{

}
````