// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.Drivers.InOut;
using Moryx.AbstractionLayer.Drivers.Message;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Drivers.OpcUa;
using Moryx.Drivers.OpcUa.Properties;
using Moryx.Drivers.OpcUa.States;
using Moryx.Serialization;
using Moryx.StateMachines;
using Moryx.Threading;
using Opc.Ua;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa;

/// <summary>
/// Driver to communicate via Opc Ua . It is able to write and read nodes
/// and subscribe to value changes of nodes in a session
/// </summary>
[ResourceRegistration]
[Display(Name = nameof(Strings.OpcUaDriver_DisplayName), Description = nameof(Strings.OpcUaDriver_Description), ResourceType = typeof(Strings))]
public class OpcUaDriver : Driver, IOpcUaDriver
{
    //TODO 6.1 Invoke Methods

    private const int NodeLayersShown = 5;
    /// <summary>
    /// Current tate of the driver
    /// </summary>
    [EntrySerialize]
    [Display(Name = nameof(Strings.OpcUaDriver_StateName), ResourceType = typeof(Strings))]
    internal string StateName => CurrentState?.ToString() ?? "";

    [EntrySerialize, ReadOnly(true)]
    [Display(Name = nameof(Strings.OpcUaDriver_ServerStatus), ResourceType = typeof(Strings))]
    internal ServerState ServerStatus { get; private set; }

    [EntrySerialize, ReadOnly(true)]
    [Display(Name = nameof(Strings.OpcUaDriver_DeviceSet), Description = nameof(Strings.OpcUaDriver_DeviceSet_Description), ResourceType = typeof(Strings))]
    internal List<DeviceType> DeviceSet { get; set; } = [];

