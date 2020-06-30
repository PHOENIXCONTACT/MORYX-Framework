---
uid: AbstractionLayer
---
# The AbstractionLayer

## Description

In a industrial production environment and not only there, there are three parts that come to play to describe the production process:

* Product - A product is something the user wants to produce
* Resource - A resource describes a part that is needed to produce a product
* Processing - The way how the product is produced with a set of resources

The AbstractionLayer provides a collection of interfaces, base classes and common implementations for use in such a typical industrial production environment.
So the AbstractionLayer is the base for Resources (e.g. Drivers, Cells, Stations), product descriptions and process data. It also gives you a base to persist these domain objects to a database.

This article gives you an overview which base classes, storage techniques, process and product description come into play. The following list gives an overview of the needed aspects of the AbstractionLayer:

## Activities

An [Activity](xref:Activities) is the smallest separately executable step of a Process.

## Articles

An [Article](xref:Moryx.AbstractionLayer.Article) is the produced instance of a Product.

## Capabilities

[Capabilities](xref:Capabilities) are used to find a matching *Resource* for an Activity.

## Constraints

[Constraints](xref:Constraints) may be used when calling a ReadyToWork to be able to restrict the possible activities which could be dispatched to the resource which has called the ReadyToWork.

## Identity

[Identities](xref:Moryx.AbstractionLayer.Identity.IIdentity) are used to represent unique properties like serial numbers and MAC adresses for instances and material number for products. There is also the derived type [ProductIdentity](xref:Moryx.AbstractionLayer.ProductIdentity) that represents a products material number and revision. The static constructor `AsLatestRevision` also lets you refer to the latest revision for a certain material.

## Process

A [Process](xref:Processes) consists of a series of activities.

## Products

A [Product](xref:Moryx.AbstractionLayer.IProduct) or better a *product description* is used by a [ProductRecipes](xref:Moryx.AbstractionLayer.ProductRecipe) to provide a basic structure to produce an instance.

## Recipes

A [Recipe](xref:Moryx.AbstractionLayer.Recipe) is the base for all recipes which combines all needed data for a process.

A [ProductRecipe](xref:Moryx.AbstractionLayer.ProductRecipe) provides a basic structure to use a product for production cases.

A [WorkplanRecipe](xref:Moryx.AbstractionLayer.WorkplanRecipe) provides a Workplan and a set of parameters to define all the [Activities](xref:Activities) needed for a Process.

## Resources

The Resources package of the AbstrationLayer contains several basic classes to be used by the [Resources](xref:Moryx.AbstractionLayer.Resources.IResource) and the [ResourceManager](xref:ResourceManagement)

## Tasks

[Tasks](xref:Tasks) are used within a Workplan to define the Activity for each step.

## Workplan

The [Workplan](xref:Workplans) package of the AbstrationLayer contains several helper classes to be used by the Workflow package.

## Tutorials

You can find some tutorials here:

* [How to create a Resource](xref:HowToCreateAResource)
* [How to build a Driver](xref:HowToBuildADriver)
* [How to use Constraints](xref:HowToUseConstraints)
