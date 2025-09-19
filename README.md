<p align="center">
    <img src="docs/resources/MORYX_logo.svg" alt="MORYX Logo" width="300px" />
</p>

<p align="center">
    <a href="https://github.com/PHOENIXCONTACT/MORYX-Framework/workflows">
        <img src="https://github.com/PHOENIXCONTACT/MORYX-Framework/workflows/CI/badge.svg" alt="CI">
    </a>
    <a href="https://codecov.io/gh/PHOENIXCONTACT/MORYX-Framework/coverage.svg?branch=dev">
        <img alt="Coverage" src="https://codecov.io/gh/PHOENIXCONTACT/MORYX-Framework/coverage.svg?branch=dev" />
    </a>
    <a href="https://gitter.im/PHOENIXCONTACT/MORYX?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge">
        <img src="https://img.shields.io/gitter/room/Phoenixcontact/MORYX?logo=gitter" alt="Gitter">
    </a>
    <a href="https://stackoverflow.com/questions/tagged/moryx">
        <img src="https://img.shields.io/badge/stackoverflow-moryx-orange?logo=stackoverflow&style=flat" alt="Stack Overflow">
    </a>
    <a href="https://github.com/PHOENIXCONTACT/MORYX-Framework/blob/dev/LICENSE">
        <img src="https://img.shields.io/github/license/PHOENIXCONTACT/MORYX-Framework" alt="License">
    </a>
</p>

<p align="center">
    <a href="https://www.nuget.org/packages/Moryx/">
        <img alt="NuGet Release" src="https://img.shields.io/nuget/v/Moryx?color=0098A1&logo=nuget" alt="Nuget Version">
    </a>
    <a href="https://www.myget.org/feed/moryx-oss-ci/package/nuget/Moryx">
        <img alt="MyGet PreRelease" src="https://img.shields.io/myget/moryx-oss-ci/vpre/Moryx?logo=nuget&label=myget&color=0098A1&link=https%3A%2F%2Fwww.myget.org%2FF%2Fmoryx-oss-ci%2Fapi%2Fv3%2Findex.json" alt="Myget Pre-Release Version">
    </a>
</p>

# MORYX Framework

The MORYX Framework is a .NET based framework to quickly build three-tier applications. It aims to reduce boilerplate code as much as possible and provides modularity, flexibility and easy configuration with very little effort.
It originates from the original MORYX project targeted to develop machines but has expanded to a much bigger field of use. 

The **MORYX Core** defines a base namespace and set of interfaces used to develop modular applications.

The **MORYX AbstractionLayer** is the environment for the digital twins of resources and products. It defines the domain independent [meta model](/docs/articles/AbstractionLayer.md) and enables applications to model their physical system and product portfolio as typed objects. It thereby makes other modules hardware independent by encapsulating details of the underlying structure and devices.

The **MORYX Factory** contains the APIs, domain objects and developer documentation for the MORYX factory scope.

