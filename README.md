<p align="center">
    <a href="https://github.com/PHOENIXCONTACT/MORYX-Framework/tree/future">
        <img src="https://img.shields.io/badge/MORYX%2010-Fully%20Open%20Source-0098A1?style=for-the-badge" alt="MORYX 10 Fully Open Source" />
    </a>
</p>
<p align="center">
    <strong>
        🚀 Switch to the <a href="https://github.com/PHOENIXCONTACT/MORYX-Core/tree/future">MORYX 10</a> branch for the fully open sourced release!
    </strong>
</p>

# 

<p align="center">
    <img src="docs/resources/MORYX_logo.svg" alt="MORYX Logo" width="300px" />
</p>

<p align="center">
    <a href="https://github.com/PHOENIXCONTACT/MORYX-Core/workflows">
        <img src="https://github.com/PHOENIXCONTACT/MORYX-Core/workflows/CI/badge.svg" alt="CI">
    </a>
    <a href="https://codecov.io/gh/PHOENIXCONTACT/MORYX-Core/coverage.svg?branch=dev">
        <img alt="Coverage" src="https://codecov.io/gh/PHOENIXCONTACT/MORYX-Core/coverage.svg?branch=dev" />
    </a>
    <a href="https://github.com/PHOENIXCONTACT/MORYX-Core/blob/dev/LICENSE">
        <img src="https://img.shields.io/github/license/PHOENIXCONTACT/MORYX-Core" alt="License">
    </a>
    <a href="https://gitter.im/PHOENIXCONTACT/MORYX?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge">
        <img src="https://badges.gitter.im/PHOENIXCONTACT/MORYX.svg" alt="Gitter">
    </a>
</p>

<p align="center">
    <a href="https://www.nuget.org/packages/Moryx/">
        <img alt="NuGet Release" src="https://img.shields.io/nuget/v/Moryx?color=0098A1">
    </a>
</p>

# MORYX Framework

The MORYX Framework is a .NET based framework to quickly build three-tier applications. It aims to reduce boilerplate code as much as possible and provides modularity, flexibility and easy configuration with very little effort.
It originates from the original MORYX project targeted to develop machines but has expanded to a much bigger field of use. 

The **MORYX Core** defines a base namespace and set of interfaces used to develop modular applications.

The **MORYX AbstractionLayer** is the environment for the digital twins of resources and products. It defines the domain independent [meta model](/docs/articles/AbstractionLayer.md) and enables applications to model their physical system and product portfolio as typed objects. It thereby makes other modules hardware independent by encapsulating details of the underlying structure and devices.

The **MORYX Factory** contains the APIs, domain objects and developer documentation for the MORYX factory scope.

**Links**

