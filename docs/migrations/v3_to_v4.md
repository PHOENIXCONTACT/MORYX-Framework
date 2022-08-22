## Migrate to Core v4

For this major release the main motivation was switching from our own MORYX Runtime to ASP.NET Core with MORYX extensions.

### .NET Framework

MORYX Core no longer supports the legacy .NET Framework and is only available for .NET 5.0 and above. There is also no more WCF support or embedded kestrel hosting. Instead standard ASP.NET API-Controllers can be used that import MORYX components (kernel, modules or facade).

### DI Container replacement

As of version 4 we replaced the global Castle Windsor container with Microsofts `ServiceCollection`. This has several benefits:

- Improved integration of ASP components, controller and MORYX modules and facades
- Bigger community and support
- Simplified project setup with standard ASP templates

It also requires a couple of changes when you migrate modules or applications from Core version 3.

- Constructor injection instead of properties
- Use constructors instead of Initialize methods to prepare a component
- StartProjects are ASP projects now and `Main` method only uses ASP now

Example for the application project:

````cs
...
using Moryx.Runtime.Kernel;
using Moryx.Tools;
using Moryx.Model;
...
public static void Main(string[] args)
{            
    AppDomainBuilder.LoadAssemblies();

    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices(serviceCollection =>
        {
            serviceCollection.AddMoryxKernel();
            serviceCollection.AddMoryxModels();
            serviceCollection.AddMoryxModules();
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        }).Build();

    host.Services.UseMoryxConfigurations("Config");
    host.Services.StartMoryxModules();

    host.Run();

    host.Services.StopMoryxModules();
}
````

Constructor for a `ModuleController`. This also includes further imports like `IDbContextManager`.

````cs
public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory, IDbContextManager contextManager) 
    : base(containerFactory, configManager, loggerFactory)
{
    ContextManager = contextManager;
}
````

When exporting facades from you module, the new structure no longer exports facades with implicit polymorphism. Instead when your facade provides multiple interfaces, for example after a new minor version, you need to export each one explicitly.

````cs
public class ModuleController : ServerModuleFacadeControllerBase<ModuleConfig>, 
    IFacadeContainer<IProductManagement>, 
    IFacadeContainer<IProductManagementModification>,
    IFacadeContainer<IProductManagementTypeSearch>

private readonly ProductManagementFacade _productManagement = new ProductManagementFacade();

IProductManagement IFacadeContainer<IProductManagement>.Facade => _productManagement;
IProductManagementTypeSearch IFacadeContainer<IProductManagementTypeSearch>.Facade => _productManagement;
IProductManagementModification IFacadeContainer<IProductManagementModification>.Facade => _productManagement;    
````

### Hosting

Hosting of Endpoints from within Modules is no longer supported removing IEndpointHosting and IEndpointHostFactory from within modules as well as the Activate Hosting extension on the container. Asp Controllers can get the Facade injected and are hosted by Asp natively.

### Logging

Most of the MORYX logging was replaced by the types from "Microsoft.Extensions.Logging". 

Changes:
- `IModuleLogger` is basically `ILogger`, thereby `LogException` is just `Log` now
- `Moryx.Logging.LogLevel` was replaced with log level from MS
- LoggerManagement is gone without replacement
- The DummyLogger was removed, an equivalent is given by instantiating `new ModuleLogger("Dummy", typeof(ResourceManager), new NullLoggerFactory())`

### Maintenance

The Maintenance module and its internally hosted web UI are gone. They are replaced by kernel based ASP endpoints and a razor hosted frontend.

### Package changes

Added:
- "Moryx.Runtime.Endpoints" // Contains endpoints for maintenance now
- "Moryx.Maintenance.Web"

Removed:
- "Moryx.Runtime.Maintenance" 
- "Moryx.Runtime.Maintenance.Web" /Replaced by Moryx.Maintenance.Web with razor hosting
- "Moryx.Asp.Extensions" // Not needed anymore, Shell related content moved to "Moryx" package
