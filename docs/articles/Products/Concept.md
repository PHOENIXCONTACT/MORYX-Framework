---
uid: ProductsConcept
---
# Products

The abstraction layer provides just basic interfaces and classes to handle [ProductTypes](xref:Moryx.AbstractionLayer.Products.IProductType) and [ProductInstances](xref:Moryx.AbstractionLayer.Products.ProductInstance).
They are used in derived projects only. 
The following picture shows the stucture how a product is placed in an application and which components will use it. 
In the following sections are short descriptions about each part of the product concept.

![Product concept structure](http://www.plantuml.com/plantuml/proxy?cache=no&fmt=svg&src=https://raw.githubusercontent.com/PHOENIXCONTACT/MORYX-AbstractionLayer/dev/docs/articles/Products/images/product_concept)

## Product

In the context of MORYX, the `product type` is mainly the instance independent description and contains the rule how to produce a `product instance`. 
The rule itself is provided by a [Recipe](xref:Moryx.AbstractionLayer.Recipe). 
For more information regarding the definition of a product including a detailed example see [this article](ProductDefinition.md).

### Recipe

A recipe is used to provide a [Workplan](../Processing/Workplans.md) for a [Process](../Processing/Processes.md) in which a product is processed. It can also provides additional parameters related to the `workplan`. All recipes are derived from [Recipe](xref:Moryx.AbstractionLayer.Recipe).

The Abstraction Layer provides the [IProductRecipe](xref:Moryx.AbstractionLayer.Recipes.IProductRecipe) and [IWorkplanRecipe](xref:Moryx.AbstractionLayer.Recipes.IWorkplanRecipe) which can be used separatly or combined. 
In a production environment, for example, we have the `process` with the belonging `product` and the corresponding `workplan`. 
The parameters regarding the products production (e.g. the material needed, technical parameters for automatic testing, etc) are provided with the `product recipe` while the `workplan` and parameters to configure it come with the `workplan recipe`.

### Workplan

The workplan defines the steps needed to run a `process`. 
For each step it defines the successors to be used depending on the result of the step. 
Each step basically represents an [Activity](../Processing/Activities.md). 
Workplans are represented inside MORYX by objects implementing [IWorkplan](xref:Moryx.Workplans.IWorkplan) and usually just [Workplan](xref:Moryx.Workplans.Workplan) is used.

### Process

A process is a sequence of `activities` defined by a `workplan` and parameterized by a `recipe`. All activities created for the process are stored with the process for tracing purposes. All objects represesting a process implement the [IProcess](xref:Moryx.AbstractionLayer.IProcess) interface.

A [ProductionProcess](xref:Moryx.AbstractionLayer.ProductionProcess) references the `product instance` it produces. 
Every process holds the reference to one product instance. 
The product instance may be removed from the facility and reworked somewhere else. 
If it is inserted again later on, a new ProductionProcess is created. The `product instance` references all processes involved in its production.

## Further Readings regarding Products

There are additional contents available to take a closer look into 
 * the [storage of products](ProductStorage.md) and [recipes](RecipeStorage.md)
 * the [product management](ProductManagement.md) as well as the available [UI on the client side](ProductManagementUI.md)
 * the posibility to [import products](ProductImport.md)