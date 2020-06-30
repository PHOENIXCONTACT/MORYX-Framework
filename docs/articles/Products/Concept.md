---
uid: ProductsConcept
---
# Products

The abstraction layer provides just basic interfaces and classes to handle `products` and `product instances`.
They are used in derived projects only. The following picture shows the stucture how a product is placed in an application and which components will use it. In the following sections are short descriptions about each part of the product concept.

![Product concept structure](http://www.plantuml.com/plantuml/proxy?cache=no&fmt=svg&src=https://raw.githubusercontent.com/PHOENIXCONTACT/MORYX-AbstractionLayer/dev/docs/articles/Products/images/product_concept)

## Product

In the context of MORYX, the product is mainly the technical description and the rule how to produce an `product instance`. The rule itself is provided by a `recipe`. A product inside MORYX is represented by an object implementing the [IProduct](xref:Moryx.AbstractionLayer.IProduct) interface.

### Recipe

A recipe is used inside the MORYX ControlSystem by the ProcessController to provide a workplan for a `process` and to provide additional parameters related to the `workplan`. All recipes a derived from [Recipe](xref:Moryx.AbstractionLayer.Recipe).

The Abstraction Layer provides the [IProductRecipe](xref:Moryx.AbstractionLayer.IProductRecipe) and [IWorkplanRecipe](xref:Moryx.AbstractionLayer.IWorkplanRecipe) which can be used separatly or combined like in the MORYX ControlSystem to control a production `process` with the belonging `product` and the corresponding workplan. The provided parameter is the product to be produced. The product data should contain the data needed by production process, eg. the material needed, technical parameters for automatic testing, etc.

### Workplan

The workplan defines the steps needed to run a `process`. For each step it defines the successors to be used depending on the result of the step. Each step is used by the ProcessController to create an `activity`. Workplan are represented inside MORYX by objects implementing `IWorkplan` and usually just `Workplan` is used.

### Process

A process is a sequence of `activities` defined by a `workplan` and parameterized by a `recipe`. All activities created for the process are stored with the process for tracing purposes. All objects represesting a process implement the [IProcess](xref:Moryx.AbstractionLayer.IProcess) interface.

A [ProductionProcess](xref:Moryx.AbstractionLayer.ProductionProcess) refers the `product instance` it produces. Every process refers exactly one product instance. The product instance may be removed from the facility and reworked somewhere else. If it is inserted later, a new ProductionProcess is created. The instance refers all processes involved in its production.

## Product Instance

An instance is the produced instance of a `product`. The product instance data itself is created by [IProduct's](xref:Moryx.AbstractionLayer.IProduct) [CreateInstance()](xref:Moryx.AbstractionLayer.IProduct.CreateInstance) method. An product instance inside MORYX is represented by an object derived from [ProductInstance](xref:Moryx.AbstractionLayer.ProductInstance).
