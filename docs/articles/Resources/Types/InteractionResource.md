---
uid: InteractionResource
---
# InteractionResource

InteractionResources are resources [Resource](../../../../src/Moryx.AbstractionLayer/Resources/Resource.cs), that host web services in addition to the one hosted by the 
`ResourceInteractionHost`, that hosts the endpoint for the standard resource UI. For WCF they use the Runtimes host factory. 
This enables developers to build special interfaces to interact with resources that are not covered by the standard web-service.

```cs
public interface ISomeInteraction
{
    long GetId( string name);
}

[Plugin(LifeCycle.Transient, typeof(ISomeService)]
internal class SomeService : ISomeService
{
    // Injected dependency 
    public IResourceGraph Graph { get; set; }

    public long GetId( string name) { return Graph.Get<Resource>(r => r.Name == name).Id; }
}

[ResourceRegistration, DependencyRegistration(typeof(ISomeInteraction))]
public sealed class MyResourceHost : Resource
{
    /// <summary>
    /// Factory to create the web service
    /// </summary>
    public IConfiguredHostFactory HostFactory { get; set; }

    /// <summary>
    /// Host config injected by resource manager
    /// </summary>
    [DataMember, EntrySerialize]
    public HostConfig HostConfig { get; set; }

    /// <inheritdoc />
    public override object Descriptor => HostConfig;

    /// <summary>
    /// Current service host
    /// </summary>
    private IConfiguredServiceHost _host;

    /// <summary>
    /// Constructor to set <see cref="HostConfig"/> defaults.
    /// </summary>
    public MyResourceHost()
    {
        HostConfig = new HostConfig
        {
            Endpoint = "myservice",
            BindingType = ServiceBindingType.WebHttp,
            MetadataEnabled = true
        };
    }

    /// <inheritdoc />
    protected override void OnInitialize()
    {
        base.OnInitialize();

        _host = HostFactory.CreateHost<ISomeInteraction>(HostConfig);
    }

    /// <inheritdoc />
    protected override void OnStart()
    {
        base.OnStart();

        _host.Start();
    }

    /// <inheritdoc />
    protected override void OnDispose()
    {
        _host.Stop();

        base.OnDispose();
    }
```

If you need a duplex connection, your resource should export an additional `ServiceManager` interface to accept the client subscriptions, as well as a collection of connected clients.
The second parameter in the attribute deﬁnes an additional service this resource is registered for besides IResource. It also uses the life cycle `Singleton` instead of the standard `Transient` life cycle.
Because of the additional service and modiﬁed life cycle the container managed service instances can declare a dependency on their host. 
As a side effect of this approach it is not possible to conﬁgure two instances of the same service at different endpoints.

```
[DependencyRegistration(typeof(IDuplexService))]
[ResourceRegistration(typeof(DuplexResourceHost))]
public class DuplexResourceHost : Resource, IServiceManager
{
    /// <summary>
    /// Factory to create the web service
    /// </summary>
    public IConfiguredHostFactory HostFactory { get; set; }

    /// <summary>
    /// Host config injected by resource manager
    /// </summary>
    [DataMember, EntrySerialize]
    public HostConfig HostConfig { get; set; }

    /// <inheritdoc />
    public override object Descriptor => HostConfig;

    public DuplexResourceHost()
    {
        HostConfig = new HostConfig
        {
            Endpoint = "duplexendpoint",
            BindingType = ServiceBindingType.NetTcp,
            MetadataEnabled = true,
        };
    }
    
    /// <summary>
    /// Current service host
    /// </summary>
    private IConfiguredServiceHost _host;

    /// <summary>
    /// Registered service instances
    /// </summary>
    private ICollection<IDuplexService> _clients = new SynchronizedCollection<IDuplexService>();

    /// <inheritdoc />
    protected override void OnInitialize()
    {
        base.OnInitialize();

        _host = HostFactory.CreateHost<IDuplexService>(HostConfig);
    }

    /// <inheritdoc />
    protected override void OnStart()
    {
        base.OnStart();

        _host.Start();
    }

    /// <inheritdoc />
    protected override void OnDispose()
    {
        _host.Stop();

        base.OnDispose();
    }

    /// <inheritdoc />
    void IServiceManager.Register(ISessionService service)
    {
        _clients.Add((IInteractionService)service);
    }

    /// <inheritdoc />
    void IServiceManager.Unregister(ISessionService service)
    {
        _clients.Remove((IInteractionService)service);
    }
}
```