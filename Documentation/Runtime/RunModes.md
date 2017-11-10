RunModes {#runtime-runModes}
========

The Runmode interface specifies access to the module manager and two methods. The ModuleManager is required to interact with the ServerModules. The Setup-method is invoked directly after loading the Runmode and before initializing the kernel. This gives Runmodes a chance to override some kernel configurations before the components are booted. After initializing the kernel the Run-method is invoked. Control over the application is now given to the run mode. Generally Runmodes start by invoking Start on the ModuleManager.

[IRunmode](@ref Marvin.Runtime.HeartOfGold.IRunmode)

# Implementation
If you would like to write a Runmode yourself all you have to do is implement the interface and decorate your class with the RunmodeAttribte. A simple implementation might look like this:

````cs
[Runmode("Dummy")]
public class DemoRunMode : IRunMode
{
    ///
    public IModuleManager ModuleManager { get; set; }
    
    ///
    public void Setup(RuntimeArguments args)
    {
        // Parse runtime or custom arguments
    }
    
    ///
    public int Run()
    {
        ModuleManager.Start();
        Thread.Sleep(100000);
        ModuleManager.Stop();
        
        return 0;
    }
}
````

# Available Runmodes#
Current 4 Runmodes are implemented. 2 of those are directly build into the HeartOfGold and 2 are placed in a seperate library. Some of this define arguments that can be passed to the Runtime in the form "-{Argument}={Parameter}".

## Console
This is the default Runmode for development and application debugging. It offers a simple command line interface to interact with modules and the kernel. Whenever a module name shall be entered the console provide auto complete by entering the fist few letters of the module name and pressing Tab. A list of available commands can be obtained by entering help and pressing enter. Some commands like 2 wonderful easter eggs are not included in this list. Those you must figure out yourself. Useful commands are:

| Input | Effect |
|-------|--------|
| start  | Starts all modules in their order of dependency |
| start *module* | Starts the module with the given name |
| stop | Stops all modules |
| stop *module* | Stops the module with this name |
| clear | Clears the current window and displays the header and all modules|
| gc collect | Enforces garbarge collection|
| print *module* -e | Displays the execption that caused a module to crash |
| print *moudle* -d | Displays a modules dependencies and their state |
| install | Installs the currently executed application as a windows service |

Arguments: The developer console does not define any custom arguments. It does not read any arguments either.

## WinService
This Runmode is used to execute the Runtime fully headless as windows service. It does not require a logged in user and is usually run with the highest privileges. This is default Runmode in productive use. Once an application seems to run stable on the target system simply type "install" into the console. From now on your application will be executed directly on system startup.
Like any other Windows Service it can also declare dependencies. Because every application build on the Runtime has different dependencies they were not hard coded into this Runmode. When the install command is run it will read the Dependencies.txt file and interpret each line without '#' as a dependency name. Those names are passed to the operating system as dependencies. Refer to WinServiceDependencies for a tutorial.
Arguments: The service Runmode does not read or define any arguments.

Sample Dependencies.txt:

````txt
# Dependencies file
# --------------------------
# This file is used by the windows service installer to determine all runtime dependencies
# of this RuntimeCore application.
postgresql-x64-9.4
````

## Smoketest
As the name suggests this Runmode is used to perform a simple smoke test of the system. This means all modules are started, restarted and stopped once. The smoke test also checks the number of executed modules to make sure no module library was skipped due to a TypeLoadException or similar. It will also monitor the time it takes modules to transition from one state to another. If this time exceeds a given interval the smoke test fails as well.

**Arguments:**

| Argument | Parameter | Default | Effect |
|----------|-----------|---------|--------|
| -e | ModuleCount | 3 |Defines the expected number of modules. If more or less modules are detected the test fails.|
| -f | | false | Instructs the Runmode to perform a full test including exhaustive restarts. By default each module is only started and stopped. |
| -pi | PortIncrement | 0 | Defines an offset added to all ports. This is used on the build system to enable simultaneous execution of smoke tests. | 
| -i | IntervallMS | 60 000 | Time interval in milliseconds between module state changes before the smoke test detects a time out. | 

## SystemTest
As the name suggests this Runmode is used to perform a system tests. Traditionally the Console was used for this but sometimes the unit test would crash an the process would remain alive on the build system blocking all other builds. The SystemTest Runmode overcomes this deficit by implementing a timeout to kill the process if it does not receive a shutdown command. The textual interface was also redirected to telnet for low level remote access.

**Arguments:**

| Argument | Parameter | Default | Effect |
|----------|-----------|---------|--------|
| -pi | PortIncrement | 0 | Defines an offset added to all ports. This is used on the build system to enable simultaneous execution of smoke tests. | 
| -p | TelnetPort | 23 | Port used for telnet connections. |
| -t | TimeoutSec | 300 | Timeout for the system test in seconds | 