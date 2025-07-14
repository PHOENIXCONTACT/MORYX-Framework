﻿// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.Drivers.InOut;
using Moryx.AbstractionLayer.Drivers.Message;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Drivers.OpcUa.DriverStates;
using Moryx.Drivers.OpcUa.Nodes;
using Moryx.Serialization;
using Moryx.StateMachines;
using Moryx.Threading;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System.ComponentModel.DataAnnotations;
using Moryx.Drivers.OpcUa.Localizations;

namespace Moryx.Drivers.OpcUa;

/// <summary>
/// Driver to communicate via Opc Ua . It is able to write and read nodes 
/// and subscribe to value changes of nodes in a session
/// </summary>
[ResourceRegistration]
[Display(Name = nameof(Strings.OPCUA_DRIVER), Description = nameof(Strings.OPCUA_DRIVER_DESCRIPTION), ResourceType = typeof(Localizations.Strings))]
public class OpcUaDriver : Driver, IOpcUaDriver2
{
    //TODO 6.1 Invoke Methods

    private const int NODE_LAYERS_SHOWN = 5;
    /// <summary>
    /// Current tate of the driver
    /// </summary>
    
    [EntrySerialize]
    [Display(Name = nameof(Strings.DRIVER_STATE), ResourceType = typeof(Localizations.Strings))]
    public string StateName => CurrentState?.ToString() ?? "";

    [EntrySerialize, ReadOnly(true)]
    [Display(Name = nameof(Strings.SERVER_STATUS), ResourceType = typeof(Localizations.Strings))]
    public ServerState ServerStatus { get; private set; }

    [EntrySerialize, ReadOnly(true)]
    [Display(Name = nameof(Strings.DEVICE_SET), Description = nameof(Strings.DEVICE_SET_DESCRIPTION), ResourceType = typeof(Localizations.Strings))]
    public List<DeviceType> DeviceSet { get; set; } = [];

