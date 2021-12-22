# Enable NLog

The runtime uses `Microsoft.Extensions.Logging` as a lower abstraction for several logging appender. The whole logging structure is described in [Logging](xref:Logging).
One possible way to extend the logging mechanism is to attach [NLog](https://nlog-project.org/).

In this tutorial log4net will be added to an existing project.

1. Add the `NLog.Extensions.Logging` nuget package to your start project
2. Add a new folder `Config` to you start project
3. Add a new file to this directory: `nlog.config` with the following content:
    ````xml
    <?xml version="1.0" encoding="utf-8" ?>
    <!-- XSD manual extracted from package NLog.Schema: https://www.nuget.org/packages/NLog.Schema-->
    <nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xsi:schemaLocation="NLog NLog.xsd"
        xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" >

    <!-- the targets to write to -->
    <targets>
        <!-- write logs to file -->
        <target name="logfile" xsi:type="File" fileName="Logs/Runtime.Log" />
        <target xsi:type="Console" name="logconsole"
                layout="${longdate}| ${level} | ${message} | ${all-event-properties} ${exception:format=tostring}" />
    </targets>

    <!-- rules to map from logger name to target -->
    <rules>
        <logger name="*" minlevel="Trace" writeTo="logfile" />
    </rules>
    </nlog>
    ````

    This file have to be set to **copy to your output directory** (Properties -> Copy to output directory -> Copy always).

    This configuration will configure nlog with a file appender. You can find other appender configurations in the [nlog documentation](https://nlog-project.org/config/).
4. Configure nlog as log provider of the logger factory (you can add this code to the `Program.cs` of your application, before starting the `HeartOfGold`) e.g.
    ````cs
    var loader = new HeartOfGold(args);

    var loggerFactory = loader.GlobalContainer.Resolve<ILoggerFactory>();
    loggerFactory.AddProvider(new NLogLoggerProvider());

    var result = loader.Run();
    return (int)result;
    ````
5. Test your configuration after a rebuild
