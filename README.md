# MARVIN Platform

![CI](https://github.com/dbeuchler/MarvinPlatform/workflows/CI/badge.svg)
[![codecov](https://codecov.io/gh/dbeuchler/MarvinPlatform/branch/dev/graph/badge.svg?token=BZUCXPUNHU)](https://codecov.io/gh/dbeuchler/MarvinPlatform)

The MARVIN Platform is a .NET based framework to quickly build three-tier applications. It aims to reduce boilerplate code as much as possible and provide modularity, flexibility and easy configuration with very little effort. It is also the foundation for the Phoenix Contact IoT Framework [MARVIN Abstraction Layer](https://git-ctvc.europe.phoenixcontact.com/marvin/AbstractionLayer)

**Links**
- [Package Feed](https://packages-ctvc.europe.phoenixcontact.com/nuget/MaRVIN-CI)
- [Training Repository](https://git-ctvc.europe.phoenixcontact.com/marvin/trainingrepo)
- [Application Skeleton](https://git-ctvc.europe.phoenixcontact.com/marvin/ApplicationSkeleton)
- [MARVIN Maintenance](https://git-ctvc.europe.phoenixcontact.com/marvin/maintenanceweb)
- [MARVIN Abstraction Layer](https://git-ctvc.europe.phoenixcontact.com/marvin/AbstractionLayer)

## Getting started

If you want to start developing with or for MARVIN, the easiest way is our [application skeleton repository](https://git-ctvc.europe.phoenixcontact.com/marvin/ApplicationSkeleton). It comes with two empty solutions, the necessary package feeds and preinstalled empty MARVIN runtime. Add projects and packages to backend and frontend solutions depending on your specific requirements.

If you wish to contribute to this project, you simply need to clone the repository and open the solution with Visual Studio 2017 or above. The Debug target should be *StartProject*. 

To build and run from the command line you need powershell and msbuild.

```powershell
.\Build.ps1 -Build
.\src\StartProject\bin\Debug\StartProject.exe
```

## Architecture

The MARVIN Platform is a .NET based framework to quickly build three-tier applications. The core architecture is a modular monolith using the service and facade pattern to isolate and decouple functionality. It uses a 2-level Dependency Injection structure to isolate a modules composition and offer a per-module life-cycle with all instances hidden behind the previously mentioned facades. It also offers a range of tools and components to speed up development, increase stability and drastically reduce bioler plate code. To improve flexibility of modules and applications the platform has built in support for configuration management as well as plugin loading.

<img src="docs/images/arch_level1.png" width="400px"/>

Each modules composition is constructed by its own DI-container instance. This makes it possible to dispose the container in order to restart the module and reconstruct the composition with a different configuration or to recover from a fatal error. The `ModuleController` and `Facade` instances are preserved through the lifecycle of the application as part of the level 1 composition. The  Components (*always present*) and plugins (*configurable*) are created when a module is started and disposed when the module stops. For each lifecycle the references of the facade are updated.

<img src="docs/images/arch_level2.png" width="400px"/>

## Maintenance

Part of the Platform is als the Maintenance Module, which hosts a HTTP REST service and *optionally* a [graphic web interface](https://git-ctvc.europe.phoenixcontact.com/marvin/maintenanceweb) to control and configure a MARVIN application. The Maintenance itself does not define that logic, but simply provides easy external access to APIs and features of the Platforms kernel. 

## History

Starting with version 3.0 of the platform we decided to open source it as a foundation for Industrial IoT (IIoT) applications. For this public version, the framework received an overhaul to replace commercial libraries and tools, remove specialized Phoenix Contact code and better comply with the .NET open source community. Because of these changes the public version is still *Work-in-Progress*, but we will stabilize and release it soon.

But even though this version is still under construction, its in-house predecessor has been used in production for years. Just a few examples of solutions build on MARVIN are listed below:
- [Manufacturing Control System](https://git-ctvc.europe.phoenixcontact.com/marvin/ControlSystem)
- Plasic Mold Tracking
- Intralogistics
- Home Automation