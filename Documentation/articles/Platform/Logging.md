---
uid: Logging
---
Logging
=======

Logging is an essential part of every application and framework. Logging within the platform is build on the Common.Logging API and Log4Net implementation. To decouple our executables from those libraries their API was wrapped within components from the [Marvin.Logging](xref:Marvin.Logging) namespace. The wrapper does not only hide the API but provides additional features. In most cases access to the loggers is fullfilled by the responsible DI Container. Level 1 components receive a logger instance by passing themselves to the ActivateLogging-method of the LoggerManagement.

## Features
Features from log4net:
* Multiple appenders: The user can choose from a range of appenders when configuring the logging. The default Runtime config only uses the RollingFileAppender.
* Documentation: http://logging.apache.org/log4net/release/features.html

Features provided by the wrapper:
* StreamListening: Components can register listeners to the logging stream. The messages can than be displayed on a GUI.
* AutoHierarchy: The parent-child structure of loggers within the Runtime create automatic hierarchies.
* Level modification: The active log level for a logger (and its children) can be applied during runtime.

## Usage
On level 1 components can gain access to the logging by passing themselves to the [ActivateLogging](xref:Marvin.Logging.LoggerManagement#Marvin_Logging_LoggerManagement_ActivateLogging_Marvin_Logging_ILoggingHost_)-method. Inside this method the [LoggerManagement](xref:Marvin.Logging.LoggerManagement) will create a [IModuleLogger](xref:Marvin.Logging.IModuleLogger) instance and set the Logger property of the [ILoggingHost](xref:Marvin.Logging.ILoggingHost) interface that is required by the [ActivateLogging](xref:Marvin.Logging.LoggerManagement)-method. After the call returned using the logger instance is identical to the usage described below.

Level 2 components that are created by a DI-Container receive a logger instance by declaring a public property of type [IModuleLogger](xref:Marvin.Logging.IModuleLogger). This property must be decorated with the [UseChildAttribute](xref:Marvin.Container.UseChildAttribute) to tell the container it shall create a class specific clone of the logger. Optionally a name can be give to create a sub-logger in the hierarchy. A name might be used more than once to group collaborating components in the same logger. It is recommended to implement the interface [ILoggingComponent](xref:Marvin.Logging.ILoggingComponent). This gives framework components a chance to use your logger in case of exceptions.

````cs
public class DummyComponent : ILoggingComponent
{
    [UseChild(nameof(DummyLogging))]
    public IModuleLogger Logger { get; set; }

    public void Foo(int argument)
    {
        Logger.LogEntry(LogLevel.Debug, "Foo was called with {0}", argument);
        
        try
        {
            RiskyCall();
        }
        catch(RiskyExcpetion exception)
        {
            Logger.LogException(LogLevel.Error, exception, "RiskyCall failed");
        }
    }
}
````