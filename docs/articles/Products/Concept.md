---
uid: ProductsConcept
---
# Products

The abstraction layer provides just basic interfaces and classes to handle `products` and `articles`.
They are used in derived projects only. The following picture shows the stucture how a product is placed in an application and which components will use it. In the following sections are short descriptions about each part of the product concept.

![Product Concept Structure](images\ProductConcept.png)

## Product

In the context of MARVIN, the product is mainly the technical description and the rule how to produce an `article`. The rule itself is provided by a `recipe`. A product inside MARVIN is represented by an object implementing the [IProduct](xref:Marvin.AbstractionLayer.IProduct) interface.

### Recipe

A recipe is used inside the MARVIN ControlSystem by the ProcessController to provide a workplan for a `process` and to provide additional parameters related to the `workplan`. All recipes a derived from [Recipe](xref:Marvin.AbstractionLayer.Recipe).

The Abstraction Layer provides the [IProductRecipe](xref:Marvin.AbstractionLayer.IProductRecipe) and [IWorkplanRecipe](xref:Marvin.AbstractionLayer.IWorkplanRecipe) which can be used separatly or combined like in the MARVIN ControlSystem to control a production `process` with the belonging `product` and the corresponding workplan. The provided parameter is the product to be produced. The product data should contain the data needed by production process, eg. the material needed, technical parameters for automatic testing, etc.

### Workplan

The workplan defines the steps needed to run a `process`. For each step it defines the successors to be used depending on the result of the step. Each step is used by the ProcessController to create an `activity`. Workplan are represented inside MARVIN by objects implementing `IWorkplan` and usually just `Workplan` is used.

### Process

A process is a sequence of `activities` defined by a `workplan` and parameterized by a `recipe`. All activities created for the process are stored with the process for tracing purposes. All objects represesting a process implement the [IProcess](xref:Marvin.AbstractionLayer.IProcess) interface.

A [ProductionProcess](xref:Marvin.AbstractionLayer.ProductionProcess) refers the `article` it produces. Every process refers exactly one article. The article may be removed from the facility and reworked somewhere else. If it is inserted later, a new ProductionProcess is created. The article refers all processes involved in its production.

## Article

An article is the produced instance of a `product`. The article data itself is created by [IProduct's](xref:Marvin.AbstractionLayer.IProduct) [CreateInstance()](xref:Marvin.AbstractionLayer.IProduct.CreateInstance) method. An article inside MARVIN is represented by an object derived from [Article](xref:Marvin.AbstractionLayer.Article).
