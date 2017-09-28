Windows Service Guide {#runtime-windowsServiceGuide}
=======


# Introduction
Like explained in the [Runmode](@ref runtime-runModes) section, the Runtime can be run as a Windows service. As such it can also declare dependencies on other services. Similar to the Runtimes server module behavior the operating system will ensure that a services dependencies are running before it is started. This applies to automatic start at system boot as well as starting the service manually. Because not all applications build or on the Runtime share the same dependencies or simply use a different version of postgres the dependencies were not hard coded into the service but are rather read from a text file. This tutorial will briefly explain how to use this file.

# Basic installation

If the application shall only be registered as a Windows service without any dependencies the process is really easy.
* Validate name: Type "mname" into the console and make sure your application is named appropriatly.
* Set name: If the displayed name does not match your application, e.g. "MarvinRuntime" edit the 'Config\Marvin.Runtime.ProductConfig.mcf' file.
* Install: Type install into the console and wait for the process to finish.

# Installation with dependencies
If your application has dependencies the process is a little more sophisticated.

## File structure
The file used to read the service dependencies is the Dependencies.txt. Its structure is as simple as a text file can be. Comments must start with '#' and each non-comment line is considered a service name. A sample file might look like this:

````
# Dependencies file
# --------------------------
# This file is used by the windows service installer to determine all runtime dependencies
# of this Marvin RuntimeCore application.
postgresql-x64-9.1
````

In this case the only dependency is the PostgreSQL database service. Other services could be OPC-Servers or 3rd Party applications.

## How To
This little HowTo will explain how to create a Dependencies.txt file and how to apply this dependencies.

1. Dependency name: Open the computer management of the target system and look for the service you want to declare dependency on. In our example we will use PostgreSQL. Open it by double-click and select the name at the top. 

![](images/Runtime/Dependency.png)

2. Add to file: Copy the selected name and paste it at the end of your Dependencies.txt file. It shall now look like the example from the previous section. 

3. Repeat: Repeat the previous steps until your Dependencies.txt contains all of your applications dependencies. 

4. Install service: Now install the Runtime by typing install into the console. Make sure it is not allready installed and that you assigned a meaningful name and version in the 'Config\Marvin.Runtime.ProductConfig.mcf' file. you should now see the following output. 

![](images/Runtime/InstallCommand.png)

5. Validate: Make sure everything worked by looking for your application in the computer management. Double-clicking your application should list dependencies like this. 

![](images/Runtime/DependencyResult.png)

