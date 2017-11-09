Platform 2.7 to 3.0 Upgrade Guide {#platform27to30}
====================================

When you read this, you are at the point when you are interested to upgrade your Platform 2.7 to 3.0.
Let me say first, that it will be a complex task for the next hours. 

# .NET Framework 4.6.1
To stay with the future, we decided to move to .NET Framework 4.6.1. The version is compatible with .NET Standard 2.0 to later move to .NET Core.
All projects have to be adjusted to the new framework version.

# Flat Folder Structure and Copy-Local
You know the days where you have to change the `CopyLocal` to `false` on every single reference you add to an assembly? Im glad to tell you that you can forget this creepy thing! In Platform 3 you do not have to do it anymore. The complete folder structure of the ServiceRuntime is pressed flat. You cannot find such folders like `Modules`, `ModulePlugins`, `Models` or `Bundles` anymore. Every assembly will be build in the root ServiceRuntime/ClientRuntime directory.

**What you have to do:** 
- Change the Build-Path of every assembly of your project to Build\\ServiceRuntime\\
  - Do this for the Build-Target Debug and Release
- Change all references of your assemblies from `CopyLocal` `false` to `true`
  - Be carefull: You should only copy local assemblies which are not located in the global assembly cache
- Remove `PluginDir` or similar properties from your configuration classes of modules.
  - The `LoadComponents`-Method of the container provides an overload without a folder 
  - All assemblies will be loaded from the app domain

# IContainer API
The [IContainer](@ref Marvin.Container.IContainer) API changed. The `IContainer.LoadComponents(directory)` is not available and not supported anymore. Use the `IContainer.LoadComponents()` instead. 
The platform is not distinguishing anymore between subfolders.

The `IContainer.SetInstances(obj, obj, obj, ...)` is not available anymore. The method now supports fluent registration:
````cs
Container.SetInstance(ComponentA).SetInstance(ComponentB).SetInstance(ComponentC);
````

# IModulePlugin
The `IModulePlugin` was renamed to `IPlugin` and doesnt inherit from `IDisposable` anymore. In older versions of the Platform it was always a problem of automatic disposable of components. The  `IDisposable` is an optional extension of a component now and `IPlugin` have an additional method `Stop()`. It is neccessary to distinguish between `Stop()` and `Dispose` now. 

# Upgraded Libraries
To not become old and gray, all third party libraries were upgraded:

## Common Logging
The common logging framework was updated from 2.1 to 3.3.1. No relevant api changes known.

## Caliburn.Micro
Caliburn was upgraded from 2.0.1 to 3.0.1. No known problems.

## JSON.NET (Newtonsoft JSON)
The JSON Library was updated to the newest version. With Platform 3 generally you need no Newtonsoft.Json reference anymore. The platform wrappes the json serializer. Have a look to the [Json](@ref Marvin.Serialization.Json) class. The class should provide all needed features for the general development with the platform.

The known static class `JsonSettings` is not existing anymore. The new [Json](@ref Marvin.Serialization.Json) class will provide several options. For more details, please read [Json Serialization](@ref platform-jsonUsage)

## NUnit
For our unit tests the Nunit framework was updated from 2.4.x to 3.8.0.

Some help:
- TestCaseAttribute
  - Property Result is now ExpectedResult
  - ExpectedException is not existing anymore. Think about your test case. The test should only test a good path or a bad path. Use the `Assert.Throws<>`-Method for the bad path through your code.
- TestFixtureSetUpAttribute is obsolete
  - Use `OneTimeSetupAttribute`
- TestFixtureTearDownAttribute is obsolete
  - Use `OneTimeTearDownAttribute`