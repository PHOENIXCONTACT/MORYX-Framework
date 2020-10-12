<p align="center">
    <img src="docs/Resources/MORYX_logo.svg" alt="MORYX Logo" width="300px" />
</p>

<p align="center">
    <a href="https://github.com/PHOENIXCONTACT/MORYX-AbstractionLayer/workflows">
        <img src="https://github.com/PHOENIXCONTACT/MORYX-AbstractionLayer/workflows/CI/badge.svg" alt="CI">
    </a>
    <a href="https://www.myget.org/feed/Packages/moryx">
        <img src="https://img.shields.io/myget/moryx/v/Moryx.AbstractionLayer" alt="MyGet">
    </a>
    <a href="https://codecov.io/gh/PHOENIXCONTACT/MORYX-AbstractionLyer/coverage.svg?branch=dev">
        <img alt="Coverage" src="https://codecov.io/gh/PHOENIXCONTACT/MORYX-AbstractionLyer/coverage.svg?branch=dev" />
    </a>
    <a href="https://github.com/PHOENIXCONTACT/MORYX-AbstractionLayer/blob/dev/LICENSE">
        <img src="https://img.shields.io/github/license/PHOENIXCONTACT/MORYX-AbstractionLayer" alt="License">
    </a>
    <a href="https://gitter.im/MORYX-Industry/Framework?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge">
        <img src="https://badges.gitter.im/MORYX-Industry/Framework.svg" alt="Gitter">
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
