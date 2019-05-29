# MarvinPlatform

[![pipeline status](http://gitlab-swtd.europe.phoenixcontact.com/marvinplatform/MarvinPlatform/badges/master/pipeline.svg)](http://gitlab-swtd.europe.phoenixcontact.com/marvinplatform/MarvinPlatform/commits/master)
[![coverage report](http://gitlab-swtd.europe.phoenixcontact.com/marvinplatform/MarvinPlatform/badges/master/coverage.svg)](http://gitlab-swtd.europe.phoenixcontact.com/marvinplatform/MarvinPlatform/commits/master)

Welcome to the MarvinPlatform! It is the foundation of the MARVIN ecosystem and base-layer its numerous layered applications. It contains the core namespace, domain independent tools and the MARVIN.Runtime application server.

# Getting Started

## Visual Studio

To build and debug the project you need Visual Studio 2017 and above. Simply clone the repository and open then *MarvinPlatform.sln*. The initial debug target should be *StartProject*, so you can just start the debugger. Fetching packages and building should happen automatically

## Command Line

To build and run from the command line you need powershell and msbuild.

```powershell
.\Build.ps1 -Build
.\src\StartProject\bin\Debug\StartProject.exe
```


