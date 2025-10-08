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

The **MORYX Framework** is a .NET based framework to quickly build three-tier applications. It aims to reduce boilerplate code as much as possible and provides modularity, flexibility and easy configuration with very little effort.
It originates from the original MORYX project targeted to develop machines but has expanded to a much bigger field of use. 
The **MORYX AbstractionLayer** is the environment for the digital twins of resources and products. 
It defines the domain independent [meta model](/docs/articles/abstractions/index.md) and enables applications to model their physical system and product portfolio as typed objects. It thereby makes other modules hardware independent by encapsulating details of the underlying structure and devices.

## Getting Started

To _participate in the development_ of the MORYX Framework, 
- checkout this repository 
- use an IDE like [Visual Studio](https://visualstudio.microsoft.com/) or [JetBrains Rider](https://www.jetbrains.com/rider/) to open the solution file `MORYX-Framework.sln`
- build the solution

To use MORYX to _build your own application_, we recommend to 
- start with the [MORYX Application Developer Training](https://github.com/PHOENIXCONTACT/MORYX-Application-Developer-Training) 
- and to use the [MORYX CLI Tool](https://github.com/PHOENIXCONTACT/MORYX-CLI) to develop it.
- Additional tutorials for specific topics can be found in the [Tutorials](./docs/articles/tutorials/index.md) section of the documentation.

To quickly get a running MORYX application to _check out the different components_, we offer the [MORYX Demo](https://github.com/PHOENIXCONTACT/MORYX-Demo), a fully functional, simulated production system based on MORYX.

## The Ecosystem
Here we list all available packages in the MORYX ecosystem separated into components, linking to their documentation and providing the respective code coverage for the component.

<details>
<summary><font size="4"><strong>MORYX Framework and Abstractions</strong></font><br>(Includes the Framework as the core of any MORYX application as well as public APIs for components used to abstract digital twins)
</summary>


|Component|Packages|Coverage|
|:--------|:------:|:------:|
|[MORYX](./docs/articles/framework/index.md)|[Moryx](./src/Moryx)<br>[Moryx.Asp.Extensions](./src/Moryx.Asp.Extensions)|to be done|
|[MORYX - Abstractions](./docs/articles/abstractions/index.md)|[Moryx.AbstractionLayer](./src/Moryx.AbstractionLayer)<br>[Moryx.ControlSystem](./src/Moryx.ControlSystem)<br>[Moryx.Factory](./src/Moryx.Factory)<br>[Moryx.Users](./src/Moryx.Users)|to be done|
|[MORYX - Container](./docs/articles/container/index.md)|[Moryx.Container](./src/Moryx.Container)|to be done|
|[MORYX - Runtime](./docs/articles/runtime/index.md)|[Moryx.Runtime](./src/Moryx.Runtime)<br>[Moryx.Runtime.DbUpdate](./src/Moryx.Runtime.DbUpdate)<br>[Moryx.Runtime.Endpoints](./src/Moryx.Runtime.Endpoints)<br>[Moryx.Runtime.Kernel](./src/Moryx.Runtime.Kernel)<br>[Moryx.Runtime.Kestrel](./src/Moryx.Runtime.Kestrel)<br>[Moryx.Runtime.Maintenance](./src/Moryx.Runtime.Maintenance)<br>[Moryx.Runtime.SmokeTest](./src/Moryx.Runtime.SmokeTest)<br>[Moryx.Runtime.Wcf](./src/Moryx.Runtime.Wcf)<br>[Moryx.Runtime.WinService](./src/Moryx.Runtime.WinService)|to be done|
|[MORYX - Model](./docs/articles/model/index.md)|[Moryx.Model](./src/Moryx.Model)<br>[Moryx.Model.InMemory](./src/Moryx.Model.InMemory)<br>[Moryx.Model.PostgreSQL](./src/Moryx.Model.PostgreSQL)<br>[Moryx.Model.Sqlite](./src/Moryx.Model.Sqlite)|to be done|
|[MORYX - Communication](./docs/articles/communication/index.md)|[Moryx.Communication.Serial](./src/Moryx.Communication.Serial)|to be done|
|[MORYX - Tools](./docs/articles/tools/index.md)|[Moryx.Tools](./src/Moryx.Tools)<br>[Moryx.Tools.Wcf](./src/Moryx.Tools.Wcf)[Moryx.TestTools.NUnit](./src/Moryx.TestTools.NUnit)<br>[Moryx.TestTools.SystemTest](./src/Moryx.TestTools.SystemTest)<br>[Moryx.TestTools.Test.Model](./src/Moryx.TestTools.Test.Model)<br>[Moryx.TestTools.UnitTest](./src/Moryx.TestTools.UnitTest)<br>[Moryx.ControlSystem.TestTools](./src/Moryx.ControlSystem.TestTools)<br>[Moryx.ControlSystem.TestTools.Assemble](./src/Moryx.ControlSystem.Assemble)<br>[Moryx.TestTools.IntegrationTest](./src/Moryx.TestTools.IntegrationTest)|to be done|
|[MORYX - Command Center](./docs/articles/command-center/index.md)|[Moryx.CommandCenter.Web](./src/Moryx.CommandCenter.Web)|to be done|
|Samples|[Moryx.Benchmarking](./src/Moryx.Benchmarking)<br>[Moryx.Orders.Samples](./src/Moryx.Orders.Samples)<br>[Moryx.ProcessData.Samples](./src/Moryx.ProcessData.Samples)<br>[Moryx.Products.Samples](./src/Moryx.Products.Samples)<br>[TestModule](./src/TestModule)<br>[Moryx.TestModule.Kestrel](./src/Moryx.TestModule.Kestrel)<br>|-|

</details>

---

<details>
<summary><font size="4"><strong>MORYX Adapters and Modules</strong></font><br>(Includes all adapters and modules built on top of the MORYX Framework, ready to be used in your applications)
</summary>

|Component|Packages|Coverage|
|:--------|:------:|:------:|
|[Adapter - Influx DB](./docs/articles/adapter-influx-db/index.md)|[Moryx.ProcessData.InfluxDbListener](./src/Moryx.ProcessData.InfluxDbListener)|to be done|
|[Adapter - Spreadsheet](./docs/articles/adapter-spreadsheet/index.md)|[Moryx.ProcessData.SpreadsheetsListener](./src/Moryx.ProcessData.SpreadsheetsListener)|to be done|
|[Module - Products](./docs/articles/module-products/index.md)|[Moryx.Products.Model](./src/Moryx.Products.Model)<br>[Moryx.Products.Management](./src/Moryx.Products.Management)<br>[Moryx.Products.Web](./src/Moryx.Products.Web)<br>[Moryx.AbstractionLayer.Products.Endpoints](./src/Moryx.AbstractionLayer.Products.Endpoints)|to be done|
|[Module - Resources](./docs/articles/module-resources/index.md)|[Moryx.Resources.Management](./src/Moryx.Resources.Management)<br>[Moryx.AbstractionLayer.Resources.Endpoints](./src/Moryx.AbstractionLayer.Resources.Endpoints)|to be done|
|[Module - Analytics](./docs/articles/module-analytics/index.md)|[Moryx.Analytics.Server](./src/Moryx.Analytics.Server)<br>[Moryx.Analytics.Web](./src/Moryx.Analytics.Web)|to be done|
|[Module - Media](./docs/articles/module-media/index.md)|[Moryx.Media](./src/Moryx.Media)<br>[Moryx.Media.Server](./src/Moryx.Media.Server)<br>[Moryx.Media.Endpoints](./src/Moryx.Media.Endpoints)<br>[Moryx.Media.Web](./src/Moryx.Media.Web)|to be done|
|[Module - Notifications](./docs/articles/module-notifications/index.md)|[Moryx.Notifications](./src/Moryx.Notifications)<br>[Moryx.Notifications.Publisher](./src/Moryx.Notifications.Publisher)<br>[Moryx.Notifications.Endpoints](./src/Moryx.Notifications.Endpoints)<br>[Moryx.Notifications.Web](./src/Moryx.Notifications.Web)|to be done|
|[Module - Orders](./docs/articles/module-orders/index.md)|[Moryx.Orders](./src/Moryx.Orders)<br>[Moryx.Orders.Management](./src/Moryx.Orders.Management)<br>[Moryx.Orders.Endpoints](./src/Moryx.Orders.Endpoints)<br>[Moryx.Orders.Web](./src/Moryx.Orders.Web)|to be done|
|[Module - Process Data](./docs/articles/module-process-data/index.md)|[Moryx.ProcessData](./src/Moryx.ProcessData)<br>[Moryx.ProcessData.Adapter.NotificationPublisher](./src/Moryx.ProcessData.Adapter.NotificationPublisher)<br>[Moryx.ProcessData.Adapter.OrderManagement](./src/Moryx.ProcessData.Adapter.OrderManagement)<br>[Moryx.ProcessData.Adapter.ProcessEngine](./src/Moryx.ProcessData.Adapter.ProcessEngine)<br>[Moryx.ProcessData.Adapter.ResourceManagement](./src/Moryx.ProcessData.Adapter.ResourceManagement)<br>[Moryx.ProcessData.Endpoints](./src/Moryx.ProcessData.Endpoints)<br>[Moryx.ProcessData.Monitor](./src/Moryx.ProcessData.Monitor)|to be done|
|[Module - Process Engine](./docs/articles/module-process-engine/index.md)|[Moryx.ControlSystem.ProcessEngine](./src/Moryx.ControlSystem.ProcessEngine)<br>[Moryx.ControlSystem.ProcessEngine.Web](./src/Moryx.ControlSystem.ProcessEngine.Web)<br>[Moryx.ControlSystem.Processes.Endpoints](./src/Moryx.ControlSystem.Processes.Endpoints)<br>[Moryx.ControlSystem.Jobs.Endpoints](./src/Moryx.ControlSystem.Jobs.Endpoints)|to be done|
|[Module - Setups](./docs/articles/module-setups/index.md)|[Moryx.ControlSystem.SetupProvider](./src/Moryx.ControlSystem.SetupProvider)|to be done|
|[Module - Simulation](./docs/articles/module-simulation/index.md)|[Moryx.ControlSystem](./src/Moryx.ControlSystem)<br>[Moryx.ControlSystem.Simulator](./src/Moryx.ControlSystem.Simulator/)<br>[Moryx.Drivers.Simulation](./src/Moryx.Drivers.Simulation)|to be done|
|[Module - Worker Support](./docs/articles/module-worker-support/index.md)|[Moryx.ControlSystem.WorkerSupport](./src/Moryx.ControlSystem.WorkerSupport)<br>[Moryx.ControlSystem.WorkerSupport.Web](./src/Moryx.ControlSystem.WorkerSupport.Web)<br>[Moryx.ControlSystem.VisualInstructions.Endpoints](./src/Moryx.ControlSystem.VisualInstructions.Endpoints)<br>[Moryx.Resources.AssemblyInstruction](./src/Moryx.Resources.AssemblyInstruction)|to be done|
|[Module - Workplans](./docs/articles/module-workplans/index.md)|[Moryx.Workplans](./src/Moryx.Workplans)<br>[Moryx.Workplans.Editing](./src/Moryx.Workplans.Editing)<br>[Moryx.Workplans.Web](./src/Moryx.Workplans.Web)|to be done|
|[MORYX - Access Management](./docs/articles/moryx-access-management/index.md)|[Moryx.Identity](./src/Moryx.Identity)<br>[Moryx.Identity.AccessManagement](./src/Moryx.Identity.AccessManagement)<br>[Moryx.Identity.Web](./src/Moryx.Identity.Web)|to be done|
|[Web - Factory Monitor](./docs/articles/web-factory-monitor/index.md)|[Moryx.FactoryMonitor.Endpoints](./src/Moryx.FactoryMonitor.Endpoints)<br>[Moryx.FactoryMonitor.Web](./src/Moryx.FactoryMonitor.Web)|to be done|
|[Web - Launcher](./docs/articles/web-launcher/index.md)|[Moryx.Launcher](./src/Moryx.Launcher)|to be done|
|[Module - Operators](./docs/articles/module-operators/index.md)|[Moryx.Operators](./src/Moryx.Operators)<br>[Moryx.Operators.Management](./src/Moryx.Operators.Management)<br>[Moryx.Operators.Endpoints](./src/Moryx.Operators.Endpoints)<br>[Moryx.Operators.Web](./src/Moryx.Operators.Web)|to be done|
|[Module - Shifts](./docs/articles/module-shifts/index.md)|[Moryx.Shifts](./src/Moryx.Shifts)<br>[Moryx.Shifts.Management](./src/Moryx.Shifts.Management)<br>[Moryx.Shifts.Endpoints](./src/Moryx.Shifts.Endpoints)<br>[Moryx.Shifts.Web](./src/Moryx.Shifts.Web)|to be done|
|[Module - Material Management](./docs/articles/module-material-management/index.md)|[Moryx.ControlSystem.MaterialManager](./src/Moryx.ControlSystem.MaterialManager)|to be done|
|[Driver - MQTT](./docs/articles/driver-mqtt/index.md)|[Moryx.Drivers.Mqtt](./src/Moryx.Drivers.Mqtt)|to be done|
|[Driver - OPC UA](./docs/articles/driver-opc-ua/index.md)|[Moryx.Drivers.OpcUa](./src/Moryx.Drivers.OpcUa)|to be done|
</details>

---

<details>
<summary><font size="4"><strong>MORYX Pending Platform Developments</strong></font><br>(Includes planned additions to the platform portfolio which are in different states of developments)
</summary>

|Component|Latest Version|Pipeline Status|Coverage|Requirements|
|:--------|:------------:|:-------------:|:------:|:----------:|
|[Module - Maintenance Calender]()|〰️|〰️|〰️|
|[Module - Transport Controller]()|〰️|〰️|〰️|
|[Module - Identifier Provider]()|〰️|〰️|〰️|

</details>

---

## Architecture of the MORYX Framework
At the core MORYX is a .NET based framework to quickly build three-tier applications. Its architecture is a modular monolith using the service and facade pattern to isolate and decouple functionality. It uses a 2-level Dependency Injection structure to isolate a modules composition and offer a per-module life-cycle with all instances hidden behind the previously mentioned facades. It also offers a range of tools and components to speed up development, increase stability and drastically reduce boilerplate code. To improve flexibility of modules and applications the core has built in support for configuration management as well as plugin loading.

<p align="center">
    <img src="docs/images/arch_level1.png" width="400px"/>
</p>

Each modules composition is constructed by its own DI-container instance. This makes it possible to dispose the container in order to restart the module and reconstruct the composition with a different configuration or to recover from a fatal error. The `ModuleController` and `Facade` instances are preserved through the lifecycle of the application as part of the level 1 composition. The  Components (*always present*) and plugins (*configurable*) are created when a module is started and disposed when the module stops. For each lifecycle the references of the facade are updated.

<p align="center">
    <img src="docs/images/arch_level2.png" width="400px"/>
</p>

## Key Features of the factory automation components

**Modular Manufacturing Systems**
Arbitrary production units (cells/stations) can be added or removed from the manufacturing system. New production capabilities will be used immediately without modifying existing workplans.

**One Piece Flow and Mass Production**
The control system components supports both, one piece flow and mass production.

**Worker Assistance**
The worker gets a detailed description on process steps with options to confirm their execution, report problems and trace application specific user inputs on the process steps. 

**Enterprise Integration**
The system is designed to allow interaction with data repositories like PDM or ERP systems with the goal to enrich their data and fuel the cyber-physical production process without duplicating data. 

## History

Starting with **version** 3.0 of the core we decided to open source it as a foundation for Industrial IoT (IIoT) applications. For this public version, the framework received an overhaul to replace commercial libraries and tools, remove specialized Phoenix Contact code and better comply with the .NET open source community.

**Version 6** uses .net 6 with ASP.net Core and EntityFramework Core 6. All WPF UIs were replaced by WebUIs. 
In order to make debugging easier, we decided to merge the AbstractionLayer and Core repositories into one and name this one MORYX Framework. The AbstractionLayer will be archived.
Additionally, all MORYX components are now released under the same major version as the MORYX-Framework and aligned with the .NET release schedule.

With **version 10** the complete MORYX portfolio aside from Phoenix Contact specific components is released as open source. 
We decided to combine all components into this mono-repository to make it easier to keep everything in sync and to lower the barrier for contributions.
The previously seperated repository MORYX-Factory as well as the internal repositories will be archived.