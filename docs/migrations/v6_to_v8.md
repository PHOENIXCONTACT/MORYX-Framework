# Migration from MORYX Framework v6 to v8

## Local Container Refactoring
The DI container within modules based on Castle Windsor was refactored and simplified. The most changes are caused by removing the historic split between local and global container, which was obsolete after switching the global container to ServiceCollection. We also removed the concept of installers and registrators and replaced everything with API on `IContainer` and extensions inspired by the `IServiceCollection`. 

- **Attribute changes:** The base attributes for registration were removed, use `ComponentAttribute`, `PluginAttribute` and `PluginFactory` instead.
- **Installers removed** The concept of installes was removed and with it their implementations `AutoInstaller` and `DependencyInstaller`. They were replaced by the extensions `LoadFromAssembly` with different signature options for `DependencyRegistrationAttribute` and `Predicate<Type>`
- **LoadComponents** was removed as a dedicate feature and is now an extension on `IContainer`. It has also been restricted to public/exported types.
- **IContainerHost** was removed. The seperate interface for accessing a modules container just caused unnecessary casts and the risk of invalid type. The property `Container` was added to `IServerModule` instead.
- **Extend** The flexible method for passing facilities to Castle was removed as it was only added and used by WCF.
- **MoryxFacility** All MORYX specific behavior like strategies and `Named` import overrides were refactored to follow Castle best practises and are now isolated in the `MoryxFacility`. This enables everyone to achieve the MORYX DI behavior with a Castle Container without the MORYX wrapper.
- **Installers** As previously mentioned installers were removed, but since the API on `IContainer` now supports everything previously reserved for installers and registrators, just migrate the registration onto the container like the [DbContextContainerExtension](https://github.com/PHOENIXCONTACT/MORYX-Framework/blob/future/src/Moryx.Model/DbContextContainerExtension.cs) or the [BasicInterceptorInstaller](https://github.com/PHOENIXCONTACT/MORYX-Framework/blob/future/src/Moryx.TestTools.UnitTest/BasicInterceptorInstaller.cs)

## ServerModuleBase

To simplify development and prepare easier integration of the Moryx.Cli we merged the `ServerModuleFacadeControllerBase` into the `ServerModuleBase`. Just replace the base type if your module is affected by this.

## GetObjectData(SerializationInfo info, StreamingContext context)

Removed all overrides of the obsolete method `Exception.GetObjectData(SerializationInfo info, StreamingContext context)` as well as all constructors which were calling the base class constructor `Exception(SerializationInfo info, StreamingContext context)`
The following classes are affected by this change
- MissingFacadeException
- HealthStateException
- InvalidConfigException

## Merged IPublicResource into IResource

`IPublicResource` and `IResource` were merged into `IResource`, since the differentiaten between those was hard to understand for some and barely had any real world advantages. Now literally "Everything is a resource".

# Factory 6.x to 8.x

## Merged `ISetupTrigger` and `IMultiSetupTrigger` 
We already offered the possibility to create a list of setup tasks in a setup trigger. The API of the setup trigger was now merged and cleaned up.
- `ISetupTrigger.CreateStep(IProductionRecipe recipe` now returns `IReadOnlyList<IWorkplanStep>`. Issues could for example be resolved as follows:
```c#
/// MORYX 6
public IWorkplanStep CreateSteps(IProductRecipe recipe) {
...
return step;
}

/// MORYX 8
public IReadOnlyList<IWorkplanStep> CreateSteps(IProductRecipe recipe) {
...
return [...step];
}
```

## VisualInstruction API improvements
### IInstructionResults was removed
- The interface just made the instructions more complex and had no practical use  
*Note: `ActiveInstruction.PossibleResults` has before and is still providing the possible results of an instruction as strings*
  
### IVisualInstructor API was changed to improve extendability
- Interface methods now take an `ActiveInstruction` parameter instead of multiple seperate parameters
- You can create an `ActiveInstruction` using the information previously given to the instructor or keep using the extension methods
- Most previous method signature are still available via `VisualInstructorExtensions`
- Overloads taking `IInstructionResults` were removed. If you used them with `EnumInstructionResults` read along.
- Overloads taking `Action<int, ActivityStart>` callbacks were removed. If you used them read along.

### EnumInstructionResult was replaced with a static type providing conversion methods
- The type itself is not used anymore due to the changes mentioned above
- If you used a method on an `IVisualInstructor` providing an `EnumInstructionResult` you can use the new static methods on this type to get the possible results as strings
```c#
// MORYX 6
VisualInstructor.Execute("Some Title", someInstructions, new EnumInstructionResult(typeof(AskRotationPermissionResult), SomeAction));

// MORYX 8
VisualInstructor.Execute("Some Title", someInstructions, EnumInstructionResult.PossibleResults(typeof(AskRotationPermissionResult)),
  param => SomeAction(EnumInstructionResult.ResultToEnumValue(typeof(ArticleMountingStrategy), param.Result)));
```

### EnumInstructionAttribute.Title and the corresponding constructor were removed
- To set the display name of an enum value please use the default data annotation `[Display(Name = "Value's name")]`

# ControlSystem 6.x to 8.x

## MORYX.Launcher 
### IShellNavigator now requires an HttpContext
- `IShellNavigatorV8` was merged into `IShellNavigator`. The interface now only defines one `GetWebModuleItems` which requires a parameters

### Localization class was removed
- MORYX now solely relies on the default Aspn.Net way of adding localization to the application. For a very concise tutorial take a look in the [documentation](https://git-ctvc.europe.phoenixcontact.com/moryx/Home/-/blob/dev/tutorials/addLanguageSupport.md)
**ToDo: Update documentation in MORYX Home**

### Standard .AspNetCore.Culture cookie replaces custom moryx-language cookie
- The selected language the shell and the angular modules use is now extracted from the standard .Net cookie for localization

### MORYX now longer adds the default languages 'DE', 'EN' and 'IT' to the language selection dropdown
- Only languages setup in the `Program.cs` wil appear in the dropdown. If a language is not provided by a module it defaults to english

### Setting AuthUrl and IdentityUrl in the appsettings.json now longer activates the login button
- The two solutions were replaces by the usage of `MoryxIdentityOptions`. For more information on how to setup you application with an IAM reference the [documentation](https://git-ctvc.europe.phoenixcontact.com/moryx/MORYX-AccessManagement/-/blob/dev/docs/articles/HowToIntegrateInYourApplication.md)

### Severeal ID changes in the DOM
- `settingDropDown` -> `setting-drop-down`
- `languagesDropDown` -> `languages-drop-down`

## Moryx.VisualInstructions
Beside the API changes you will find in [MORYX Factory](), which will of course also reflect in the module, there are the following changes
### EmptyInstructionResult was removed
- There is now use for it anymore after the API changes
