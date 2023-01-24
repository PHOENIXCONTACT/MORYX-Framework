
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

# MORYX Core

The MORYX Core is a .NET based framework to quickly build three-tier applications. It aims to reduce boilerplate code as much as possible and provides modularity, flexibility and easy configuration with very little effort.
It originates from the original MORYX project targeted to develop machines but has expanded to a much bigger field of use. The MORYX Core defines a base namespace and set of interfaces used to develop modular applications.

The **MORYX AbstractionLayer** is the environment for the digital twins of resources and products. It defines the domain independent [meta model](/docs/articles/AbstractionLayer.md) and enables applications to model their physical system and product portfolio as typed objects. It thereby makes other modules hardware independent by encapsulating details of the underlying structure and devices. [Like the platform](https://github.com/PHOENIXCONTACT/MORYX-Platform#history) version 5.0 of the AbstractionLayer is focused on the open source community and we are still applying the final touches, while the in-house stable version powers a range of different applications.

**Links**

- [Package Feed](https://www.myget.org/feed/Packages/moryx)
- [Repository Template](https://github.com/PHOENIXCONTACT/MORYX-Template)
- [MORYX Factory](https://github.com/PHOENIXCONTACT/MORYX-Factory)

## Getting Started

If you want to start developing with or for MORYX, the easiest way is our [template repository](https://github.com/PHOENIXCONTACT/MORYX-Template). It comes with two empty solutions, the necessary package feeds and preinstalled empty MORYX runtime. Add projects and packages to backend and frontend solutions depending on your specific requirements. Install stable releases via Nuget; development releases are available via MyGet.

| Package Name | Release (NuGet) | CI (MyGet) | Future (MyGet) |
|--------------|-----------------|------------|------------|
| `Moryx` | [![NuGet](https://img.shields.io/nuget/v/Moryx.svg)](https://www.nuget.org/packages/Moryx/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx)](https://www.myget.org/feed/moryx/package/nuget/Moryx) | [![MyGet-Release](https://img.shields.io/myget/moryx-future/vpre/Moryx)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx) |
| `Moryx.Model` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Model.svg)](https://www.nuget.org/packages/Moryx.Model/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Model)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Model) | [![MyGet-Release](https://img.shields.io/myget/moryx-future/vpre/Moryx.Model)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Model) |
| `Moryx.Model.InMemory` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Model.InMemory.svg)](https://www.nuget.org/packages/Moryx.Model.InMemory/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Model.InMemory)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Model.InMemory) | [![MyGet-Release](https://img.shields.io/myget/moryx-future/vpre/Moryx.Model.InMemory)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Model.InMemory) |
| `Moryx.Model.PostgreSQL` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Model.PostgreSQL.svg)](https://www.nuget.org/packages/Moryx.Model.PostgreSQL/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Model.PostgreSQL)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Model.PostgreSQL) | [![MyGet-Release](https://img.shields.io/myget/moryx-future/vpre/Moryx.Model.PostgreSQL)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Model.PostgreSQL) |
| `Moryx.Container` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Container.svg)](https://www.nuget.org/packages/Moryx.Container/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Container)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Container) | [![MyGet-Release](https://img.shields.io/myget/moryx-future/vpre/Moryx.Container)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Container) |
| `Moryx.Communication.Serial` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Communication.Serial.svg)](https://www.nuget.org/packages/Moryx.Communication.Serial/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Communication.Serial)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Communication.Serial) | [![MyGet-Release](https://img.shields.io/myget/moryx-future/vpre/Moryx.Communication.Serial)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Communication.Serial) |
| `Moryx.Tools.Wcf` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Tools.Wcf.svg)](https://www.nuget.org/packages/Moryx.Tools.Wcf/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Tools.Wcf)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Tools.Wcf) | [![MyGet-Release](https://img.shields.io/myget/moryx-future/vpre/Moryx.Tools.Wcf)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Tools.Wcf) |
| `Moryx.Runtime.Wcf` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Runtime.Wcf.svg)](https://www.nuget.org/packages/Moryx.Runtime.Wcf/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Runtime.Wcf)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Runtime.Wcf) | [![MyGet-Release](https://img.shields.io/myget/moryx-future/vpre/Moryx.Runtime.Wcf)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Runtime.Wcf) |
| `Moryx.Runtime` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Runtime.svg)](https://www.nuget.org/packages/Moryx.Runtime/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Runtime)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Runtime) | [![MyGet-Release](https://img.shields.io/myget/moryx-future/vpre/Moryx.Runtime)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Runtime) |
| `Moryx.Runtime.DbUpdate` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Runtime.DbUpdate.svg)](https://www.nuget.org/packages/Moryx.Runtime.DbUpdate/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Runtime.DbUpdate)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Runtime.DbUpdate) | [![MyGet-Release](https://img.shields.io/myget/moryx-future/vpre/Moryx.Runtime.DbUpdate)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Runtime.DbUpdate) |
| `Moryx.Runtime.Kernel` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Runtime.Kernel.svg)](https://www.nuget.org/packages/Moryx.Runtime.Kernel/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Runtime.Kernel)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Runtime.Kernel) | [![MyGet-Release](https://img.shields.io/myget/moryx-future/vpre/Moryx.Runtime.Kernel)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Runtime.Kernel) |
| `Moryx.Runtime.Kestrel` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Runtime.Kestrel.svg)](https://www.nuget.org/packages/Moryx.Runtime.Kestrel/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Runtime.Kestrel)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Runtime.Kestrel) | [![MyGet-Release](https://img.shields.io/myget/moryx-future/vpre/Moryx.Runtime.Kestrel)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Runtime.Kestrel) |
| `Moryx.Runtime.Maintenance` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Runtime.Maintenance.svg)](https://www.nuget.org/packages/Moryx.Runtime.Maintenance/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Runtime.Maintenance)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Runtime.Maintenance) | [![MyGet-Release](https://img.shields.io/myget/moryx-future/vpre/Moryx.Runtime.Maintenance)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Runtime.Maintenance) |
| `Moryx.Runtime.SmokeTest` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Runtime.SmokeTest.svg)](https://www.nuget.org/packages/Moryx.Runtime.SmokeTest/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Runtime.SmokeTest)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Runtime.SmokeTest) | [![MyGet-Release](https://img.shields.io/myget/moryx-future/vpre/Moryx.Runtime.SmokeTest)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Runtime.SmokeTest) |
| `Moryx.Runtime.WinService` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Runtime.WinService.svg)](https://www.nuget.org/packages/Moryx.Runtime.WinService/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Runtime.WinService)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Runtime.WinService) | [![MyGet-Release](https://img.shields.io/myget/moryx-future/vpre/Moryx.Runtime.WinService)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Runtime.WinService) |
| `Moryx.TestTools.SystemTest` | [![NuGet](https://img.shields.io/nuget/v/Moryx.TestTools.SystemTest.svg)](https://www.nuget.org/packages/Moryx.TestTools.SystemTest/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.TestTools.SystemTest)](https://www.myget.org/feed/moryx/package/nuget/Moryx.TestTools.SystemTest) | [![MyGet-Release](https://img.shields.io/myget/moryx-future/vpre/Moryx.TestTools.SystemTest)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.TestTools.SystemTest) |
| `Moryx.AbstractionLayer` | [![NuGet](https://img.shields.io/nuget/v/Moryx.AbstractionLayer.svg)](https://www.nuget.org/packages/Moryx.AbstractionLayer/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.AbstractionLayer)](https://www.myget.org/feed/moryx/package/nuget/Moryx.AbstractionLayer) | [![MyGet](https://img.shields.io/myget/moryx-future/vpre/Moryx.AbstractionLayer)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.AbstractionLayer) |
| `Moryx.AbstractionLayer.TestTools` | [![NuGet](https://img.shields.io/nuget/v/Moryx.AbstractionLayer.TestTools.svg)](https://www.nuget.org/packages/Moryx.AbstractionLayer.TestTools/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.AbstractionLayer.TestTools)](https://www.myget.org/feed/moryx/package/nuget/Moryx.AbstractionLayer.TestTools) | [![MyGet](https://img.shields.io/myget/moryx-future/vpre/Moryx.AbstractionLayer.TestTools)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.AbstractionLayer.TestTools) |
| `Moryx.AbstractionLayer.Products.Endpoints` | [![NuGet](https://img.shields.io/nuget/v/Moryx.AbstractionLayer.Products.Endpoints.svg)](https://www.nuget.org/packages/Moryx.AbstractionLayer.Products.Endpoints/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.AbstractionLayer.Products.Endpoints)](https://www.myget.org/feed/moryx/package/nuget/Moryx.AbstractionLayer.Products.Endpoints) | [![MyGet](https://img.shields.io/myget/moryx-future/vpre/Moryx.AbstractionLayer.Products.Endpoints)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.AbstractionLayer.Products.Endpoints) |
| `Moryx.AbstractionLayer.Resources.Endpoints` | [![NuGet](https://img.shields.io/nuget/v/Moryx.AbstractionLayer.Resources.Endpoints.svg)](https://www.nuget.org/packages/Moryx.AbstractionLayer.Resources.Endpoints/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.AbstractionLayer.Resources.Endpoints)](https://www.myget.org/feed/moryx/package/nuget/Moryx.AbstractionLayer.Resources.Endpoints) | [![MyGet](https://img.shields.io/myget/moryx-future/vpre/Moryx.AbstractionLayer.Resources.Endpoints)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.AbstractionLayer.Resources.Endpoints) |
| `Moryx.Notifications` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Notifications.svg)](https://www.nuget.org/packages/Moryx.Notifications/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Notifications)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Notifications) | [![MyGet](https://img.shields.io/myget/moryx-future/vpre/Moryx.Notifications)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Notifications) |
| `Moryx.Products.Management` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Products.Management.svg)](https://www.nuget.org/packages/Moryx.Products.Management/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Products.Management)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Products.Management) | [![MyGet](https://img.shields.io/myget/moryx-future/vpre/Moryx.Products.Management)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Products.Management) |
| `Moryx.Products.Model` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Products.Model.svg)](https://www.nuget.org/packages/Moryx.Products.Model/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Products.Model)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Products.Model) | [![MyGet](https://img.shields.io/myget/moryx-future/vpre/Moryx.Products.Model)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Products.Model) |
| `Moryx.Resources.Management` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Resources.Management.svg)](https://www.nuget.org/packages/Moryx.Resources.Management/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Resources.Management)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Resources.Management) | [![MyGet](https://img.shields.io/myget/moryx-future/vpre/Moryx.Resources.Management)](https://www.myget.org/feed/moryx-future/package/nuget/Moryx.Resources.Management) |

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

Part of the Core is also the Maintenance module, which hosts a HTTP REST service and *optionally* a graphic web interface to control and configure a MORYX application. The Maintenance itself does not define that logic, but simply provides easy external access to APIs and features of the runtime kernel.

## History

Starting with version 3.0 of the core we decided to open source it as a foundation for Industrial IoT (IIoT) applications. For this public version, the framework received an overhaul to replace commercial libraries and tools, remove specialized Phoenix Contact code and better comply with the .NET open source community.

Version 6 uses .net 6 with ASP.net Core and EntityFramework 6. All WPF UIs were replaced by WebUIs.

A few examples of solutions build on MORYX are listed below:

- Manufacturing Control System
- Plastic Mold Tracking
- Intralogistics
- Home Automation