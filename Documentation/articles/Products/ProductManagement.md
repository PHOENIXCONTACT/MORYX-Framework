ProductManagement {#architecture-ProductManagement}
======================================
[TOC]

# Description # {#ProductManagement-description}

The [ProductManagement](xref:Marvin.Products.Management) is a server module providing access 
to [product descriptions and article data](xref:concepts-Products).

# Provided facades # {#ProductManagement-provided}

ProductManager's public API is provided by the following facades:
 
* [IProductManagement](xref:Marvin.Products.IProductManagement) 

# Dependencies # {#ProductManagement-dependencies}

## Referenced facades ## {#ProductManagement-references}

None. The product management does not depend on any other server module.

## Used DataModels ## {#ProductManagement-models}

- [Marvin.Products.Model](xref:Marvin.Products.Model) This data model is used to store product 
data as well as article data. The product data describes how to produce an article and represents the 
manufacturing master data while the article data contains tracing data about every produced article 
which is the dynamic data of the product management module.

# Architecture # {#ProductManagement-architecture}
The ProductManagement is the central component to manage product types and their instances. Each application can [define custom classes](xref:productDefinition) to best
meet their requirements. Each application also defines a set of plugins to adapt the product management to their needs.

## Overview ## {#ProductManagement-architecture-overview}

Component name|Implementation|Desription
--------------|--------------|----------
[IProductManager](xref:Marvin.Products.Management.IProductManager)|intern|The API of the ProductManager
[IProductStorage](xref:Marvin.Products.IProductStorage)|extern|The plant specific product storage
[IProductInteraction](xref:Marvin.Products.Management.Modification.IProductInteraction)|intern|Defines teAPI of the product WCF service.
[IProductConverter](xref:Marvin.Products.Management.Modification.IProductConverter)|intern| *TBD*
[ICustomization](xref:Marvin.Products.ICustomization)|extern|Plugin to fetch possible values from storage
[IProductImporter](xref:Marvin.Products.IProductImporter)|intern/extern|Plugins that can import products from file
[IRecipeManagement](xref:Marvin.Products.Management.IRecipeManagement)|intern|Component to handle all recipe operations
[IWorkplanEditingService](xref:Marvin.Products.Management.Modification.IWorkplanEditingService)|intern|Service used to edit workplan
[IWorkplanEditingHost](xref:Marvin.Products.Management.Modification.IWorkplanEditingHost)|intern|Host for editing workplans

## Diagrams ## {#ProductManagement-architecture-diagrams}

![](images\ProductManagement.png "EA:MARVIN.MARVIN 2.0.ControlSystem.Level-2.Implementation.ReverseEngineering.ProductManagement")

# Configuration # {#ProductManagement-configuration}

