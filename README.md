
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

The MORYX Core is a .NET based framework to quickly build three-tier applications. It aims to reduce boilerplate code as much as possible and provides modularity, flexibility and easy configuration with very little effort. It is also the foundation for the Phoenix Contact IoT Framework [MORYX Abstraction Layer](https://github.com/PHOENIXCONTACT/MORYX-AbstractionLayer)

**Links**

- [Package Feed](https://www.myget.org/feed/Packages/moryx)
- [Repository Template](https://github.com/PHOENIXCONTACT/MORYX-Template)
- [MORYX Maintenance](https://github.com/PHOENIXCONTACT/MORYX-MaintenanceWeb)
- [MORYX ClientFramework](https://github.com/PHOENIXCONTACT/MORYX-ClientFramework)
- [MORYX Abstraction Layer](https://github.com/PHOENIXCONTACT/MORYX-AbstractionLayer)

## Getting started

If you want to start developing with or for MORYX, the easiest way is our [template repository](https://github.com/PHOENIXCONTACT/MORYX-Template). It comes with two empty solutions, the necessary package feeds and preinstalled empty MORYX runtime. Add projects and packages to backend and frontend solutions depending on your specific requirements. Install stable releases via Nuget; development releases are available via MyGet.

| Package Name | Release (NuGet) | CI (MyGet) |
|--------------|-----------------|------------|
| `Moryx` | [![NuGet](https://img.shields.io/nuget/v/Moryx.svg)](https://www.nuget.org/packages/Moryx/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx)](https://www.myget.org/feed/moryx/package/nuget/Moryx) |
| `Moryx.Model` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Model.svg)](https://www.nuget.org/packages/Moryx.Model/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Model)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Model) |
| `Moryx.Model.InMemory` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Model.InMemory.svg)](https://www.nuget.org/packages/Moryx.Model.InMemory/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Model.InMemory)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Model.InMemory) |
| `Moryx.Model.PostgreSQL` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Model.PostgreSQL.svg)](https://www.nuget.org/packages/Moryx.Model.PostgreSQL/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Model.PostgreSQL)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Model.PostgreSQL) |
| `Moryx.Container` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Container.svg)](https://www.nuget.org/packages/Moryx.Container/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Container)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Container) |
| `Moryx.Communication.Serial` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Communication.Serial.svg)](https://www.nuget.org/packages/Moryx.Communication.Serial/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Communication.Serial)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Communication.Serial) |
| `Moryx.Tools.Wcf` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Tools.Wcf.svg)](https://www.nuget.org/packages/Moryx.Tools.Wcf/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Tools.Wcf)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Tools.Wcf) |
| `Moryx.Runtime.Wcf` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Runtime.Wcf.svg)](https://www.nuget.org/packages/Moryx.Runtime.Wcf/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Runtime.Wcf)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Runtime.Wcf) |
| `Moryx.Runtime` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Runtime.svg)](https://www.nuget.org/packages/Moryx.Runtime/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Runtime)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Runtime) |
| `Moryx.Runtime.DbUpdate` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Runtime.DbUpdate.svg)](https://www.nuget.org/packages/Moryx.Runtime.DbUpdate/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Runtime.DbUpdate)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Runtime.DbUpdate) |
| `Moryx.Runtime.Kernel` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Runtime.Kernel.svg)](https://www.nuget.org/packages/Moryx.Runtime.Kernel/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Runtime.Kernel)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Runtime.Kernel) |
| `Moryx.Runtime.Maintenance` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Runtime.Maintenance.svg)](https://www.nuget.org/packages/Moryx.Runtime.Maintenance/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Runtime.Maintenance)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Runtime.Maintenance) |
| `Moryx.Runtime.SmokeTest` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Runtime.SmokeTest.svg)](https://www.nuget.org/packages/Moryx.Runtime.SmokeTest/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Runtime.SmokeTest)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Runtime.SmokeTest) |
| `Moryx.Runtime.WinService` | [![NuGet](https://img.shields.io/nuget/v/Moryx.Runtime.WinService.svg)](https://www.nuget.org/packages/Moryx.Runtime.WinService/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.Runtime.WinService)](https://www.myget.org/feed/moryx/package/nuget/Moryx.Runtime.WinService) |
| `Moryx.TestTools.SystemTest` | [![NuGet](https://img.shields.io/nuget/v/Moryx.TestTools.SystemTest.svg)](https://www.nuget.org/packages/Moryx.TestTools.SystemTest/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.TestTools.SystemTest)](https://www.myget.org/feed/moryx/package/nuget/Moryx.TestTools.SystemTest) |
| `Moryx.TestTools.UnitTest` | [![NuGet](https://img.shields.io/nuget/v/Moryx.TestTools.UnitTest.svg)](https://www.nuget.org/packages/Moryx.TestTools.UnitTest/) | [![MyGet](https://img.shields.io/myget/moryx/vpre/Moryx.TestTools.UnitTest)](https://www.myget.org/feed/moryx/package/nuget/Moryx.TestTools.UnitTest) |

If you wish to contribute to this project, you simply need to clone the repository and open the solution with Visual Studio 2017 or above. The Debug target should be *StartProject*.

To build and run from the command line you need powershell and msbuild.

```powershell
.\Build.ps1 -Build
.\src\StartProject\bin\Debug\StartProject.exe
```

## Architecture

The MORYX Core is a .NET based framework to quickly build three-tier applications. The core architecture is a modular monolith using the service and facade pattern to isolate and decouple functionality. It uses a 2-level Dependency Injection structure to isolate a modules composition and offer a per-module life-cycle with all instances hidden behind the previously mentioned facades. It also offers a range of tools and components to speed up development, increase stability and drastically reduce boilerplate code. To improve flexibility of modules and applications the core has built in support for configuration management as well as plugin loading.

<p align="center">
    <img src="docs/images/arch_level1.png" width="400px"/>
</p>

Each modules composition is constructed by its own DI-container instance. This makes it possible to dispose the container in order to restart the module and reconstruct the composition with a different configuration or to recover from a fatal error. The `ModuleController` and `Facade` instances are preserved through the lifecycle of the application as part of the level 1 composition. The  Components (*always present*) and plugins (*configurable*) are created when a module is started and disposed when the module stops. For each lifecycle the references of the facade are updated.

<p align="center">
    <img src="docs/images/arch_level2.png" width="400px"/>
</p>

## Maintenance

Part of the Core is also the Maintenance module, which hosts a HTTP REST service and *optionally* a [graphic web interface](https://github.com/PHOENIXCONTACT/MORYX-MaintenanceWeb) to control and configure a MORYX application. The Maintenance itself does not define that logic, but simply provides easy external access to APIs and features of the runtime kernel.

## History

Starting with version 3.0 of the core we decided to open source it as a foundation for Industrial IoT (IIoT) applications. For this public version, the framework received an overhaul to replace commercial libraries and tools, remove specialized Phoenix Contact code and better comply with the .NET open source community. Because of these changes the public version is still *Work-in-Progress*, but we will stabilize and release it soon.

But even though this version is still under construction, its in-house predecessor has been used in production for years. Just a few examples of solutions build on MORYX are listed below:

- Manufacturing Control System
- Plastic Mold Tracking
- Intralogistics
- Home Automation
