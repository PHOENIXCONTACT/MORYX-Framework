---
uid: RuntimeMain
---
# Runtime

The Moryx Runtime is an application framework for the server part of a client-server application. It is based on the concept of modular software development and the service pattern. The frameworks main features are listed below:

[HeartOfGold](xref:HeartOfGold)

The framework offers a runtime application called "HeartOfGold" that offers different execution modes. One of the build-in RunModes is a Windows service to be used for final deployment and the other is a console application to be used as an emulator or debugging tool. Additional RunModes for testing are available as well.

[LifeCycle](xref:LifeCycle)

The module interface and framework components provide full lifecycle management from component creation to component destruction.

[ComponentComposition](xref:ComponentComposition)

The Moryx Runtime defines two levels of components. The top level or main components include server modules, framework components and datamodels and is composed using a Moryx enhanced CastleWindsor DI-Container. The second level or plugin level is composed using a level 2 optimized Moryx-Castle-Container.

[DataModels](xref:DataModels)

Even though the framework is fully functional without any kind of database at all, it offers support for using multiple datamodels at the same time. These features include, but are not limited to, database creation, configuration and backup.

## Further

* [RunModes](xref:RunModes)
* [Advance](xref:Advance)

## Guides

* [ServerModuleGuide](xref:ServerModuleGuide)
* [FacadeGuide](xref:FacadeGuide)
* [ModelSetupGuide](xref:ModelSetupGuide)
* [WindowsServiceGuide](xref:WindowsServiceGuide)
* [DataModelUpdateGuide](xref:DataModelUpdateGuide)
* [SetupGuide](xref:SetupGuide)

## Provided Modules

* [UserManagement](xref:UserManagement)

## Tools & Templates

To make working with the Moryx Runtime more efficient, there are a couple of tools and templates that speed up plugin and component development significantly. Listed below are only Moryx specific tools and templates. 3rd party tools and libraries used for Moryx development can be found elsewhere.

DataModel-Template: This template can only be used with an EntityDeveloper DataModel.
