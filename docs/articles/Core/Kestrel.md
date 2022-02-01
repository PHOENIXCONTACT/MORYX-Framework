---
uid: KestrelHosting
---
# Kestrel Endpoint Hosting

For cross platform hosting of HTTP API endpoints we provide a Kestrel integration in MORYX. This let's you integrate ASP.NET Core API controllers with your module.

## Activate in Module

To access this feature you need to declare a dependency on `IEndpointHosting` in your `ModuleController`. Use the extension to activate hosting for your module.

```cs
public class ModuleController : ServerModuleBase<ModuleConfig>, IPlatformModule
{
    /// <summary>
    /// Endpoint hosting
    /// </summary>
    public IEndpointHosting Hosting { get; set; }

    protected override void OnInitialize()
    {
        // ...

        Container.ActivateHosting(Hosting);

        // ..
    }
}
```

## Controller Declaration

To link the controller with your module and register it in MORYX endpoint discovery, a few attributes are necessary. If your controller replaces a previous WCF endpoint, you should export it under the old interface. The produces `"application/json"` is optional and lets you use objects as return values of your methods without explicit conversion. Within your controller you have access to your modules internal DI container.

```cs
[Plugin(LifeCycle.Transient, typeof(IModuleMaintenance))]
[ApiController, Route("legacy"), Produces("application/json")]
[Endpoint(Name = nameof(ILegacyService), Version = "3.0.0")]
internal class MyController : Controller, ILegacyService
{
    public IMyModuleComponent Component { get; set; }

    [HttpGet("foo/{value}")]
    public FooModel Foo(int value) => new FooModel(value);
}
```

## Host creation

To create the host and link the controller with your module you need to resolve or inject the `IEndpointHostFactory` and create host passing the controller or it's interface.

```cs
private IEndpointHost _host;

public IEndpointHostFactory HostFactory { get; set; }

public virtual void Start()
{
        _host = EndpointHostFactory.CreateHost(typeof(ILegacyService), null);
        // OR
        _host = EndpointHostFactory.CreateHost(typeof(MyController), null);
        _host.Start();
}
```

## Host extension

If you wish to add the ASP host running in the background you can do this from your executable with an extension on `HeartOfGold`. For example to add swagger look at the example below

```cs
static int Main(string[] args)
{
    var hog = new HeartOfGold(args);
    hog.UseStartup<SwaggerStartup>();
    hog.Run();
}

internal class SwaggerStartup : Startup
{
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "MORYX API", Version = "v1" });
        });
    }

    public override void Configure(IApplicationBuilder app)
    {
        base.Configure(app);

        app.UseDeveloperExceptionPage();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "MORYX API v1");
        });
    }
}
```

## Integrate MORYX with ASP.NET Core

There is also the option to run MORYX alongside ASP.NET Core. We created an extension for the service collection to register all facades, so you can access MORYX modules within your controllers or Razor pages. 

In your `Main` you need to load MORYX and pass it to the `StartUp` of the `HostBuilder`. Within the `StartUp` you pass it to `AddMoryxFacades()`.

````cs
public static int Main(string[] args)
{
    var moryxRuntime = new HeartOfGold(args);
    moryxRuntime.Load();
    
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices(serviceCollection => serviceCollection.AddSingleton(typeof(IApplicationRuntime), moryxRuntime))
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup(conf => new Startup(moryxRuntime));
        }).Build();

    host.Start();
    var result = moryxRuntime.Execute();
    host.Dispose();

    return (int)result;
}

public class Startup
{
    private readonly IApplicationRuntime _moryxRuntime;

    public Startup(IApplicationRuntime moryxRuntime)
    {
        _moryxRuntime = moryxRuntime;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();

        services.AddMoryxFacades(_moryxRuntime);
    }
````

You can than access facades through standard ASP constructor or Razor page injection with `@inject IMoryxFacade facade`.