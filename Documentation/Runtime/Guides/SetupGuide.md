Setup Guide {#runtime-setupGuide}
=======

This Tutorial explains how to setup a "Marvin Runtime Environment". 

# Runtime
## Setting up the "Marvin Runtime Environment"
First we will prepare the "Marvin Runtime Environment" for your project.

1. Create a project folder "_NameOfYourProject_"
1. Create a Folder named ".build" inside your project folder
2. Checkout the ".build" Folder from the repository into your local ".build" Folder (http://vcs-ms.europe.phoenixcontact.com/repo/marvin/.build/trunk).
3. Navigate into ".build/ProjectBase/"
4. Copy the content from ".build/ProjectBase/" to your project folder.
5. Rename the Solution Files "ProjectName.sln" and "before.ProjectName.sln.targets" to a name of your choice.
6. Download [RuntimeCore-Full-0.0.*-CI.mab](http://nts-eu-jenk02.europe.phoenixcontact.com:8080/job/MarvinPlatform-CI/) from Jenkins into your local bundle Folder. 
7. Download [ModelTemplates-0.0.*-CI.mlb](http://nts-eu-jenk02.europe.phoenixcontact.com:8080/job/MarvinPlatform-CI/) from Jenkins into your local bundle Folder.
8. Download [DataModelWizard-0.0.*-CI.vsix](http://nts-eu-jenk02.europe.phoenixcontact.com:8080/job/MarvinPlatform-CI/) from Jenkins into your local bundle Folder.
9. Execute ".build/InstallBundles.bat"

After Installation succeeded Your directory should look like this:

````
D:\MYPROJECT
|   before.ProjectName.sln.targets
|   ProjectName.sln
|   
+---.build
|   |   before.sln.targets
|   |   Build.proj
|   |   CleanBundleFiles.bat
|   |   CreateBundleFile.bat
|   |   CreateReadme.bat
|   |   CstfProcessor.exe
|   |   DefaultDoxyfile
|   |   InstallBundles.bat
|   |   MarvinCommon.targets
|   |   MarvinInstall.targets
|   |   MarvinPreBuild.targets
|   |   MSBuild.Community.Tasks.dll
|   |   MSBuild.Community.Tasks.targets
|   |   RunDoxygen.bat
|   |   RunOpenCover.bat
|   |   UninstallBundles.bat
|   |   
|   +---7za920 
|   +---Microsoft       
|   +---ProjectBase
|   +---Resharper
|   \---Test.Libs
+---Build
|   |   README.txt
|   +---EddieLight
|   +---RuntimeModes
|   \---ServiceRuntime
|       +---Backups
|       +---Bundles
|       +---Config
|       +---Core
|       +---Libraries
|       +---Models    
|       +---ModelSetups
|       +---ModulePlugins
|       \---Modules
+---bundles
|   |   DataModelWizard-0.0-1572.vsix
|   |   ModelTemplates-0.0.1572-CI.mlb
|   |   RuntimeCore-Full-0.0.1572-CI.mab  
|   \---Temp  
+---IntegrationTests
+---Libraries
|   +---PlatformToolkit
|   \---Templates
|       |   README.txt
|       \---Model
|               CreateIndexGenerator.tmpl
|               DateTriggers.tmpl
|               Marvin.Repo.tmpl
|               MarvinDbContext.tmpl
|               MarvinInheritance.tmpl
|               UniqueConstraint.tmpl
+---Models
+---Modules
+---StartProject
|   |   StartProject.csproj
|   \---Properties
+---SystemTests
\---Tests
````

## Install the Templates
In this step we will install Project Templates for Visual Studio to assist you creating DataModels. 

1. Execute "DataModelWizard-0-0-*.vsix" in the Bundles Folder. 
2. Follow the instructions of the installer.
3. Open Visual Studio.
4. Create a "New Project" .
5. Now you should find DataModel Template.

## First contact
Now it is time to get familiar with the initial structure of the Runtime. 

1. Start "Build\ServiceRuntime\HeartOfGold.exe" 
2. Wait for the Console to open. You will see a header containing the Runtime version and a lot of state change notifications. 

## Playing with the CLI
The Runtime developer console is your most basic interaction with the Runtime and it's Kernel. The Console provides access to Runtime functionalities. Here you can send start or stop commands to the Runtime. By entering help you can list all commands.

Enjoy!