    #region Configuration
    /// <summary>
    /// List of default subscriptions
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.DEFAULT_SUBSCRIPTIONS), ResourceType = typeof(Localizations.Strings))]
    public List<string> DefaultSubscriptions { get; set; } = [];

    [DataMember]
    public Dictionary<string, string> NodeIdAliasDictionary;

    [EntrySerialize]
    [Display(Name = nameof(Strings.NODE_ID_ALIAS), ResourceType = typeof(Localizations.Strings))]
    internal List<NodeIdAlias> NodeIdAlias
    {
        get
        {
            if (NodeIdAliasDictionary == null)
            {
                NodeIdAliasDictionary = [];
                return [];
            }
            return [.. NodeIdAliasDictionary.Select(x => new NodeIdAlias { Alias = x.Key, NodeId = x.Value })];
        }
        set
        {
            if (value != null)
            {
                NodeIdAliasDictionary = value.ToDictionary(x => x.Alias, x => x.NodeId);
            }
            else
            {
                NodeIdAliasDictionary = [];
            }
        }
    }
    /// <summary>
    /// Identifier of the driver
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.IDENTIFIER), ResourceType = typeof(Localizations.Strings))]
    public string Identifier { get; set; }

    /// <summary>
    /// Url of the OPC UA Server
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.OPCUA_SERVER_URL), Description = nameof(Strings.OPCUA_SERVER_URL_DESCRIPTION), ResourceType = typeof(Localizations.Strings))]
    public string OpcUaServerUrl { get; set; }

    /// <summary>
    /// Port of the OPC UA Server
    /// </summary>
    [EntrySerialize, DataMember]
    [Obsolete("This property will be removed in a future version. It's recommended to include the port into the URL")]
    [Display(Name = nameof(Strings.OPCUA_SERVER_PORT), ResourceType = typeof(Localizations.Strings))]
    public int OpcUaServerPort { get; set; }

    /// <summary>
    /// Username needed to authenticate on the server
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.USERNAME), ResourceType = typeof(Localizations.Strings))]
    public string Username { get; set; }

    /// <summary>
    /// Password needed to authenticate on the server
    /// </summary>
    [EntrySerialize, DataMember, Password]
    [Display(Name = nameof(Strings.PASSWORD), ResourceType = typeof(Localizations.Strings))]
    public string Password { get; set; }

    /// <summary>
    /// Use encryption during communication
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.USE_ENCRYPTION), Description =nameof(Strings.USE_ENCRYPTION_DESCRIPTION), ResourceType = typeof(Localizations.Strings))]
    public bool UseEncryption { get; set; }

    /// <summary>
    /// Path of the config file
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.FILE_PATH_CLIENT), Description = nameof(Strings.FILE_PATH_CLIENT_DESCRIPTION), ResourceType = typeof(Localizations.Strings))]
    public string FilePathClientConfig { get; set; }

    /// <summary>
    /// Reconnection Period
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.RECONNECTION_PERIOD), Description = nameof(Strings.RECONNECTION_PERIOD_DESCRIPTION), ResourceType = typeof(Localizations.Strings))]
    public int ReconnectionPeriod { get; set; }

    /// <summary>
    /// Interval, how often the server publishes notifications to the driver
    /// </summary>
    //TODO 6.2 Update Publishing- and SamplingInterval whithout restarting the driver
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.PUBLISH_INTERVALL), Description = nameof(Strings.PUBLISH_INTERVALL_DESCRIPTION), ResourceType = typeof(Localizations.Strings))]
    public int PublishingInterval { get; set; }

    /// <summary>
    /// Interval on which the changes of the monitored values are checked
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.USE_ENCRYPTION), Description = nameof(Strings.USE_ENCRYPTION_DESCRIPTION), ResourceType = typeof(Localizations.Strings))]
    public int SamplingInterval { get; set; }

    //TODO 6.1 Use selfsigned certificates for communication

    #endregion

    /// <summary>
    /// All nodes found on the Opc Ua server 
    /// </summary>
    [EntrySerialize, ReadOnly(true)]
    [Display(Name = nameof(Strings.DISPLAY_NODE), ResourceType = typeof(Localizations.Strings))]
    public List<OpcUaDisplayNode> Nodes { get; private set; } = [];
    /// <summary>
    /// The number of nodes under the driver
    /// </summary>
    public bool HasChannels => _nodesFlat.Count > 0;

    /// <summary>
    /// The driver
    /// </summary>
    public IDriver Driver => this;

    /// <summary>
    /// Timer used in message queue
    /// </summary>
    public IParallelOperations ParallelOperations { get; set; }

    internal DriverOpcUaState State => (DriverOpcUaState)CurrentState;

    public IInput<object> Input { get; set; }

    public IOutput<object> Output { get; set; }

    private List<OpcUaNode> _nodes = [];
    private readonly Dictionary<string, OpcUaNode> _nodesFlat = [];
    private List<OpcUaNode> _nodesToBeSubscribed = [];
    private readonly HashSet<string> _savedIds = [];

    internal ISession _session;
    private SessionReconnectHandler _reconnectHandler;

    private readonly object _lock = new();
    private readonly object _stateLock = new();

    private string _applicationName = "MORYX OpcUa Client ";

    private Subscription _subscription = null;

    /// <inheritdoc/>
    public event EventHandler<object> Received;

    private event EventHandler<OpcUaMessage> _opcUaMessageReceived;
    event EventHandler<OpcUaMessage> IMessageChannel<OpcUaMessage, OpcUaMessage>.Received
    {
        add
        {
            _opcUaMessageReceived += value;
        }

        remove
        {
            _opcUaMessageReceived -= value;
        }
    }

    /// <summary>
    /// Convert an OpcUaNode to an entity to be shown on the UI
    /// </summary>
    /// <param name="nodes"></param>
    /// <returns></returns>
    private List<OpcUaDisplayNode> ConvertToDisplayNodes(List<OpcUaNode> nodes)
    {
        var list = new List<OpcUaDisplayNode>();
        foreach (var node in nodes)
        {
            OpcUaDisplayNode displayNode;
            if (node.NodeClass == NodeClass.Object)
            {
                var objectDisplayNode = new OpcUaObjectDisplayNode(node.NodeId)
                {
                    Nodes = ConvertToDisplayNodes(node.Nodes)
                };
                displayNode = objectDisplayNode;
            }
            else
            {
                displayNode = new OpcUaDisplayNode(node.NodeId);
            }
            displayNode.BrowseName = node.BrowseName.ToString();
            displayNode.DisplayName = node.DisplayName;
            displayNode.Description = node.Description;
            displayNode.ClassType = node.NodeClass;
            list.Add(displayNode);
        }

        return list;
    }

    #region Lifecycle
    /// <inheritdoc/>
    protected override void OnInitialize()
    {
        base.OnInitialize();
        Input = new OpcUaInput(this);
        Output = new OpcUaOutput(this);
        NodeIdAliasDictionary ??= [];

        StateMachine.Initialize(this).With<DriverOpcUaState>();

        ServerStatus = ServerState.Unknown;

    }

    /// <inheritdoc/>
    protected override void OnStart()
    {
        base.OnStart();
        _applicationName += " " + Identifier;
        Connect();
    }

    /// <inheritdoc/>
    protected override void OnStop()
    {
        State.Disconnect();
    }
    #endregion

    #region Connection Handling
    internal void Connect()
    {
        lock (_stateLock)
        {
            State.Connect();
        }
    }

    /// <summary>
    /// Try to connect to the Opc Ua server
    /// </summary>
    /// <exception cref="Exception"></exception>
    internal async void TryConnect(bool firstTry)
    {
        if (_session == null)
        {
            var result = await CreateSession(firstTry);
            if (result == false)
            {
                return;
            }
        }

        _session.KeepAlive += ClientKeepAlive;

        lock (_stateLock)
        {
            State.OnConnectingCompleted(true);
        }
    }

    private async Task<bool> CreateSession(bool firstTry)
    {
        var config = await CreateConfig();
        if (config == null)
        {
            return false;
        }

        var builder = new UriBuilder(OpcUaServerUrl);
        EndpointDescription selectedEndpoint;
        try
        {
            builder.Scheme = BuildScheme(builder);
            builder.Port = BuildPort(builder);

            selectedEndpoint = CoreClientUtils.SelectEndpoint(config,
                builder.Uri.ToString(), UseEncryption);
        }
        catch (Exception e)
        {
            if (firstTry)
            {
                Logger.Log(LogLevel.Error, "Failed to connect {Uri} ({Message})", builder.Uri, e.Message);
            }

            ParallelOperations.ScheduleExecution(TryToConnectAgaion, ReconnectionPeriod, -1);
            return false;
        }
        var endpointConfiguration = EndpointConfiguration.Create(config);
        var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);

        var userIdentity = new UserIdentity(Username, Password);
        if (Username == null || Username.Equals(""))
        {
            userIdentity = null;
        }

        try
        {
            _session = await Session.Create(config, endpoint, false, false, _applicationName, 60000, userIdentity, null);
        }
        catch (Exception ex)
        {
            if (firstTry)
            {
                Logger.Log(LogLevel.Error, "{Message}", ex.Message);
            }

            ParallelOperations.ScheduleExecution(TryToConnectAgaion, ReconnectionPeriod, -1);
            return false;
        }
        return true;
    }

    private int BuildPort(UriBuilder builder)
    {
        return (builder.Port > -1)
            ? builder.Port
            : OpcUaServerPort > 0
                ? OpcUaServerPort
                : DefaultSchemePort(builder.Scheme);
    }

    private static string BuildScheme(UriBuilder builder)
    {
        return IsOpcScheme(builder.Scheme) ? builder.Scheme : "opc.tcp";
    }

    private static bool IsOpcScheme(string scheme) => !string.IsNullOrEmpty(scheme) && scheme.Contains("opc");

    private static int DefaultSchemePort(string scheme)
    {
        const int DefaultPortTcp = 62541;
        const int DefaultPortHttps = 62540;
        return scheme.Contains("tcp") ? DefaultPortTcp : DefaultPortHttps;
    }

    private async Task<ApplicationConfiguration> CreateConfig()
    {
        var application = new ApplicationInstance
        {
            ApplicationName = _applicationName,
            ApplicationType = ApplicationType.Client,
            ConfigSectionName = "Moryx.OpcUa.Client",
        };

        ApplicationConfiguration config;
        var defaultPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "Config\\Opc.Ua.Default.Config.xml"));
        var filePath = string.IsNullOrEmpty(FilePathClientConfig) ? defaultPath : FilePathClientConfig;
        try
        {
            config = await application.LoadApplicationConfiguration(filePath, false);
        }
        catch (Exception ex)
        {
            Logger.LogError("{Message}", ex.Message);
            return null;
        }

        _applicationName = config.ApplicationName;
        // check the application certificate
        var haveAppCertificate = await application.CheckApplicationInstanceCertificate(false, 0);
        if (!haveAppCertificate)
        {
            throw new Exception("Application instance certificate invalid!");
        }
        else
        {
            config.ApplicationUri = X509Utils.GetApplicationUriFromCertificate(config.SecurityConfiguration.ApplicationCertificate.Certificate);
            config.CertificateValidator.CertificateValidation += CertificateValidatorCertificateValidation;
        }
        return config;
    }

    private void TryToConnectAgaion()
    {
        State.OnConnectingCompleted(false);
    }

    private void CertificateValidatorCertificateValidation(CertificateValidator validator, CertificateValidationEventArgs e)
    {
        Logger.Log(LogLevel.Error, "{StatusCode}", e.Error.StatusCode);
        if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
        {
            if (validator.AutoAcceptUntrustedCertificates)
            {
                e.Accept = true;
                if (validator.AutoAcceptUntrustedCertificates)
                {
                    Logger.Log(LogLevel.Information, "Accepted Certificate: {Subject}",
                    e.Certificate.Subject);
                }
                else
                {
                    Logger.Log(LogLevel.Information, "Rejected Certificate: {Subject}",
                    e.Certificate.Subject);
                }
            }
        }
    }

    private void ClientKeepAlive(ISession session, KeepAliveEventArgs e)
    {

        // check for events from discarded sessions.
        if (!Object.ReferenceEquals(session, _session))
        {
            return;
        }

        // start reconnect sequence on communication error.
        if (ServiceResult.IsBad(e.Status))
        {
            ServerStatus = ServerState.Unknown;
            State.OnConnectionLost(e);
        }

    }

    internal void Reconnect(KeepAliveEventArgs e)
    {
        if (ReconnectionPeriod <= 0)
        {
            Logger.Log(LogLevel.Warning, "KeepAlive status {Status}, but reconnect is disabled.", e.Status);
            return;
        }

        lock (_lock)
        {
            if (_reconnectHandler == null)
            {
                _reconnectHandler = new SessionReconnectHandler(true);
                _reconnectHandler.BeginReconnect(_session, ReconnectionPeriod, ReconnectComplete);
            }
            else
            {
                Logger.Log(LogLevel.Warning, "KeepAlive status {Status}, but reconnection should have already started.", e.Status);
                return;
            }
        }
    }

    /// <summary>
    /// Called when the reconnect attempt was successful.
    /// </summary>
    private void ReconnectComplete(object sender, EventArgs e)
    {
        // ignore callbacks from discarded objects.
        if (!Object.ReferenceEquals(sender, _reconnectHandler))
        {
            return;
        }

        lock (_lock)
        {
            // if session recovered, Session property is null
            if (_reconnectHandler.Session != null)
            {
                _session = _reconnectHandler.Session as Session;
            }

            _reconnectHandler.Dispose();
            _reconnectHandler = null;
            lock (_stateLock)
            {
                State.OnConnectingCompleted(true);
            }
        }
    }

    /// <summary>
    /// Disconnect from the OPC UA server
    /// </summary>
    public void Disconnect()
    {
        RemoveSubscription();
        if (_session == null)
        {
            return;
        }

        _session.KeepAlive -= ClientKeepAlive;
        _session?.Close();
        _session = null;
    }

    #endregion

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TChannel"></typeparam>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public IMessageChannel<TChannel> Channel<TChannel>(string identifier)
    {
        if (typeof(TChannel) != typeof(object))
        {
            Logger.Log(LogLevel.Warning,
                "Opc Ua Message Channels only implement IMessageChannel<object>");
            return null;
        }
        return (IMessageChannel<TChannel>)State.GetNode(identifier);
    }

    /// <inheritdoc/>
    public IMessageChannel<TSend, TReceive> Channel<TSend, TReceive>(string identifier)
    {
        if (typeof(TSend) == typeof(TReceive))
        {
            return (IMessageChannel<TSend, TReceive>)Channel<TSend>(identifier);
        }

        throw new NotSupportedException("Opc Ua Message Channels only implement IMessageChannel<object>");
    }

    /// <inheritdoc/>
    public OpcUaNode GetNode(string identifier)
    {
        var nodeId = OpcUaNode.CreateExpandedNodeId(GetNodeIdAsString(identifier));
        if (!_nodesFlat.ContainsKey(nodeId))
        {
            return null;
        }

        return _nodesFlat[nodeId];
    }

    private string GetNodeIdAsString(string identifier)
    {
        if (NodeIdAliasDictionary.ContainsKey(identifier))
        {
            return NodeIdAliasDictionary[identifier];
        }

        return identifier;
    }

    internal OpcUaNode GetNotInitializedNode(string identifier)
    {
        var nodeId = ExpandedNodeId.Parse(GetNodeIdAsString(identifier));
        if (nodeId.NamespaceUri == null || nodeId.NamespaceUri.Equals(""))
        {
            return null;
        }

        var node = new OpcUaNode(this, Logger, identifier);
        _nodesFlat.Add(node.Identifier, node);
        return node;
    }

    /// <inheritdoc/>
    public void AddSubscription(OpcUaNode node)
    {
        State.AddSubscription(node);
    }

    internal void SaveSubscriptionToBeAdded(OpcUaNode node)
    {
        var duplicateNode = _nodesToBeSubscribed.FirstOrDefault(x => x.NodeId.ToString().Equals(node.NodeId.ToString()));
        if (duplicateNode == null)
        {
            _nodesToBeSubscribed.Add(node);
        }
    }

    internal void RemoveSubscription()
    {
        var subscribedNodes = _nodesFlat.Select(x => x.Value).Where(x => x.MonitoredItem != null).ToList();
        foreach (var node in subscribedNodes)
        {
            if (node.MonitoredItem == null)
            {
                continue;
            }

            node.MonitoredItem.Notification -= OnMonitoredItemNotification;
            node.MonitoredItem = null;
            _nodesToBeSubscribed.Add(node);
        }
        _subscription?.Dispose();
        _subscription = null;
    }

    internal void SubscribeSavedNodes()
    {
        _subscription = new Subscription(_session.DefaultSubscription)
        {
            PublishingEnabled = true,
            PublishingInterval = PublishingInterval,
            LifetimeCount = 0
        };

        _session.AddSubscription(_subscription);

        // Create the subscription on Server side
        _subscription.Create();

        //Subscribe to ServerStatus
        //var serverStatusNode = _nodesFlat.Select(x=> x.Value).FirstOrDefault(
        //    x => x.NodeId.IdType == IdType.Numeric && int.Parse(x.NodeId.Identifier.ToString()) == Variables.Server_ServerStatus
        //    && x.NodeId.NamespaceIndex == 0);
        //if (serverStatusNode != null)
        //    _nodesToBeSubscribed.Add(serverStatusNode);

        //Subscribe Saved Nodes
        foreach (var node in _nodesToBeSubscribed)
        {
            if (node.NodeClass != NodeClass.Variable)
            {
                Logger.Log(LogLevel.Warning, "It was tried to subscribe to the node {NodeId}. But that node is no variable node", node.NodeId);
                continue;
            }
            var monitoredItem = CreateMonitoredItem(node);
            if (monitoredItem == null)
            {
                continue;
            }

            _subscription.AddItem(monitoredItem);
        }

        //Subscribe default Nodes
        foreach (var nodeId in DefaultSubscriptions)
        {
            var node = (OpcUaNode)Channel<object, object>(nodeId);
            if (node == null)
            {
                Logger.Log(LogLevel.Warning, "Node with the id {nodeId} was not found", nodeId);
                continue;
            }

            if (_nodesToBeSubscribed.Contains(node))
            {
                continue;
            }

            if (node.NodeClass != NodeClass.Variable)
            {
                Logger.Log(LogLevel.Warning, "It was tried to subscribe to the node {NodeId}. But that node is no variable node", node.NodeId);
                continue;
            }
            var monitoredItem = CreateMonitoredItem(node);
            if (monitoredItem == null)
            {
                continue;
            }

            _subscription.AddItem(monitoredItem);
        }
        _nodesToBeSubscribed = [];
        _subscription.ApplyChanges();

        State.OnSubscriptionsInitialized();
    }

    internal void AddSubscriptionToSession(OpcUaNode node)
    {

        if (node.NodeClass != NodeClass.Variable)
        {
            Logger.Log(LogLevel.Warning, "It was tried to subscribe to the node {NodeId}. But that node is no variable node", node.NodeId);
            return;
        }
        var monitoredItem = CreateMonitoredItem(node);
        if (monitoredItem == null)
        {
            return;
        }

        _subscription.AddItem(monitoredItem);
        _subscription.ApplyChanges();
    }

    private MonitoredItem CreateMonitoredItem(OpcUaNode node)
    {
        if (node.MonitoredItem != null)
        {
            return null;
        }

        var monitoredItem = new MonitoredItem(_subscription.DefaultItem)
        {
            StartNodeId = ExpandedNodeId.ToNodeId(node.NodeId, _session.NamespaceUris),
            AttributeId = Attributes.Value,
            DisplayName = node.DisplayName,
            SamplingInterval = SamplingInterval,
            QueueSize = (uint)(PublishingInterval / SamplingInterval + 10),
            DiscardOldest = true
        };
        monitoredItem.Notification += OnMonitoredItemNotification;
        node.MonitoredItem = monitoredItem;
        return monitoredItem;
    }

    private void OnMonitoredItemNotification(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
    {
        var nodeId = new ExpandedNodeId(monitoredItem.ResolvedNodeId,
            _session.NamespaceUris.GetString(monitoredItem.ResolvedNodeId.NamespaceIndex));
        var receivedObject = ((MonitoredItemNotification)e.NotificationValue).Value.Value;
        onSubscriptionChanged(nodeId, receivedObject);
    }

    internal void onSubscriptionChanged(ExpandedNodeId nodeId, object value)
    {
        var nodeIdString = nodeId.ToString();

        var msg = new OpcUaMessage()
        {
            Identifier = nodeIdString,
            Payload = value
        };

        if (nodeId.IdType == IdType.Numeric && int.Parse(nodeId.Identifier.ToString()) == Variables.Server_ServerStatus
            && nodeId.NamespaceIndex == 0)
        {
            ServerStatus = ((ServerStatusDataType)((ExtensionObject)value).Body).State;
        }

        var node = State.GetNode(nodeIdString);
        if (node != null && node.Subscribed)
        {
            node.ReceivedMessage(value);
        }

        Received?.Invoke(this, msg);
        _opcUaMessageReceived?.Invoke(this, msg);
    }

    #region Browse Nodes
    internal void BrowseNodes()
    {

        var namespaceUris = _session.NamespaceUris;
        var nodes = new List<OpcUaNode>();

        BrowseNodes(Opc.Ua.ObjectIds.RootFolder, namespaceUris, nodes, 0);
        _nodes = nodes;

        _savedIds.Clear();
        Nodes = ConvertToDisplayNodes(_nodes);
        lock (_stateLock)
        {
            State.OnBrowsingNodesCompleted();
        }
    }

    //todo: Change to BFS
    private void BrowseNodes(NodeId nodeId, NamespaceTable namespaceTable, List<OpcUaNode> list, int layer)
    {

        _session.Browse(null, null, nodeId, uint.MaxValue, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                out var continuationPoint,
                out var nextRefs);

        nextRefs ??= [];

        //https://reference.opcfoundation.org/Core/Part4/v104/docs/7.6
        while (continuationPoint != null)
        {
            _session.BrowseNext(null, false, continuationPoint, out var newContinuationPoint, out var ref3);
            if (ref3 == null)
            {
                if (newContinuationPoint == null)
                {
                    break;
                }

                continue;
            }
            foreach (var z in ref3)
            {
                nextRefs.Add(z);
            }

            continuationPoint = newContinuationPoint;
        }

        foreach (var nextRd in nextRefs)
        {
            var nextRdNodeId = OpcUaNode.CreateExpandedNodeId(nextRd.NodeId.ToString());
            OpcUaNode node = null;
            if (_nodesFlat.ContainsKey(nextRdNodeId))
            {
                node = _nodesFlat[nextRdNodeId];
            }

            if (node == null)
            {
                node = ConvertToNode(nextRd, namespaceTable);
            }
            else
            {
                node.UpdateNodeId(namespaceTable);
                node.DisplayName = nextRd.DisplayName.ToString();
                node.NodeClass = nextRd.NodeClass;
                node.BrowseName = nextRd.BrowseName;
            }

            if (node == null)
            {
                continue;
            }

            if (nextRd.NodeClass == NodeClass.Object)
            {
                var nodesOfObject = new List<OpcUaNode>();
                BrowseNodes(ExpandedNodeId.ToNodeId(nextRd.NodeId, namespaceTable), namespaceTable, nodesOfObject, layer + 1);
                node.Nodes = nodesOfObject;
            }

            var result = _savedIds.Add(node.Identifier);

            if (result == false)
            {
                continue;
            }

            if (!_nodesFlat.ContainsKey(node.Identifier))
            {
                _nodesFlat.Add(node.Identifier, node);
            }

            if (layer < NODE_LAYERS_SHOWN)
            {
                list.Add(node);
            }
        }
    }

    private OpcUaNode ConvertToNode(ReferenceDescription referenceDescription, NamespaceTable namespaceTable)
    {
        var node = new OpcUaNode(this, Logger, referenceDescription.NodeId, namespaceTable)
        {
            DisplayName = referenceDescription.DisplayName.ToString(),
            BrowseName = referenceDescription.BrowseName
        };
        switch (referenceDescription.NodeClass)
        {
            case NodeClass.Object:
                node.NodeClass = NodeClass.Object;
                break;
            case NodeClass.Method:
                node.NodeClass = NodeClass.Method;
                break;
            case NodeClass.Variable:
                node.NodeClass = NodeClass.Variable;
                break;
            default: return null;
        }

        return node;

    }

    #endregion

    #region Read and write nodes
    /// <inheritdoc/>
    public void WriteNode(OpcUaNode node, object payload)
    {
        State.WriteNode(node, payload);
    }

    public void WriteNode(string nodeId, object payload)
    {
        var node = State.GetNode(nodeId);
        WriteNode(node, payload);
    }

    internal void OnWriteNode(OpcUaNode node, object payload)
    {

        var valueToBeWritten = new WriteValue
        {
            NodeId = ExpandedNodeId.ToNodeId(node.NodeId, _session.NamespaceUris),
            AttributeId = Attributes.Value,
            Value = new DataValue
            {
                Value = payload
            }
        };


        _session.Write(null, [valueToBeWritten], out var results, out var diagnosticInfos);

        if (results != null)
        {
            if (results.First().Code != 0)
            {
                Logger.Log(LogLevel.Warning, "There was an error when trying to write a value to node {NodeId}", node.NodeId);
            }
        }
    }
    /// <inheritdoc/>
    public object ReadNode(string NodeId)
    {

        var value = State.ReadValue(NodeId);
        if (value == null)
        {
            Logger.Log(LogLevel.Error, "There was an error, when trying to read the value of the node. Please look into the log for further information");
            return null;
        }
        return value.Value;
    }

    internal DataValue OnReadValueOfNode(string identifier)
    {
        var node = State.GetNode(identifier);
        var errormsg = "When trying to read the value of the node, ";
        if (node == null)
        {
            Logger.Log(LogLevel.Error, "{errormsg} the node with the id {identifier} was not found", errormsg, identifier);
            return null;
        }
        if (node.NodeClass != NodeClass.Variable)
        {
            Logger.Log(LogLevel.Error, "{errormsg} the node with the id {identifier} was no variable node", errormsg, identifier);
            return null;
        }

        var nodeId = ExpandedNodeId.ToNodeId(node.NodeId, _session.NamespaceUris);
        var value = _session.ReadValue(nodeId);
        if (StatusCode.IsGood(value.StatusCode))
        {
            return value;
        }

        Logger.Log(LogLevel.Error, "{errormsg} the status was {StatusCode}", errormsg, value.StatusCode);
        return null;
    }

    /// <summary>
    /// Write a value to a node using the driver directly
    /// </summary>
    /// <param name="payload">Must be of the type OpcUaMessage</param>
    /// <exception cref="NotImplementedException"></exception>
    public void Send(object payload)
    {
        if (payload is not OpcUaMessage msg)
        {
            Logger.Log(LogLevel.Warning, "Currently it is only possible to send messages of the type OpcUaMessage " +
                "using the Opc Ua Driver directly");
            return;
        }

        Send(msg);

    }

    /// <inheritdoc/>
    public Task SendAsync(object payload)
    {
        if (payload is not OpcUaMessage msg)
        {
            Logger.Log(LogLevel.Warning, "Currently it is only possible to send messages of the type OpcUaMessage " +
                "using the Opc Ua Driver directly");
            return Task.CompletedTask;
        }
        return SendAsync(msg);
    }

    public void Send(OpcUaMessage msg)
    {
        var node = State.GetNode(msg.Identifier);
        var errormsg = "When trying to read the value of the node, ";
        if (node == null)
        {
            Logger.Log(LogLevel.Error, "{errormsg} the node with the id {Identifier} was not found", errormsg, msg.Identifier);
            return;
        }
        if (node.NodeClass != NodeClass.Variable)
        {
            Logger.Log(LogLevel.Error, "{errormsg} the node with the id {Identifier} was no variable node", errormsg, msg.Identifier);
            return;
        }

        WriteNode(node, msg.Payload);
    }

    public Task SendAsync(OpcUaMessage payload)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region UI Methods
    /// <summary>
    /// Method to read nodes from the ui for testing
    /// </summary>
    /// <param name="NodeId"></param>
    /// <returns></returns>
    [EntrySerialize]
    public string ReadNodeAsString(string NodeId)
    {
        var value = ReadNode(NodeId);
        if (value == null)
        {
            return "There was an error, when trying to read the value of the node. Please look into the log for further information";
        }

        return value.ToString();
    }

    /// <summary>
    /// Method to write values to a node over the UI for testing 
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="valueString"></param>
    [EntrySerialize]
    public void WriteNode(string identifier, string valueString)
    {
        var node = State.GetNode(identifier);
        var errormsg = "When trying to read the value of the node, ";
        if (node == null)
        {
            Logger.Log(LogLevel.Error, "{errormsg} the node with the id {identifier} was not found", errormsg, identifier);
            return;
        }
        if (node.NodeClass != NodeClass.Variable)
        {
            Logger.Log(LogLevel.Error, "{errormsg} the node with the id {NodeId} was no variable node", errormsg, node.NodeId);
            return;
        }

        var nodeId = ExpandedNodeId.ToNodeId(node.NodeId, _session.NamespaceUris);
        var currentValue = _session.ReadValue(nodeId);
        var value = CreateValue(currentValue.WrappedValue.TypeInfo.BuiltInType, valueString);

        State.WriteNode(node, value);

    }

    private object CreateValue(BuiltInType type, string stringValue)
    {
        try
        {
            switch (type)
            {
                case BuiltInType.Boolean: return bool.Parse(stringValue);

                case BuiltInType.Int16: return short.Parse(stringValue);
                case BuiltInType.Enumeration:
                case BuiltInType.Integer:
                case BuiltInType.Int32: return int.Parse(stringValue);
                case BuiltInType.Int64: return long.Parse(stringValue);
                case BuiltInType.UInt16: return ushort.Parse(stringValue);
                case BuiltInType.UInteger:
                case BuiltInType.UInt32: return uint.Parse(stringValue);
                case BuiltInType.UInt64: return ulong.Parse(stringValue);
                case BuiltInType.DateTime: return DateTime.Parse(stringValue);

                case BuiltInType.Guid:
                case BuiltInType.String: return stringValue;

                case BuiltInType.Number:
                case BuiltInType.Float:
                case BuiltInType.Double:
                    return double.Parse(stringValue);

                case BuiltInType.Byte: return byte.Parse(stringValue);

            }
        }
        catch (Exception ex)
        {
            Logger.Log(
                LogLevel.Error,
                "An error occured, when trying to cast the value for the OPC UA node to the corresponding type: {Message}",
                ex.Message);
        }
        return null;
    }

    [EntrySerialize]
    public List<string> FindNodeId(string Displayname)
    {
        var result = _nodesFlat.Where(x => x.Value.DisplayName.ToLower().Contains(Displayname.ToLower()) || x.Value.DisplayName.ToLower().Equals(Displayname.ToLower()))
            .Select(x => x.Key).ToList();

        return result;
    }

    /// <inheritdoc/>
    [EntrySerialize]
    public void RebrowseNodes()
    {
        lock (_stateLock)
        {
            State.RebrowseNodes();
        }
    }

    /// <summary>
    /// Subscribe Nodes directly using the driver
    /// </summary>
    /// <param name="identifier"></param>
    [EntrySerialize]
    public void SubscribeNode(string identifier)
    {
        var node = State.GetNode(identifier);
        AddSubscription(node);
    }

    #endregion

    internal void ReadDeviceSet()
    {

        var node = _nodesFlat.Select(x => x.Value).FirstOrDefault(x => x.DisplayName != null && x.DisplayName.Equals("DeviceSet"));
        if (node == null)
        {
            return;
        }

        DeviceSet = [];
        foreach (var subNode in node.Nodes)
        {

            if (subNode.DisplayName.Equals("DeviceFeatures"))
            {
                continue;
            }

            if (subNode.NodeClass != NodeClass.Object)
            {
                continue;
            }

            var deviceType = new DeviceType()
            {
                Name = subNode.DisplayName
            };
            var properties = deviceType.GetType().GetProperties();
            foreach (var subSubNode in subNode.Nodes)
            {
                var propertyName = subSubNode.DisplayName;
                var property = properties.FirstOrDefault(x => x.Name.Equals(propertyName));
                if (property == null)
                {
                    continue;
                }

                var value = ReadNode(subSubNode.NodeId.ToString()).ToString();
                if (property.PropertyType == typeof(int))
                {
                    property.SetValue(deviceType, int.Parse(value));
                }
                else
                {
                    property.SetValue(deviceType, value);
                }
            }

            DeviceSet.Add(deviceType);

        }

    }

    public List<object> InvokeMethod(string nodeId, object[] parameters)
    {
        throw new NotImplementedException();
    }

}
