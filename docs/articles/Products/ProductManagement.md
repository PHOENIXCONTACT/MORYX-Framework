---
uid: ProductsManagement
---
# ProductManagement

## Description

The [ProductManagement](xref:Moryx.Products.Management) is a server module providing access to [product descriptions and instance data](xref:ProductsConcept).

## Provided facades

ProductManager's public API is provided by the following facades:

* [IProductManagement](xref:Moryx.AbstractionLayer.Products.IProductManagement) 

## Dependencies

## Referenced facades

None. The product management does not depend on any other server module.

## Used DataModels

* [Moryx.Products.Model](xref:Moryx.Products.Model) This data model is used to store product data as well as instance data. The product data describes how to produce an product instance and represents the manufacturing master data while the instance data contains tracing data about every produced instance which is the dynamic data of the product management module.

# Architecture
The ProductManagement is the central component to manage product types and their instances. Each application can [define custom classes](xref:ProductsDefinition) to best
meet their requirements. Each application also defines a set of plugins to adapt the product management to their needs.

## Overview

Component name|Implementation|Desription
--------------|--------------|----------
[IProductManager](xref:Moryx.Products.Management.IProductManager)|internal|The API of the ProductManager
[IProductStorage](xref:Moryx.Products.Management.IProductStorage)|external|The plant specific product storage
[IProductImporter](xref:Moryx.AbstractionLayer.Products.IProductImporter)|internal/external|Plugins that can import products from file
[IRecipeManagement](xref:Moryx.Products.Management.IRecipeManagement)|internal|Component to handle all recipe operations

## Diagrams

TODO
