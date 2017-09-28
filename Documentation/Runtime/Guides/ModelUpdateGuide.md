DataModel Update Guide {#runtime-dataModelUpdateGuide}
=====

# How to update a database
To update your database do the following steps in that order.

## Update your runtime
To update your runtime go to  
* [Marvin Platform](http://nts-eu-jenk02.europe.phoenixcontact.com:8080/job/MarvinPlatform-CI/) developer build
* Select the latest release build and load the needed runtime files (runtime core, runtime full etc.) and save the bundle in your bundle folder. 
* Run .build/InstallBundels.bat!

## Regenerate you model
Regenerate your database model to have the newest version available. 

## Create database
Start your application and create the databases. 
The Metadatatable will be checked or created automatically.

![](images/Runtime/metadataTable.png)


## Develop your application

## Increase the database version
In the way of developing it is possible to change the database. If that happens, then increase the version number of your database. Do this in the model before regenerating it.

![](images/Runtime/Marvin.RepoVersion.png)

## Create db update scripts
Open the model in entity developer, do your changes and then follow the wizard steps below.

| Six steps to create update scripts |
|-------|---------|--------------|
| ![](images/Runtime/EntityDeveloperUpdate1.png) | ![](images/Runtime/EntityDeveloperUpdate2.png) | ![](images/Runtime/EntityDeveloperUpdate3.png) |
| ![](images/Runtime/EntityDeveloperUpdate4.png) | ![](images/Runtime/EntityDeveloperUpdate5.png) | ![](images/Runtime/EntityDeveloperUpdate6.png) |


Copy the script text from picture 6 into the sql file and change the script if necessary.

## Create new script files

| Three steps to create new script files|
|-------|---------|--------------|
| ![](images/Runtime/UpdateFolder.png) | ![](images/Runtime/NewUpdateScript.png) | ![](images/Runtime/ExampleScriptFile.png) |


This is an example how it should look like. Name the script like in the example from which version to which version this script should work. Example: **From1to2**. Also add this values in the **From** and **To** property. 
ExecuteScript accepts a sql file which must be placed in the updates folder of the model and must be marked as **EmbeddedResource**.

## Execute updates
To execute the updates on the database, start the runtime with the argumet
* **-dbUpdate**
This must be done only once until a new update of the database is available. It will update all available databases when it finds a new version for it.

 ![](images/Runtime/ExampleOfUpdate.png)

## Check for success
To check if the updates was a success, open the metadata table and check the version number in the version colum of your model.

![](images/Runtime/UpdatedMetadata.png)



