---
uid: AbstractionLayer
---
# The AbstractionLayer

## Description

In an industrial production environment and not only there, there are three parts that come to play to describe the production process:

* [Product](Products/Concept.md) - A product is something the user wants to produce
* [Resource](Resources/Overview.md) - A resource describes a part that is needed to produce a product
* Processing - The way how the product is produced with a set of resources

The AbstractionLayer provides a collection of interfaces, base classes and common implementations for use in such a typical industrial production environment.
So the AbstractionLayer is the base for Resources (e.g. Drivers, Cells, Stations), product descriptions and process data. It also gives you a base to persist these domain objects to a database.

This article gives you an overview which base classes, storage techniques, processes and product descriptions come into play. The following list gives an overview of the needed aspects of the AbstractionLayer:

## Activities

An [Activity](Processing/Activities.md) is the smallest separately executable step of a Process.

## Capabilities

[Capabilities](Processing/Capabilities.md) are used to find a matching *Resource* for an Activity.

## Constraints

[Constraints](Processing/Constraints.md) have a similar impact on the mapping between activities and resources. 
In contrast to the capabilities, constraints aren't sets of provided and required abilities, but boolean conditions that might restrict the possibility to execute an activity on a resource.
The constraint is also not based on an inherent property of the activity or resource but arises from the current context in the system.

## Identity

[Identities](../../src/Moryx.AbstractionLayer/Identity/IIdentity.cs) are used to represent unique properties like serial numbers and MAC adresses for instances and material number for products. There is also the derived type [ProductIdentity](../../src/Moryx.AbstractionLayer/Products/ProductIdentity.cs) that represents a products material number and revision. The static constructor `AsLatestRevision` also lets you refer to the latest revision for a certain material.

## Process

A [Process](Processing/Processes.md) consists of a series of activities.

## Products

A [ProductType](../../src/Moryx.AbstractionLayer/Products/IProductType.cs) or better a *product description* is used by a [ProductRecipe](../../src/Moryx.AbstractionLayer/Recipes/ProductRecipe.cs) to provide a basic structure to produce a [ProductInstance](../../src/Moryx.AbstractionLayer/Products/ProductInstance.cs) 

## Recipes

A [Recipe](../../src/Moryx.AbstractionLayer/Recipes/Recipe.cs) is the base for all recipes which combines all needed data for a process.

A [ProductRecipe](../../src/Moryx.AbstractionLayer/Recipes/ProductRecipe.cs) provides a basic structure to use a product for production cases.

A [WorkplanRecipe](../../src/Moryx.AbstractionLayer/Recipes/WorkplanRecipe.cs) provides a Workplan and a set of parameters to define all the [Activities](Processing/Activities.md) needed for a Process.

A [ProductionRecipe](../../src/Moryx.AbstractionLayer/Recipes/ProductionRecipe.cs) is the combination of a `ProductRecipe` and `WorkplanRecipe`.

## Resources

The Resources package of the AbstrationLayer contains several basic classes to be used by the [Resources](../../src/Moryx.AbstractionLayer/Resources/IResource.cs) and the [ResourceManager](Resources/ResourceManagement.md)

## Tasks

[Tasks](Processing/Tasks.md) are used within a Workplan to define the Activity for each step.

## Workplan

The [Workplan](Processing/Workplans.md) package of the AbstrationLayer contains several helper classes to be used by the Workflow package.

## Tutorials

To start using the AbstractionLayer for your own projects you can jump into these tutorials:

* [How to create a Resource](/docs/tutorials/HowToCreateResource.md)
* [How to build a Driver](/docs/tutorials/HowToBuildADriver.md)