---
uid: Logging
---
# Logging

Logging is an essential part of every application and framework. Logging within the platform is build on the `Microsoft.Extensions.Logging`. To decouple our executables from those libraries their API was wrapped within components from the `Moryx.Logging namespace. The wrapper does not only hide the API but provides additional features. In most cases access to the loggers is fullfilled by the responsible DI Container. Level 1 components receive a logger instance by passing themselves to the ActivateLogging-method of the LoggerManagement.

## Usage

Level 2 components that are created by a DI-Container receive a logger instance by declaring a public property of type [IModuleLogger](../../../src/Moryx/Logging/IModuleLogger.cs), which is basically `ILogger`. This property must be decorated with the [UseChildAttribute](../../../src/Moryx/Container/UseChildAttribute.cs) to tell the container it shall create a class specific clone of the logger. Optionally a name can be give to create a sub-logger in the hierarchy. A name might be used more than once to group collaborating components in the same logger. It is recommended to implement the interface [ILoggingComponent](../../../src/Moryx/Logging/ILoggingComponent.cs). This gives framework components a chance to use your logger in case of exceptions.

````cs
public class DummyComponent : ILoggingComponent
{
    [UseChild(nameof(DummyLogging))]
    public IModuleLogger Logger { get; set; }

    public void Foo(int argument)
    {
        Logger.Log(LogLevel.Debug, "Foo was called with {0}", argument);
        
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