- [Package Feed](https://www.myget.org/feed/Packages/moryx)
- [Repository Template](https://github.com/PHOENIXCONTACT/MORYX-Template)
- [MORYX Factory](https://github.com/PHOENIXCONTACT/MORYX-Factory)

## Getting Started

If you want to start developing with or for MORYX, the easiest way is our [template repository](https://github.com/PHOENIXCONTACT/MORYX-Template). It comes with two empty solutions, the necessary package feeds and preinstalled empty MORYX runtime. Add projects and packages to backend and frontend solutions depending on your specific requirements. Install stable releases via Nuget; development releases are available via MyGet.

| Package Name |  |
|--------------|--|
| `Moryx` |  |
| `Moryx.Model` |  |
| `Moryx.Model.InMemory` |  |
| `Moryx.Model.PostgreSQL` |  |
| `Moryx.Model.Sqlite` |  |
| `Moryx.Container` |  |
| `Moryx.Communication.Serial` |  |
| `Moryx.Asp.Extensions` |  |
| `Moryx.Runtime` |  |
| `Moryx.Runtime.Kernel` |  |
| `Moryx.Runtime.Endpoints` |  |
| `Moryx.CommandCenter.Web` |  |
| `Moryx.AbstractionLayer` |  |
| `Moryx.AbstractionLayer.TestTools` |  |
| `Moryx.TestTools.UnitTest` |  |
| `Moryx.AbstractionLayer.Products.Endpoints` |  |
| `Moryx.AbstractionLayer.Resources.Endpoints` |  |
| `Moryx.Notifications` |  |
| `Moryx.Products.Management` |  |
| `Moryx.Products.Model` |  |
| `Moryx.Resources.Management` |  |

If you wish to contribute to this project, you simply need to clone the repository and open the solution with Visual Studio 2017 or above. The Debug target should be *StartProject*.

To build and run from the command line you need powershell and msbuild.

```powershell
.\Build.ps1 -Build
.\src\StartProject\bin\Debug\StartProject.exe
```

Whether you want to debug and experiment with this repository or build an application based on the Abstraction Layers packages you need to follow a few simple steps to setup each of the modules. For both modules this requires the package *Moryx.Runtime.Maintenance.Web* and its [database configuration](http://localhost/maintenanceweb/#/databases).

**Product Management**:
1. Create or configure the database for *Moryx.Products.Model* using the Maintenance
2. Configure the [storage mapping](/docs/articles/Products/ProductStorage.md) for your domain objects. This is necessary to store, load and use the objects within MORYX.

**Resource Management**:
1. Create or configure the database for *Moryx.Resources.Model*
2. Execute the `ResourceInteractionInitializer` from [ResourceManager console](http://localhost/maintenanceweb/#/modules/ResourceManager/console) to provide the endpoint for the resource configuration UI.

## Architecture
The MORYX Core is a .NET based framework to quickly build three-tier applications. The core architecture is a modular monolith using the service and facade pattern to isolate and decouple functionality. It uses a 2-level Dependency Injection structure to isolate a modules composition and offer a per-module life-cycle with all instances hidden behind the previously mentioned facades. It also offers a range of tools and components to speed up development, increase stability and drastically reduce boilerplate code. To improve flexibility of modules and applications the core has built in support for configuration management as well as plugin loading.

<p align="center">
    <img src="docs/images/arch_level1.png" width="400px"/>
</p>

Each modules composition is constructed by its own DI-container instance. This makes it possible to dispose the container in order to restart the module and reconstruct the composition with a different configuration or to recover from a fatal error. The `ModuleController` and `Facade` instances are preserved through the lifecycle of the application as part of the level 1 composition. The  Components (*always present*) and plugins (*configurable*) are created when a module is started and disposed when the module stops. For each lifecycle the references of the facade are updated.

<p align="center">
    <img src="docs/images/arch_level2.png" width="400px"/>
</p>

## Resource Management

The [Resource Management](/docs/articles/Resources/ResourceManagement.md) holds the object graph representing the physical system. It maintains the database entities for the object graph in the background and reconstructs the object graph upon boot. It also provides the API for other modules to interact with resources based on their implemented interfaces and supports resource look-up by various conditions or capabilities.

## Product Management

The [Product Management](/docs/articles/Products/ProductManagement.md) holds all product variants, created instances of those variants and recipes how to create an instance. It provides an API giving access to product types, instances, recipes and workplans.

## Tutorials

To start using the MORYX for your own projects you can jump into these tutorials:

* [How to create a Resource](/docs/tutorials/HowToCreateResource.md)
* [How to build a Driver](/docs/tutorials/HowToBuildADriver.md)
* [How to create a Product](/docs/tutorials/HowToCreateAProduct.md)
* [How to create a Server Module](/docs/tutorials/ServerModule/ServerModule.md)

## Maintenance

Part of the Framework is also the Maintenance module, which hosts a HTTP REST service and *optionally* a graphic web interface to control and configure a MORYX application. The Maintenance itself does not define that logic, but simply provides easy external access to APIs and features of the runtime kernel.

## History

Starting with version 3.0 of the core we decided to open source it as a foundation for Industrial IoT (IIoT) applications. For this public version, the framework received an overhaul to replace commercial libraries and tools, remove specialized Phoenix Contact code and better comply with the .NET open source community.

Version 6 uses .net 6 with ASP.net Core and EntityFramework Core 6. All WPF UIs were replaced by WebUIs. In order to make debugging easier, we decided to merge the AbstractionLayer and Core repositories into one and name this one MORYX Framework. The AbstractionLayer will be archived.

A few examples of solutions build on MORYX are listed below:

- Manufacturing Control System
- Plastic Mold Tracking
- Intralogistics
- Home Automation