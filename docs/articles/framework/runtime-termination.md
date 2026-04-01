---
uid: RuntimeTermination
---
# Runtime Termination

## Description

A MORYX based application is constructed as a [WebApplication](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.webapplication) in the following way:

```cs
var builder = WebApplication.CreateBuilder();
...
var app = builder.Build();

var moduleManager = app.Services.GetRequiredService<IModuleManager>();
await moduleManager.StartModulesAsync();

await app.RunAsync();

await moduleManager.StopModulesAsync();
```

To shut down the application gracefully `app.RunAsync()` must run to end and `IModuleManager.StopModulesAsync()` must be fully executed in time.
While `app.RunAsync()` finishes automatically on receiving certain events from the operating system, it lies in the resposibility of the module developer to keep the stop process as fast as possible, to prevent the operating system from assuming that the application is nonresponsive.

## Limitations

An application on its own cannot control, how much time it is given by its execution environment to terminate gracefully.
The time given depends on the operating system, the application type, the termination event and the configuration of the graceful shutdown timespan. 

## Termination on Microsoft Windows
If a MORYX based application is executed as a Windows console process, it is only notified, when `Ctrl+C` or `Ctrl+Break` are pressed. Other process terminating activities like closing the console window, logging off the current user or shutting down Windows result in a hard kill of the application process, without the possibility to shut down the application gracefully.

There is the possibility to receive more termination events, but this requires the usage of the Win32 API, especially the usage of the [SetConsoleCtrlHandler](https://learn.microsoft.com/de-de/windows/console/setconsolectrlhandler) function.
And even if this OS dependent functionality is used to begin a graceful application termination, the time between the termination event arrival and the hard process kill is limited to 5 seconds.

To gain more control over the application shutdown process, a MORYX application could be executed as a Windows service. More information about this topic can be found [here](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/windows-service).
Windows tries to shut down services gracefully, when services are stopped by the user or the operating system is rebooted or shut down.
By default, a service has 30 seconds on being stopped by the user to stop execution and to cleanup all its modules. After this period the service is considered as unresponsive and hence is killed by the operating system.
A service can extend this period to a maximum of 125 seconds by using [RequestAdditionalTime](https://learn.microsoft.com/en-us/dotnet/api/system.serviceprocess.servicebase.requestadditionaltime).
There is a different timeout, when a service is stopped due to operating system shutdown/reboot. Search for `WaitToKillServiceTimeout` for more details.

## Termination on Linux/Docker
On Linux the MORYX application receives the `SIGTERM`-> `SIGKILL` event sequence in case of an operating system reboot/shutdown and a `SIGTERM` event in case of a `kill -TERM <pid>` call.
The application receives a `SIGINT` event on pressing `Ctrl+C`. In all these cases a graceful application shutdown is initiated in the context of a ASP.NET Core application.
Like on Windows logging off the user or closing a terminal window does not result in a graceful application shutdown.

The grace period between a `SIGTERM` and a `SIGKILL` event is 90 seconds by default for plain linux processes.
For applications in docker containers the default grace period is set to 10 seconds.
See the docker documentation on how to configure this grace period.

## Termination on machines with Uninterruptible Power Supplies (UPS)

As can be seen above, there is no guarantee that a MORYX application can shut down gracefully under all conditions, when relying on operating system events alone.
To decouple the application shutdown process from the operating system lifecycle and events, it is possible to listen to events originated from Uninterruptible Power Supplies (UPS), which announce a coming power shutdown in advance, before actually shutting down the PC the MORYX application runs on.

An UPS can come with power backup times up to several hours, which gives enough time to gracefully terminate the running applications.
This should not prevent the module developers from keeping their module cleanup phases as short as possible.

One possible solution to realize the described graceful shutdown architecture is to use the [Network UPS Tools (NUT)](https://networkupstools.org/).
The NUT come with a client implementation, which allows observing power shutdown events.
Once a client receives a power shutdown event, it could advice docker to terminate all its running containers with an unlimited grace period.
An unlimited grace period can be set in the docker compose configuration by using a large value for `stop_grace_period` or by setting `--stop-timeout = -1` on docker run.
