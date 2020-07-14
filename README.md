
<p align="center">
    <img src="docs/resources/MORYX_logo.svg" alt="MORYX Logo" width="300px" />
</p>

<p align="center">
    <a href="https://github.com/PHOENIXCONTACT/MORYX-Platform/workflows">
        <img src="https://github.com/PHOENIXCONTACT/MORYX-Platform/workflows/CI/badge.svg" alt="CI">
    </a>
    <a href="https://www.myget.org/feed/Packages/moryx">
        <img src="https://img.shields.io/myget/moryx/v/Moryx" alt="MyGet">
    </a>
    <a href="https://github.com/PHOENIXCONTACT/MORYX-Platform/blob/dev/LICENSE">
        <img src="https://img.shields.io/github/license/PHOENIXCONTACT/MORYX-Platform" alt="License">
    </a>
    <a href="https://github.com/PHOENIXCONTACT/MORYX-Platform/pulls">
        <img src="https://img.shields.io/github/issues-pr/PHOENIXCONTACT/MORYX-Platform" alt="GitHub pull requests">
    </a>
    <a href="https://github.com/PHOENIXCONTACT/MORYX-Platform/issues">
        <img src="https://img.shields.io/github/issues/PHOENIXCONTACT/MORYX-Platform" alt="GitHub issues">
    </a>
    <a href="https://github.com/PHOENIXCONTACT/MORYX-Platform/graphs/contributors">
        <img src="https://img.shields.io/github/contributors-anon/PHOENIXCONTACT/MORYX-Platform" alt="GitHub contributors">
    </a>
</p>

# MORYX Platform

The MORYX Platform is a .NET based framework to quickly build three-tier applications. It aims to reduce boilerplate code as much as possible and provides modularity, flexibility and easy configuration with very little effort. It is also the foundation for the Phoenix Contact IoT Framework [MORYX Abstraction Layer](https://github.com/PHOENIXCONTACT/MORYX-AbstractionLayer)

**Links**

- [Package Feed](https://www.myget.org/feed/Packages/moryx)
- [Repository Template](https://github.com/PHOENIXCONTACT/MORYX-Template)
- [MORYX Maintenance](https://github.com/PHOENIXCONTACT/MORYX-MaintenanceWeb)
- [MORYX ClientFramework](https://github.com/PHOENIXCONTACT/MORYX-ClientFramework)
- [MORYX Abstraction Layer](https://github.com/PHOENIXCONTACT/MORYX-AbstractionLayer)

## Getting started

If you want to start developing with or for MORYX, the easiest way is our [template repository](https://github.com/PHOENIXCONTACT/MORYX-Template). It comes with two empty solutions, the necessary package feeds and preinstalled empty MORYX runtime. Add projects and packages to backend and frontend solutions depending on your specific requirements.

If you wish to contribute to this project, you simply need to clone the repository and open the solution with Visual Studio 2017 or above. The Debug target should be *StartProject*.

To build and run from the command line you need powershell and msbuild.

```powershell
.\Build.ps1 -Build
.\src\StartProject\bin\Debug\StartProject.exe
```

## Architecture

The MORYX Platform is a .NET based framework to quickly build three-tier applications. The core architecture is a modular monolith using the service and facade pattern to isolate and decouple functionality. It uses a 2-level Dependency Injection structure to isolate a modules composition and offer a per-module life-cycle with all instances hidden behind the previously mentioned facades. It also offers a range of tools and components to speed up development, increase stability and drastically reduce boilerplate code. To improve flexibility of modules and applications the platform has built in support for configuration management as well as plugin loading.

<p align="center">
    <img src="docs/images/arch_level1.png" width="400px"/>
</p>

Each modules composition is constructed by its own DI-container instance. This makes it possible to dispose the container in order to restart the module and reconstruct the composition with a different configuration or to recover from a fatal error. The `ModuleController` and `Facade` instances are preserved through the lifecycle of the application as part of the level 1 composition. The  Components (*always present*) and plugins (*configurable*) are created when a module is started and disposed when the module stops. For each lifecycle the references of the facade are updated.

<p align="center">
    <img src="docs/images/arch_level2.png" width="400px"/>
</p>

## Maintenance

Part of the Platform is also the Maintenance module, which hosts a HTTP REST service and *optionally* a [graphic web interface](https://github.com/PHOENIXCONTACT/MORYX-MaintenanceWeb) to control and configure a MORYX application. The Maintenance itself does not define that logic, but simply provides easy external access to APIs and features of the Platforms kernel.

## History

Starting with version 3.0 of the platform we decided to open source it as a foundation for Industrial IoT (IIoT) applications. For this public version, the framework received an overhaul to replace commercial libraries and tools, remove specialized Phoenix Contact code and better comply with the .NET open source community. Because of these changes the public version is still *Work-in-Progress*, but we will stabilize and release it soon.

But even though this version is still under construction, its in-house predecessor has been used in production for years. Just a few examples of solutions build on MORYX are listed below:

- Manufacturing Control System
- Plastic Mold Tracking
- Intralogistics
- Home Automation