# Migration from MORYX Framework v6 to v8


## Local Container Refactoring
The DI container within modules based on Castle Windsor was refactored and simplified. The most changes are caused by removing the historic split between local and global container, which was obsolete after switching the global container to ServiceCollection. We also removed the concept of installers and registrators and replaced everything with API on `IContainer` and extensions inspired by the `IServiceCollection`. 

- **Attribute changes:** The base attributes for registration were removed, use `ComponentAttribute`, `PluginAttribute` and `PluginFactory` instead.
- **Installers removed** The concept of installes was removed and with it their implementations `AutoInstaller` and `DependencyInstaller`. They were replaced by the extensions `LoadFromAssembly` with different signature options for `DependencyRegistrationAttribute` and `Predicate<Type>`
- **LoadComponents** was removed as a dedicate feature and is now an extension on `IContainer`.
- **IContainerHost** was removed. The seperate interface for accessing a modules container just caused unnecessary casts and the risk of invalid type. The property `Container` was added to `IServerModule` instead.
- **Extend** The flexible method for passing facilities to Castle was removed as it was only added and used by WCF.
- **MoryxFacility** All MORYX specific behavior like strategies and `Named` import overrides were refactored to follow Castle best practises and are now isolated in the `MoryxFacility`. This enables everyone to achieve the MORYX DI behavior with a Castle Container without the MORYX wrapper.
- **Installers** As previously mentioned installers were removed, but since the API on `IContainer` now supports everything previously reserved for installers and registrators, just migrate the registration onto the container like the [DbContextContainerExtension](https://github.com/PHOENIXCONTACT/MORYX-Framework/blob/future/src/Moryx.Model/DbContextContainerExtension.cs) or the [BasicInterceptorInstaller](https://github.com/PHOENIXCONTACT/MORYX-Framework/blob/future/src/Moryx.TestTools.UnitTest/BasicInterceptorInstaller.cs)

## ServerModuleBase

To simplify development and prepare easier integration of the Moryx.Cli we merged the `ServerModuleFacadeControllerBase` into the `ServerModuleBase`. Just replace the base type if your module is affected by this.