**Links**
- [Application-Developer Training](https://github.com/PHOENIXCONTACT/Application-Developer-Training)
- [Demo Application](https://github.com/PHOENIXCONTACT/MORYX-Demo)
- [MORYX CLI Tool](https://github.com/PHOENIXCONTACT/MORYX-CLI)
- [Repository Template](https://github.com/PHOENIXCONTACT/MORYX-Template)


## The Ecosystem
Here we list all available packages in the MORYX ecosystem separated into components, linking to their documentation and pproviding the respective code coverage for the component.

<details>
<summary><font size="4"><strong>MORYX Framework and Abstractions</strong></font><br>(Includes the Framework as the core of any MORYX application as well as public APIs for components used to abstract digital twins)
</summary>


|Component|Packages|Documentation|Coverage|
|:--------|:------:|:-----------:|:------:|
|<details><summary>[MORYX - Framework](https://github.com/PHOENIXCONTACT/MORYX-Framework)</summary>Moryx<br>Moryx.Model<br>Moryx.Model.InMemory<br>Moryx.Model.PostgreSQL<br>Moryx.Model.Sqlite<br>Moryx.Container<br>Moryx.Communication.Serial<br>Moryx.Asp.Extensions<br>Moryx.Runtime<br>Moryx.Notifications<br>Moryx.AbstractionLayer<br>Moryx.AbstractionLayer.TestTools<br>Moryx.TestTools.UnitTest</details>|![GitHub Release](https://img.shields.io/github/v/release/PHOENIXCONTACT/MORYX-Framework?sort=semver&filter=v8*&label=latest%20release&link=https%3A%2F%2Fgithub.com%2FPHOENIXCONTACT%2FMORYX-Framework%2Freleases%3Fq%3D8*%26expanded%3Dtrue)<br>![GitHub Release](https://img.shields.io/github/v/release/PHOENIXCONTACT/MORYX-Framework?sort=semver&filter=v6*&label=release%20(MORYX%206)&link=https%3A%2F%2Fgithub.com%2FPHOENIXCONTACT%2FMORYX-Framework%2Freleases%3Fq%3Dv6*%26expanded%3Dtrue)|![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/PHOENIXCONTACT/Moryx-Framework/build-and-test.yml?branch=dev&event=push&label=pipeline%20(MORYX%208)&link=https%3A%2F%2Fgithub.com%2FPHOENIXCONTACT%2FMORYX-Framework%2Factions%2Fworkflows%2Fbuild-and-test.yml%3Fquery%3Dbranch%253Adev%2B%2B)<br>![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/PHOENIXCONTACT/Moryx-Framework/build-and-test.yml?branch=release%2F6&event=push&label=pipeline%20(MORYX%206)&link=https%3A%2F%2Fgithub.com%2FPHOENIXCONTACT%2FMORYX-Framework%2Factions%2Fworkflows%2Fbuild-and-test.yml%3Fquery%3Dbranch%253Arelease%252F6%2B%2B)|〰️|_missing_|
|<details><summary>[MORYX - Factory](https://github.com/PHOENIXCONTACT/MORYX-Factory)</summary>Moryx.ControlSystem<br>Moryx.Orders<br>Moryx.Users<br>Moryx.ProcessData<br>Moryx.Simulation</details>|![GitHub Release](https://img.shields.io/github/v/release/PHOENIXCONTACT/Moryx-Factory?sort=semver&filter=v8*&label=latest%20release&link=https%3A%2F%2Fgithub.com%2FPHOENIXCONTACT%2FMoryx-Factory%2Freleases%3Fq%3D8*%26expanded%3Dtrue)<br>![GitHub Release](https://img.shields.io/github/v/release/PHOENIXCONTACT/Moryx-Factory?sort=semver&filter=v6*&label=release%20(MORYX%206)&link=https%3A%2F%2Fgithub.com%2FPHOENIXCONTACT%2FMoryx-Factory%2Freleases%3Fq%3Dv6*%26expanded%3Dtrue)|![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/PHOENIXCONTACT/Moryx-Factory/build-and-test.yml?branch=dev&event=push&label=pipeline%20(MORYX%208)&link=https%3A%2F%2Fgithub.com%2FPHOENIXCONTACT%2FMoryx-Factory%2Factions%2Fworkflows%2Fbuild-and-test.yml%3Fquery%3Dbranch%253Adev%2B%2B)<br>![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/PHOENIXCONTACT/Moryx-Factory/build-and-test.yml?branch=release%2F6&event=push&label=pipeline%20(MORYX%206)&link=https%3A%2F%2Fgithub.com%2FPHOENIXCONTACT%2FMoryx-Factory%2Factions%2Fworkflows%2Fbuild-and-test.yml%3Fquery%3Dbranch%253Arelease%252F6%2B%2B)|〰️|_missing_|
|<details><summary>[Web - Command Center](https://github.com/PHOENIXCONTACT/MORYX-Framework)</summary>Moryx.Runtime.Kernel<br>Moryx.Runtime.Endpoints<br>Moryx.CommandCenter.Web</details>|![GitHub Release](https://img.shields.io/github/v/release/PHOENIXCONTACT/MORYX-Framework?sort=semver&filter=v8*&label=latest%20release&link=https%3A%2F%2Fgithub.com%2FPHOENIXCONTACT%2FMORYX-Framework%2Freleases%3Fq%3D8*%26expanded%3Dtrue)<br>![GitHub Release](https://img.shields.io/github/v/release/PHOENIXCONTACT/MORYX-Framework?sort=semver&filter=v6*&label=release%20(MORYX%206)&link=https%3A%2F%2Fgithub.com%2FPHOENIXCONTACT%2FMORYX-Framework%2Freleases%3Fq%3Dv6*%26expanded%3Dtrue)|![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/PHOENIXCONTACT/Moryx-Framework/build-and-test.yml?branch=dev&event=push&label=pipeline%20(MORYX%208)&link=https%3A%2F%2Fgithub.com%2FPHOENIXCONTACT%2FMORYX-Framework%2Factions%2Fworkflows%2Fbuild-and-test.yml%3Fquery%3Dbranch%253Adev%2B%2B)<br>![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/PHOENIXCONTACT/Moryx-Framework/build-and-test.yml?branch=release%2F6&event=push&label=pipeline%20(MORYX%206)&link=https%3A%2F%2Fgithub.com%2FPHOENIXCONTACT%2FMORYX-Framework%2Factions%2Fworkflows%2Fbuild-and-test.yml%3Fquery%3Dbranch%253Arelease%252F6%2B%2B)|〰️|_missing_|

</details>

---

<details>
<summary><font size="4"><strong>MORYX Adapters and Modules</strong></font><br>(Includes all adapters and modules built on top of the MORYX Framework, ready to be used in your applications)
</summary>

|Component|Packages|Documentation|Coverage|
|:--------|:------:|:-----------:|:------:|
|Adapter - Influx DB|Moryx.ProcessData.InfluxDbListener| |to be done|
|Adapter - Spreadsheet|Moryx.ProcessData.SpreadsheetsListener| |to be done|
|Module - Products|Moryx.Products.Model<br>Moryx.Products.Management<br>Moryx.Products.Web<br>Moryx.AbstractionLayer.Products.Endpoints| |to be done|
|Module - Resources|Moryx.Resources.Management<br>Moryx.AbstractionLayer.Resources.Endpoints| |to be done|
|Module - Analytics|Moryx.Analytics.Server<br>Moryx.Analytics.Web| |to be done|
|Module - Media|Moryx.Media<br>Moryx.Media.Server<br>Moryx.Media.Endpoints<br>Moryx.Media.Web| |to be done|
|Module - Notifications|Moryx.Notifications<br>Moryx.Notifications.Publisher<br>Moryx.Notifications.Endpoints<br>Moryx.Notifications.Web| |to be done|
|Module - Orders|Moryx.Orders<br>Moryx.Orders.Management<br>Moryx.Orders.Endpoints<br>Moryx.Orders.Web| |to be done|
|Module - Process Data|Moryx.ProcessData<br>Moryx.ProcessData.Adapter.NotificationPublisher<br>Moryx.ProcessData.Adapter.OrderManagement<br>Moryx.ProcessData.Adapter.ProcessEngine<br>Moryx.ProcessData.Adapter.ResourceManagement<br>Moryx.ProcessData.Endpoints<br>Moryx.ProcessData.Monitor| |to be done|
|Module - Process Engine|Moryx.ControlSystem.ProcessEngine<br>Moryx.ControlSystem.ProcessEngine.Web<br>Moryx.ControlSystem.Processes.Endpoints<br>Moryx.ControlSystem.Jobs.Endpoints| |to be done|
|Module - Setups|Moryx.ControlSystem.SetupProvider| |to be done|
|Module - Simulation|Moryx.Drivers.Simulation| |to be done|
|Module - Worker Support|Moryx.ControlSystem.WorkerSupport<br>Moryx.ControlSystem.WorkerSupport.Web<br>Moryx.ControlSystem.VisualInstructions.Endpoints<br>Moryx.Resources.AssemblyInstruction| |to be done|
|Module - Workplans|Moryx.Workplans<br>Moryx.Workplans.Editing<br>Moryx.Workplans.Web| |to be done|
|MORYX - Access Management|Moryx.Identity<br>Moryx.Identity.AccessManagement<br>Moryx.Identity.Web| |to be done|
|Web - Factory Monitor|Moryx.FactoryMonitor.Endpoints<br>Moryx.FactoryMonitor.Web| |to be done|
|Web - Launcher|Moryx.Launcher| |to be done|
|Module - Operators|Moryx.Operators<br>Moryx.Operators.Management<br>Moryx.Operators.Endpoints<br>Moryx.Operators.Web| |to be done|
|Module - Shifts|Moryx.Shifts<br>Moryx.Shifts.Management<br>Moryx.Shifts.Endpoints<br>Moryx.Shifts.Web| |to be done|
|Module - Material Management|Moryx.ControlSystem.MaterialManager| |to be done|
|Driver - MQTT|Moryx.Drivers.Mqtt| |to be done|
|Driver - OPC UA|Moryx.Drivers.OpcUa| |to be done|

</details>

---

<details>
<summary><font size="4"><strong>MORYX Pending Platform Developments</strong></font><br>(Includes planned additions to the platform portfolio which are in different states of developments)
</summary>

|Component|Latest Version|Pipeline Status|Coverage|Requirements|
|:--------|:------------:|:-------------:|:------:|:----------:|
|[Module - Maintenance Calender]()|〰️|〰️|〰️|
|[Module - Material Management]()|〰️|〰️|〰️|
|[Module - Transport Controller]()|〰️|〰️|〰️|
|[Module - Identifier Provider]()|〰️|〰️|〰️|

</details>

---

## Getting Started

If you want to start developing with or for MORYX, the easiest way is our [template repository](https://github.com/PHOENIXCONTACT/MORYX-Template). It comes with two empty solutions, the necessary package feeds and preinstalled empty MORYX runtime. Add projects and packages to backend and frontend solutions depending on your specific requirements. Install stable releases via Nuget; development releases are available via MyGet.

If you wish to contribute to this project, you simply need to clone the repository and open the solution with Visual Studio 2017 or above. The Debug target should be *StartProject*.

To build and run from the command line you need powershell and msbuild.

```powershell
.\Build.ps1 -Build
.\src\StartProject\bin\Debug\StartProject.exe
```


## Architecture
The MORYX core is a .NET based framework to quickly build three-tier applications. Its architecture is a modular monolith using the service and facade pattern to isolate and decouple functionality. It uses a 2-level Dependency Injection structure to isolate a modules composition and offer a per-module life-cycle with all instances hidden behind the previously mentioned facades. It also offers a range of tools and components to speed up development, increase stability and drastically reduce boilerplate code. To improve flexibility of modules and applications the core has built in support for configuration management as well as plugin loading.

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