    #region Configuration
    /// <summary>
    /// List of default subscriptions
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.OpcUaDriver_DefaultSubscriptions), ResourceType = typeof(Strings))]
    public List<string> DefaultSubscriptions { get; set; } = [];

    [DataMember]
    internal Dictionary<string, string> _nodeIdAliasDictionary; // TODO: Internal field just for tests, could be private

    [EntrySerialize]
    [Display(Name = nameof(Strings.OpcUaDriver_NodeIdAlias), ResourceType = typeof(Strings))]
    internal List<NodeIdAlias> NodeIdAlias
    {
        get
        {
            if (_nodeIdAliasDictionary == null)
            {
                _nodeIdAliasDictionary = [];
                return [];
            }
            return [.. _nodeIdAliasDictionary.Select(x => new NodeIdAlias { Alias = x.Key, NodeId = x.Value })];
        }
        set
        {
            if (value != null)
            {
                _nodeIdAliasDictionary = value.ToDictionary(x => x.Alias, x => x.NodeId);
            }
            else
            {
                _nodeIdAliasDictionary = [];
            }
        }
    }

    /// <summary>
    /// Identifier of the driver
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.OpcUaDriver_Identifier), ResourceType = typeof(Strings))]
    public string Identifier { get; set; }

    /// <summary>
    /// Url of the OPC UA Server
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.OpcUaDriver_OpcUaServerUrl), Description = nameof(Strings.OpcUaDriver_OpcUaServerUrl_Description), ResourceType = typeof(Strings))]
    public string OpcUaServerUrl { get; set; }

    /// <summary>
    /// Username needed to authenticate on the server
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.OpcUaDriver_Username), ResourceType = typeof(Strings))]
    public string Username { get; set; }

    /// <summary>
    /// Password needed to authenticate on the server
    /// </summary>
    [EntrySerialize, DataMember, Password]
    [Display(Name = nameof(Strings.OpcUaDriver_Password), ResourceType = typeof(Strings))]
    public string Password { get; set; }

    /// <summary>
    /// Use encryption during communication
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.OpcUaDriver_UseEncryption), Description = nameof(Strings.OpcUaDriver_UseEncryption_Description), ResourceType = typeof(Strings))]
    public bool UseEncryption { get; set; }

    /// <summary>
    /// Path of the config file
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.OpcUaDriver_FilePathClientConfig), Description = nameof(Strings.OpcUaDriver_FilePathClientConfig_Description), ResourceType = typeof(Strings))]
    public string FilePathClientConfig { get; set; }

    /// <summary>
    /// Reconnection Period
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.OpcUaDriver_ReconnectionPeriod), Description = nameof(Strings.OpcUaDriver_ReconnectionPeriod_Description), ResourceType = typeof(Strings))]
    public int ReconnectionPeriod { get; set; }

    // TODO: Update Publishing- and SamplingInterval without restarting the driver
    /// <summary>
    /// Interval, how often the server publishes notifications to the driver
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.OpcUaDriver_PublishingInterval), Description = nameof(Strings.OpcUaDriver_PublishingInterval_Description), ResourceType = typeof(Strings))]
    public int PublishingInterval { get; set; }

    /// <summary>
    /// Interval on which the changes of the monitored values are checked
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.OpcUaDriver_SamplingInterval), Description = nameof(Strings.OpcUaDriver_SamplingInterval_Description), ResourceType = typeof(Strings))]
    public int SamplingInterval { get; set; }

    //TODO: Use selfsigned certificates for communication

    #endregion

    /// <summary>
    /// All nodes found on the Opc Ua server
    /// </summary>
    [EntrySerialize, ReadOnly(true)]
    [Display(Name = nameof(Strings.OpcUaDriver_Nodes), ResourceType = typeof(Strings))]
    internal List<OpcUaDisplayNode> Nodes { get; private set; } = [];

    /// <summary>
    /// Timer used in message queue
    /// </summary>
    public IParallelOperations ParallelOperations { get; set; }

    /// <summary>
    /// The number of nodes under the driver
    /// </summary>
    public bool HasChannels => _nodesFlat.Count > 0;

    private DriverOpcUaState State => (DriverOpcUaState)CurrentState;

    /// <inheritdoc />
    public IInput Input { get; set; }

    /// <inheritdoc />
    public IOutput Output { get; set; }

    /// <inheritdoc />
    public IDriver Driver => this;

    private List<OpcUaNode> _nodes = [];
    private readonly Dictionary<string, OpcUaNode> _nodesFlat = [];
    private List<OpcUaNode> _nodesToBeSubscribed = [];
    private readonly HashSet<string> _savedIds = [];

    internal ISession _session; //TODO: Internal field just for tests
    private SessionReconnectHandler _reconnectHandler;

    private readonly object _lock = new();
    private readonly object _stateLock = new();

    private Subscription _subscription;

    //TODO: Internal property just for tests, use xml also in tests
    internal ApplicationConfigurationFactory ApplicationConfigurationFactory { get; set; } = new();

    /// <summary>
    /// Convert an OpcUaNode to an entity to be shown on the UI
    /// </summary>
    /// <param name="nodes"></param>
    /// <returns></returns>
    private static List<OpcUaDisplayNode> ConvertToDisplayNodes(List<OpcUaNode> nodes)
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
    protected override async Task OnInitializeAsync()
    {
        await base.OnInitializeAsync();

        Input = new OpcUaInput(this);
        Output = new OpcUaOutput(this);
        _nodeIdAliasDictionary ??= [];

        StateMachine.Initialize(this).With<DriverOpcUaState>();

        ServerStatus = ServerState.Unknown;
    }

    /// <inheritdoc/>
    protected override async Task OnStartAsync()
    {
        await base.OnStartAsync();
        ApplicationConfigurationFactory.ApplicationName += " " + Identifier;
        Connect();
    }

    /// <inheritdoc/>
    protected override Task OnStopAsync()
    {
        State.Disconnect();
        return Task.CompletedTask;
    }
    #endregion

    #region Connection Handling

    private void Connect()
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
    internal async Task TryConnect(bool firstTry)
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
        var config = await ApplicationConfigurationFactory.Create(Logger, FilePathClientConfig);
        if (config == null)
        {
            return false;
        }

        UriBuilder builder = null;
        EndpointDescription selectedEndpoint;
        try
        {
            builder = new UriBuilder(OpcUaServerUrl);
            builder.Scheme = BuildScheme(builder);

            selectedEndpoint = CoreClientUtils.SelectEndpoint(config,
                builder.Uri.ToString(), UseEncryption);
        }
        catch (Exception e)
        {
            if (firstTry)
            {
                Logger.Log(LogLevel.Error, "Failed to connect {Uri} ({Message})", builder?.Uri.ToString() ?? OpcUaServerUrl, e.Message);
            }

            ParallelOperations?.ScheduleExecution(TryToConnectAgain, ReconnectionPeriod, -1);
            return false;
        }
        var endpointConfiguration = EndpointConfiguration.Create(config);
        var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);

        var userIdentity = new UserIdentity(Username, Password);
        if (string.IsNullOrEmpty(Username))
        {
            userIdentity = null;
        }

        try
        {
            _session = await Session.Create(config, endpoint, false, false, ApplicationConfigurationFactory.ApplicationName, 60000, userIdentity, null);
        }
        catch (Exception ex)
        {
            if (firstTry)
            {
                Logger.Log(LogLevel.Error, "{Message}", ex.Message);
            }

            ParallelOperations.ScheduleExecution(TryToConnectAgain, ReconnectionPeriod, -1);
            return false;
        }
        return true;
    }

    private static string BuildScheme(UriBuilder builder)
    {
        return IsOpcScheme(builder.Scheme) ? builder.Scheme : "opc.tcp";
    }

    private static bool IsOpcScheme(string scheme) => !string.IsNullOrEmpty(scheme) && scheme.Contains("opc");

    private void TryToConnectAgain()
    {
        State.OnConnectingCompleted(false);
    }

    private void ClientKeepAlive(ISession session, KeepAliveEventArgs e)
    {

        // check for events from discarded sessions.
        if (!ReferenceEquals(session, _session))
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
        if (!ReferenceEquals(sender, _reconnectHandler))
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
    internal void Disconnect()
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

    /// <inheritdoc />
    public IMessageChannel Channel(string identifier)
    {
        return State.GetNode(identifier);
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
        if (_nodeIdAliasDictionary.ContainsKey(identifier))
        {
            return _nodeIdAliasDictionary[identifier];
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
            var node = State.GetNode(nodeId);
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
                Logger.Log(LogLevel.Warning, "It was tried to subscribe to the node {nodeId}. But that node is no variable node", node.NodeId);
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
            QueueSize = ComputeQueueSize(PublishingInterval, SamplingInterval),
            DiscardOldest = true
        };
        monitoredItem.Notification += OnMonitoredItemNotification;
        node.MonitoredItem = monitoredItem;
        return monitoredItem;
    }

    private uint ComputeQueueSize(int publishingInterval, int samplingInterval)
    {
        samplingInterval = samplingInterval > 0
            ? SamplingInterval
            : 1;
        return (uint)(publishingInterval / samplingInterval + 10);
    }

    private void OnMonitoredItemNotification(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
    {
        var nodeId = new ExpandedNodeId(monitoredItem.ResolvedNodeId,
            _session.NamespaceUris.GetString(monitoredItem.ResolvedNodeId.NamespaceIndex));
        var receivedObject = ((MonitoredItemNotification)e.NotificationValue).Value.Value;
        OnSubscriptionChanged(nodeId, receivedObject);
    }

    internal void OnSubscriptionChanged(ExpandedNodeId nodeId, object value)
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
    }

    #region Browse Nodes

    internal void BrowseNodes()
    {

        var namespaceUris = _session.NamespaceUris;
        var nodes = new List<OpcUaNode>();

        BrowseNodes(ObjectIds.RootFolder, namespaceUris, nodes, 0);
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

            if (layer < NodeLayersShown)
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

    internal void WriteNode(OpcUaNode node, object payload)
    {
        State.WriteNode(node, payload);
    }

    public void WriteNode(string nodeId, object payload)
    {
        var node = State.GetNode(nodeId);
        WriteNode(node, payload);
    }

    /// <inheritdoc />
    public Task WriteNodeAsync(string nodeId, object payload, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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
    public object ReadNode(string nodeId)
    {
        return ReadNodeDataValue(nodeId).Result.Value;
    }

    /// <inheritdoc />
    public Task<object> ReadNodeAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private DataValueResult ReadNodeDataValue(string nodeId)
    {
        var value = State.ReadValue(nodeId);
        if (!value.Success)
        {
            if (value.Error?.Exception != null)
            {
                Logger.Log(LogLevel.Error, value.Error.Exception, value.Error?.Message);
                return null;
            }
        }
        return value;
    }

    internal DataValueResult OnReadValueOfNode(string identifier)
    {
        var node = State.GetNode(identifier);
        if (node == null)
        {
            return DataValueResult.WithError($"The node \"{identifier}\" was not found");
        }
        if (node.NodeClass != NodeClass.Variable)
        {
            return DataValueResult.WithError($"The node \"{identifier}\" was not of type 'variable'");
        }

        var nodeId = ExpandedNodeId.ToNodeId(node.NodeId, _session.NamespaceUris);
        var value = _session.ReadValue(nodeId);
        if (StatusCode.IsGood(value.StatusCode))
        {
            return new DataValueResult(value);
        }

        return DataValueResult.WithError($"The node \"{identifier}\" was not of type 'variable'");
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

        var node = State.GetNode(msg.Identifier);
        const string errorMsg = "When trying to read the value of the node, ";
        if (node == null)
        {
            Logger.Log(LogLevel.Error, "{errorMsg} the node with the id {Identifier} was not found", errorMsg, msg.Identifier);
            return;
        }
        if (node.NodeClass != NodeClass.Variable)
        {
            Logger.Log(LogLevel.Error, "{errorMsg} the node with the id {Identifier} was no variable node", errorMsg, msg.Identifier);
            return;
        }

        WriteNode(node.Identifier, msg.Payload);

    }

    /// <inheritdoc/>
    public Task SendAsync(object payload, CancellationToken cancellationToken = default)
    {
        if (payload is not OpcUaMessage msg)
        {
            Logger.Log(LogLevel.Warning, "Currently it is only possible to send messages of the type OpcUaMessage " +
                                         "using the Opc Ua Driver directly");
            return Task.CompletedTask;
        }

        throw new NotImplementedException();
    }


    #endregion

    #region UI Methods

    /// <summary>
    /// Method to read nodes from the ui for testing
    /// </summary>
    /// <param name="nodeId"></param>
    /// <returns></returns>
    [EntrySerialize]
    internal string ReadNodeAsString(string nodeId)
    {
        var value = ReadNodeDataValue(nodeId);
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
    internal void WriteNode(string identifier, string valueString)
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
                case BuiltInType.Boolean:
                    return bool.Parse(stringValue);
                case BuiltInType.Int16:
                    return short.Parse(stringValue);
                case BuiltInType.Enumeration:
                case BuiltInType.Integer:
                case BuiltInType.Int32:
                    return int.Parse(stringValue);
                case BuiltInType.Int64:
                    return long.Parse(stringValue);
                case BuiltInType.UInt16:
                    return ushort.Parse(stringValue);
                case BuiltInType.UInteger:
                case BuiltInType.UInt32:
                    return uint.Parse(stringValue);
                case BuiltInType.UInt64:
                    return ulong.Parse(stringValue);
                case BuiltInType.DateTime:
                    return DateTime.Parse(stringValue);
                case BuiltInType.Guid:
                case BuiltInType.String:
                    return stringValue;
                case BuiltInType.Number:
                case BuiltInType.Float:
                case BuiltInType.Double:
                    return double.Parse(stringValue);
                case BuiltInType.Byte:
                    return byte.Parse(stringValue);
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
    internal List<string> FindNodeId(string displayName)
    {
        var result = _nodesFlat.Where(x => x.Value.DisplayName.ToLower().Contains(displayName.ToLower()) || x.Value.DisplayName.ToLower().Equals(displayName.ToLower()))
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
    internal void SubscribeNode(string identifier)
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

    /// <inheritdoc/>
    public event EventHandler<object> Received;
}
