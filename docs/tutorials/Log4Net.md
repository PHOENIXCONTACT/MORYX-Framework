# Enable Log4Net

The runtime uses `Common.Logging` as a lower abstraction for several logging appender. The whole logging structure is described in [Logging](xref:Logging).
One possible way to extend the logging mechanism is to attach [Apache Log4Net](http://logging.apache.org/log4net/release/features.html).

In this tutorial log4net will be added to an existing project.

1. Add the `Common.Logging.Log4Net208` nuget package to your start project (Attention: The `Common.Logging` dependency of this package must match the version used in `Marvin.Runtime.Kernel`)
2. Add a new folder `Config` to you start project
3. Add a new file to this directory: `log4net.config` with the following content:
    ````xml
    <?xml version="1.0" encoding="utf-8"?>
    <log4net>
    <appender name="RollingFileAppender" type="log4net.Appender.RollingFileAppender">
        <!-- file value is set from log4net.{configuration}.config /-->
        <appendToFile value="true" />
        <rollingStyle value="Size" />
        <maxSizeRollBackups value="10" />
        <maximumFileSize value="10000KB" />
        <staticLogFileName value="true" />
        <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%date - %-5level [%logger].%message%newline" />
        </layout>
        <file value="Logs/Runtime.log"/>
    </appender>

    <root>
        <level value="ALL" />
        <appender-ref ref="RollingFileAppender" />
    </root>
    </log4net>
    ````

    This file have to be set to **copy to your output directory** (Properties -> Copy to output directory -> Copy always).

    This configuration will configure log4net with a rolling file appender. You can find other appender configurations in the [log4net documentation](https://logging.apache.org/log4net/release/config-examples.html).
4. Add an additional config section and configuration to your `app.config` of your start project:
    ````xml
    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
    <configSections>
        <!-- Register Common Logging Section-->
        <sectionGroup name="common">
        <section name="logging" type="Common.Logging.ConfigurationSectionHandler, Common.Logging" />
        </sectionGroup>
        <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
    </configSections>
    <!-- Common Logging Configuration -->
    <common>
        <logging>
        <!-- using log4net 2.0.8 as Logging tool -->
        <factoryAdapter type="Common.Logging.Log4Net.Log4NetLoggerFactoryAdapter, Common.Logging.Log4Net208">
            <arg key="configType" value="FILE-WATCH" />
            <!-- log4net as configured in a separate file -->
            <arg key="configFile" value="~/Config/log4net.config" />
        </factoryAdapter>
        </logging>
    </common>
    </configuration>
    ````

    This will configure the common logging adapter to load the previously added config file.
5. Test your configuration after a rebuild