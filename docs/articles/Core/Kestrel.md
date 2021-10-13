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