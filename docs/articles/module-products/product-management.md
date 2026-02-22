---
uid: ProductsManagement
---
# ProductManagement

## Description

The [ProductManagement](/src/Moryx.Products.Management/) is a server module providing access to [product descriptions and instance data](index.md).

## Provided facades

ProductManager's public API is provided by the following facades:

* [IProductManagement](/src/Moryx.AbstractionLayer/Products/IProductManagement.cs) 

## Dependencies

## Referenced facades

None. The product management does not depend on any other server module.

## Used DataModels

* [Moryx.Products.Model](/src/Moryx.Products.Model/) This data model is used to store product data as well as instance data. The product data describes how to produce an product instance and represents the manufacturing master data while the instance data contains tracing data about every produced instance which is the dynamic data of the product management module.

# Architecture
The ProductManagement is the central component to manage product types and their instances. Each application can [define custom classes](product-definition.md) to best
meet their requirements. Each application also defines a set of plugins to adapt the product management to their needs.

## Overview

Component name|Implementation|Desription
--------------|--------------|----------
[IProductManager](/src/Moryx.Products.Management/Components/IProductManager.cs)|internal|The API of the ProductManager
[IProductStorage](/src/Moryx.Products.Management/Components/IProductStorage.cs)|external|The plant specific product storage
[IProductImporter](/src/Moryx.AbstractionLayer/Products/Import/IProductImporter.cs)|internal/external|Plugins that can import products from file
[IRecipeManagement](/src/Moryx.Products.Management/Components/IRecipeManagement.cs)|internal|Component to handle all recipe operations

## Diagrams

TODO
