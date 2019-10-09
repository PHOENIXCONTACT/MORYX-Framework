---
uid: RunModes
---
# RunModes

The RunMode interface specifies access to the module manager and two methods. The `ModuleManager` is required to interact with the `ServerModules`. The `Setup`-method is invoked directly after loading the RunMode and before initializing the kernel. This gives RunMode a chance to override some kernel configurations before the components are booted. After initializing the kernel the `Run`-method is invoked. Control over the application is now given to the run mode. Generally RunMode start by invoking Start on the ModuleManager.

[IRunMode](xref:Marvin.Runtime.Kernel.IRunMode)

## Implementation

If you would like to write a RunMode yourself all you have to do is implement the interface `IRunMode` or better the base class `RunModeBase<TOptions>` and decorate your class with the RunModeAttribte.
The options class will be used by the [CommandLineParser](https://github.com/commandlineparser/commandline) to parse all arguments.

A simple implementation might look like this:

````cs
[Verb("demo")]
public class DemoRunModeOptions : RuntimeOptions
{
    [Option('s', "sleepTime", Default = 2000, HelpText = "Defines the time how long the runtime should run")]
    public int SleepTime { get; set; }
}

[RunMode(typeof(DemoRunModeOptions))]
public class DemoRunMode : RunModeBase<DemoRunModeOptions>
{
    public override void Setup(RuntimeOptions args)
    {
        base.Setup(args);
    }

    public RuntimeErrorCode Run()
    {
        ModuleManager.Start();

        Thread.Sleep(Options.SleepTime);

        ModuleManager.Stop();

        return RuntimeErrorCode.NoError;
    }
}
````

The sample could be started with:

````ps1
$> .\Application.exe demo --sleepTime 10000
````

## Available RunModes

Current 4 Runmodes are implemented. 2 of those are directly build into the HeartOfGold and 2 are placed in a seperate library. Some of this define arguments that can be passed to the Runtime in the form "-{Argument}={Parameter}".

### Developer Console

This is the default RunMode for development and application debugging. It offers a simple command line interface to interact with modules and the kernel.

Possible parameter options for this RunMode can be printed by `.\Application.exe dev --help`. Here are some sample:

````ps1
// Starts the developer console with a custom config directory
$> .\Application.exe dev --c C:\YourApp\Config

// Starts the developer console with the default config directory
$> .\Application.exe
````

Whenever a module name shall be entered the console provide auto complete by entering the fist few letters of the module name and pressing `Tab`. A list of available commands can be obtained by entering `help` and pressing enter. Some commands like 2 wonderful easter eggs are not included in this list. Those you must figure out yourself. Useful commands are:

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

Arguments: The developer console does not define any custom arguments. It does not read any arguments either.

### WinService

This RunMode is used to execute the Runtime fully headless as windows service. It does not require a logged in user. This should be the default RunMode in productive use. From now on your application will be executed directly on system startup.
Like any other Windows Service it can also declare dependencies. Because every application build on the Runtime has different dependencies they were not hard coded into this RunMode. When the install command is run there is also a parameter for service dependencies. Those names are passed to the operating system as dependencies. Refer to WinServiceDependencies for a tutorial.

Possible parameter options for this RunMode can be printed by `.\Application.exe service --help`. Here are some sample:

````ps1
// Default service installation under local system
$> .\Application.exe service --install --c C:\YourApp\Config

// User based service installation
$> .\Application.exe --install --c C:\YourApp\Config --dependencies postgresql-x64-11 --security User --user domain\JohnDoe --password secret

// Uninstall the service
$> .\Application.exe --uninstall
````

### SmokeTest

As the name suggests this RunMode is used to perform a simple smoke test of the system. This means all modules are started, restarted and stopped once. The smoke test also checks the number of executed modules to make sure no module library was skipped due to a TypeLoadException or similar. It will also monitor the time it takes modules to transition from one state to another. If this time exceeds a given interval the smoke test fails as well.

Possible parameter options for this RunMode can be printed by `.\Application.exe smokeTest --help`. Here are some sample:

````ps1
// Runs the SmokeTest with 5 expected modules and an port increment of 4711
$> .\Application.exe smokeTest --expected 5 --portIncrement 4711
````


### SystemTest

As the name suggests this RunMode is used to perform a system tests. Traditionally the console was used for this but sometimes the unit test would crash an the process would remain alive on the build system blocking all other builds. The SystemTest RunMode overcomes this deficit by implementing a timeout to kill the process if it does not receive a shutdown command. The textual interface was also redirected to telnet for low level remote access.

Possible parameter options for this RunMode can be printed by `.\Application.exe systemTest --help`. Here are some sample:

````ps1
// Runs the runtime in system test mode with an port increment of 4711 and a custom config folder
$> .\Application.exe systemTest --portIncrement 4711 --configDir C:\YourApp\Config
````
