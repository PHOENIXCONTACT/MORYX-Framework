<p align="center">
    <img src="docs/Resources/MORYX_logo.svg" alt="MORYX Logo" width="300px" />
</p>

<p align="center">
    <a href="https://github.com/PHOENIXCONTACT/MORYX-AbstractionLayer/workflows">
        <img src="https://github.com/PHOENIXCONTACT/MORYX-AbstractionLayer/workflows/CI/badge.svg" alt="CI">
    </a>
    <a href="https://codecov.io/gh/PHOENIXCONTACT/MORYX-AbstractionLayer/branch/dev">
        <img alt="Coverage" src="https://codecov.io/gh/PHOENIXCONTACT/MORYX-AbstractionLayer/coverage.svg?branch=dev" />
    </a>
    <a href="https://github.com/PHOENIXCONTACT/MORYX-AbstractionLayer/blob/dev/LICENSE">
        <img src="https://img.shields.io/github/license/PHOENIXCONTACT/MORYX-AbstractionLayer" alt="License">
    </a>
    <a href="https://gitter.im/PHOENIXCONTACT/MORYX?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge">
        <img src="https://badges.gitter.im/PHOENIXCONTACT/MORYX.svg" alt="Gitter">
    </a>
</p>

# MORYX AbstractionLayer

The **MORYX AbstractionLayer** is the environment for the digital twins of resources and products. It defines the domain independent [meta model](/docs/articles/AbstractionLayer.md) and enables applications to model their physical system and product portfolio as typed objects. It thereby makes other modules hardware independent by encapsulating details of the underlying structure and devices. [Like the platform](https://github.com/PHOENIXCONTACT/MORYX-Platform#history) version 5.0 of the AbstractionLayer is focused on the open source community and we are still applying the final touches, while the in-house stable version powers a range of different applications.

**Links**
- [Package Feed](https://www.myget.org/feed/Packages/moryx)
- [Repository Template](https://github.com/PHOENIXCONTACT/MORYX-Template)
- [MORYX Platform](https://github.com/PHOENIXCONTACT/MORYX-Platform)
- [MORYX Maintenance](https://github.com/PHOENIXCONTACT/MORYX-MaintenanceWeb)
- [MORYX ClientFramework](https://github.com/PHOENIXCONTACT/MORYX-ClientFramework)

## Getting Started

If you want to start developing with or for MORYX, the easiest way is our [template repository](https://github.com/PHOENIXCONTACT/MORYX-Template). It comes with two empty solutions, the necessary package feeds and preinstalled empty MORYX runtime. Add projects and packages to backend and frontend solutions depending on your specific requirements. Install stable releases via Nuget; development releases are available via MyGet.

| Package Name | Release (NuGet) | CI (MyGet) |
|--------------|-----------------|------------|
| `Moryx.AbstractionLayer` | [![NuGet](https://img.shields.io/nuget/v/Moryx.AbstractionLayer.svg)](https://www.nuget.org/packages/Moryx.AbstractionLayer/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.AbstractionLayer)](https://www.myget.org/feed/moryx/package/nuget/Moryx.AbstractionLayer) |
| `Moryx.AbstractionLayer.TestTools` | [![NuGet](https://img.shields.io/nuget/v/Moryx.AbstractionLayer.TestTools.svg)](https://www.nuget.org/packages/Moryx.AbstractionLayer.TestTools/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.AbstractionLayer.TestTools)](https://www.myget.org/feed/moryx/package/nuget/Moryx.AbstractionLayer.TestTools) |
| `Moryx.Notifications` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Notifications.svg)](https://www.nuget.org/packages/Moryx.Notifications/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Notifications)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Notifications) |
| `Moryx.Products.Management` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Products.Management.svg)](https://www.nuget.org/packages/Moryx.Products.Management/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Products.Management)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Products.Management) |
| `Moryx.Products.Model` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Products.Model.svg)](https://www.nuget.org/packages/Moryx.Products.Model/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Products.Model)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Products.Model) |
| `Moryx.Resources.Interaction` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Resources.Interaction.svg)](https://www.nuget.org/packages/Moryx.Resources.Interaction/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Resources.Interaction)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Resources.Interaction) |
| `Moryx.Resources.Management` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Resources.Management.svg)](https://www.nuget.org/packages/Moryx.Resources.Management/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Resources.Management)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Resources.Management) |
| `Moryx.AbstractionLayer.UI` | [![NuGet](https://img.shields.io/nuget/v/Moryx.AbstractionLayer.UI.svg)](https://www.nuget.org/packages/Moryx.AbstractionLayer.UI/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.AbstractionLayer.UI)](https://www.myget.org/feed/moryx/package/nuget/Moryx.AbstractionLayer.UI) |
| `Moryx.Products.UI` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Products.UI.svg)](https://www.nuget.org/packages/Moryx.Products.UI/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Products.UI)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Products.UI) |
| `Moryx.Products.UI.Interaction` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Products.UI.Interaction.svg)](https://www.nuget.org/packages/Moryx.Products.UI.Interaction/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Products.UI.Interaction)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Products.UI.Interaction) |
| `Moryx.Resources.UI` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Resources.UI.svg)](https://www.nuget.org/packages/Moryx.Resources.UI/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Resources.UI)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Resources.UI) |
| `Moryx.Resources.UI.Interaction` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Resources.UI.Interaction.svg)](https://www.nuget.org/packages/Moryx.Resources.UI.Interaction/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Resources.UI.Interaction)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Resources.UI.Interaction) |

Whether you want to debug and experiment with this repository or build an application based on the Abstraction Layers packages you need to follow a few simple steps to setup each of the modules. For both modules this requires the package *Moryx.Runtime.Maintenance.Web* and its [database configuration](http://localhost/maintenanceweb/#/databases).

**Product Management**:
1. Create or configure the database for *Moryx.Products.Model* using the Maintenance
2. Configure the [storage mapping](/docs/articles/Products/ProductStorage.md) for your domain objects. This is necessary to store, load and use the objects within MORYX.

**Resource Management**: 
1. Create or configure the database for *Moryx.Resources.Model*
2. Execute the `ResourceInteractionInitializer` from [ResourceManager console](http://localhost/maintenanceweb/#/modules/ResourceManager/console) to provide the endpoint for the resource configuration UI.

## Domain Meta Model

The Abstraction Layer defines the [domain independent model](/docs/articles/AbstractionLayer.md) of assets, products, processes and many more. It is the foundation for compatible models of systems and hardware independence of other modules.

## Resource Management

The [Resource Management](/docs/articles/Resources/ResourceManagement.md) holds the object graph representing the physical system. It maintains the database entities for the object graph in the background and reconstructs the object graph upon boot. It also provides the API for other modules to interact with resources based on their implemented interfaces and supports resource look-up by various conditions or capabilities.

## Product Management

The [Product Management](/docs/articles/Products/ProductManagement.md) holds all product variants, created instances of those variants and recipes how to create an instance. It provides an API giving access to product types, instances, recipes and workplans.

## Tutorials

To start using the AbstractionLayer for your own projects you can jump into these tutorials:

* [How to create a Resource](/docs/articles/Tutorials/HowToCreateResource.md)
* [How to build a Driver](/docs/articles/Tutorials/HowToBuildADriver.md)
