// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.Drivers.InOut;
using Moryx.AbstractionLayer.Drivers.Message;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Drivers.OpcUa.Factories;
using Moryx.Drivers.OpcUa.Nodes;
using Moryx.Drivers.OpcUa.Properties;
using Moryx.Drivers.OpcUa.States;
using Moryx.Serialization;
using Moryx.StateMachines;
using Moryx.Threading;
using Moryx.Tools;
using Opc.Ua;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa;

/// <summary>
/// Driver to communicate via Opc Ua . It is able to write and read nodes
/// and subscribe to value changes of nodes in a session
/// </summary>
[ResourceRegistration]
[Display(Name = nameof(Strings.OpcUaDriver_DisplayName), Description = nameof(Strings.OpcUaDriver_Description), ResourceType = typeof(Strings))]
public class OpcUaDriver : Driver, IOpcUaDriver, IOpcUaDriverAddSubscription
{
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

    /// <summary>
    /// List of node id aliases to simply node access in code that uses this driver.
    /// Defining a NodeIdAlias `"switch_pressed"="ns=4;i=123"` can help reducing hard coded
    /// node ids and makes it possible to move those to any kind of configuration.
    /// </summary>
    [EntrySerialize]
    [Display(Name = nameof(Strings.OpcUaDriver_NodeIdAlias), ResourceType = typeof(Strings))]
    public List<NodeIdAlias> NodeIdAlias
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
    /// Timer used in message queue
    /// </summary>
    public IParallelOperations ParallelOperations { get; set; }

    // TODO: Check if at least some nodes should be read initially.
    // Should probably be fine to set this to true when connected or ready.
    /// <summary>
    /// The number of nodes that have been read.
    /// </summary>
    public bool HasChannels => _nodesFlat.Count > 0;

    private DriverOpcUaState State => (DriverOpcUaState)CurrentState;

    /// <inheritdoc />
    public IInput Input { get; set; }

    /// <inheritdoc />
    public IOutput Output { get; set; }

    /// <inheritdoc />
    public IDriver Driver => this;

    private readonly Dictionary<string, OpcUaNode> _nodesFlat = [];
    private List<string> _nodesToBeSubscribed = [];

    internal ISession _session; //TODO: Internal field just for tests
    private SessionReconnectHandler _reconnectHandler;

    private readonly SemaphoreSlim _lock = new(1, 1);

    private Subscription _subscription;

    //TODO: Internal property just for tests, use xml also in tests
    internal ApplicationConfigurationFactory ApplicationConfigurationFactory { get; set; } = new();
    internal NodeReaderFactory NodeReaderFactory { get; set; } = new NodeReaderFactory();
    internal SubscriptionFactory SubscriptionFactory { get; set; } = new();

    #region Lifecycle

    /// <inheritdoc/>
    protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        await base.OnInitializeAsync(cancellationToken);

        Input = new OpcUaInput(this);
        Output = new OpcUaOutput(this);
        _nodeIdAliasDictionary ??= [];

        await StateMachine.ForAsyncContext(this).WithAsync<DriverOpcUaState>(cancellationToken);

        ServerStatus = ServerState.Unknown;
    }

    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <inheritdoc/>
    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        await base.OnStartAsync(cancellationToken);
        ApplicationConfigurationFactory.ApplicationName += " " + Identifier;
        await Connect(cancellationToken);
    }

    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <inheritdoc/>
    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        return State.DisconnectAsync(cancellationToken);
    }
    #endregion

    #region Connection Handling

    private Task Connect(CancellationToken cancellationToken)
    {
        return State.Semaphore.ExecuteAsync(() => State.ConnectAsync(cancellationToken), cancellationToken);
    }

    /// <summary>
    /// Try to connect to the Opc Ua server
    /// </summary>
    /// <exception cref="Exception"></exception>
    internal async Task TryConnect(bool firstTry, CancellationToken cancellationToken)
    {
        if (_session == null)
        {
            var result = await CreateSessionAsync(firstTry, cancellationToken);
            if (result == false)
            {
                return;
            }
        }

        await _session.FetchNamespaceTablesAsync(cancellationToken);
        _session.KeepAlive += ClientKeepAlive;

        await State.Semaphore.ExecuteAsync(async () => await State.OnConnectingCompletedAsync(true, cancellationToken), cancellationToken);
    }

    private async Task<bool> CreateSessionAsync(bool firstTry, CancellationToken cancellationToken)
    {
        var config = await ApplicationConfigurationFactory.Create(Logger, FilePathClientConfig, cancellationToken);
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

            selectedEndpoint = await CoreClientUtils.SelectEndpointAsync(config,
                builder.Uri.ToString(), UseEncryption, cancellationToken);
        }
        catch (Exception e)
        {
            if (firstTry)
            {
                Logger.Log(LogLevel.Error, "Failed to connect {Uri} ({Message})", builder?.Uri.ToString() ?? OpcUaServerUrl, e.Message);
            }
            ParallelOperations?.ScheduleExecution(async () => await TryToConnectAgainAsync(cancellationToken), ReconnectionPeriod, -1);
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
            config.TransportQuotas.ChannelLifetime = 3600000;
            _session = await Session.CreateAsync(DefaultSessionFactory.Instance, config, (ITransportWaitingConnection)null, endpoint, false, false, ApplicationConfigurationFactory.ApplicationName, 1200000, userIdentity, null, cancellationToken);
        }
        catch (Exception ex)
        {
            if (firstTry)
            {
                Logger.Log(LogLevel.Error, "{Message}", ex.Message);
            }

            ParallelOperations.ScheduleExecution(() => TryToConnectAgainAsync(cancellationToken), ReconnectionPeriod, -1);
            return false;
        }
        return true;
    }

    private static string BuildScheme(UriBuilder builder)
    {
        return IsOpcScheme(builder.Scheme) ? builder.Scheme : "opc.tcp";
    }

    private static bool IsOpcScheme(string scheme) => !string.IsNullOrEmpty(scheme) && scheme.Contains("opc");

    private Task TryToConnectAgainAsync(CancellationToken cancellationToken)
    {
        return State.OnConnectingCompletedAsync(false, cancellationToken);
    }

    private async void ClientKeepAlive(ISession session, KeepAliveEventArgs e)
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
            await State.OnConnectionLostAsync(e, default);
        }

    }

    internal void Reconnect(KeepAliveEventArgs e)
    {
        if (ReconnectionPeriod <= 0)
        {
            Logger.Log(LogLevel.Warning, "KeepAlive status {Status}, but reconnect is disabled.", e.Status);
            return;
        }

        _lock.Execute(() =>
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
        });
    }

    /// <summary>
    /// Called when the reconnect attempt was successful.
    /// </summary>
    private async void ReconnectComplete(object sender, EventArgs e)
    {
        // ignore callbacks from discarded objects.
        if (!ReferenceEquals(sender, _reconnectHandler))
        {
            return;
        }

        await _lock.ExecuteAsync(async () =>
        {
            // if session recovered, Session property is null
            if (_reconnectHandler.Session != null)
            {
                _session = _reconnectHandler.Session as Session;
            }

            _reconnectHandler.Dispose();
            _reconnectHandler = null;
            await State.Semaphore.ExecuteAsync(async () => await State.OnConnectingCompletedAsync(true, default), default);
        },
        default);
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
        _session?.CloseAsync().GetAwaiter().GetResult();
        _session = null;
    }

    #endregion

    /// <inheritdoc />
    public IMessageChannel Channel(string identifier)
    {
        return State.GetNode(identifier);
    }

    /// <inheritdoc/>
    public async Task<OpcUaNode> GetNodeAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        var expandedNodeId = OpcUaNode.CreateExpandedNodeId(GetNodeIdAsString(nodeId));
        if (!_nodesFlat.TryGetValue(expandedNodeId, out var node))
        {
            var browser = NodeReaderFactory.CreateNodeReader(Logger, this);
            node = await browser.ReadNodeAsync(expandedNodeId, _session.NamespaceUris, _session, cancellationToken);
            if (node != null)
            {
                _nodesFlat.TryAdd(expandedNodeId, node);
            }
        }
        return node;
    }

    private string GetNodeIdAsString(string identifier)
    {
        if (_nodeIdAliasDictionary.TryGetValue(identifier, out var nodeId))
        {
            return nodeId;
        }

        return identifier;
    }

    /// <inheritdoc/>
    public Task AddSubscriptionAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        State.AddSubscription(nodeId);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task AddSubscriptionAsync(OpcUaNode node, CancellationToken cancellationToken = default)
    {
        State.AddSubscription(node.NodeId.ToString());
        return Task.CompletedTask;
    }

    internal void SaveSubscriptionToBeAdded(string nodeId)
    {
        var duplicateNode = _nodesToBeSubscribed.FirstOrDefault(x => x == nodeId);
        if (duplicateNode == null)
        {
            _nodesToBeSubscribed.Add(nodeId);
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
            _nodesToBeSubscribed.Add(node.Identifier);
        }
        _subscription?.Dispose();
        _subscription = null;
    }

    internal async Task SubscribeSavedNodesAsync(CancellationToken cancellationToken = default)
    {
        _subscription = SubscriptionFactory.CreateSubscription(new Subscription(_session.DefaultSubscription)
        {
            PublishingEnabled = true,
            PublishingInterval = PublishingInterval,
            LifetimeCount = 0,
        });

        _session.AddSubscription(_subscription);

        // Create the subscription on Server side
        await _subscription.CreateAsync(cancellationToken);

        //Subscribe Saved Nodes
        foreach (var nodeId in _nodesToBeSubscribed ?? [])
        {
            var node = await GetNodeAsync(nodeId, cancellationToken);
            var monitoredItem = CreateMonitoredItem(node);
            if (monitoredItem == null)
            {
                continue;
            }

            _subscription.AddItem(monitoredItem);
        }

        //Subscribe default Nodes
        foreach (var nodeId in DefaultSubscriptions ?? [])
        {
            var node = State.GetNode(nodeId);
            if (node == null)
            {
                Logger.Log(LogLevel.Warning, "Node with the id {nodeId} was not found", nodeId);
                continue;
            }

            if (_nodesToBeSubscribed.Contains(nodeId))
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
        await _subscription.ApplyChangesAsync(cancellationToken);

        await State.OnSubscriptionsInitializedAsync(cancellationToken);
    }

    internal Task AddSubscriptionToSession(OpcUaNode node, CancellationToken cancellationToken = default)
    {
        if (node.NodeClass != NodeClass.Variable)
        {
            Logger.Log(LogLevel.Warning, "It was tried to subscribe to the node {NodeId}. But that node is no variable node", node.NodeId);
            return Task.CompletedTask;
        }

        if (node.MonitoredItem != null)
        {
            return Task.CompletedTask;
        }

        var monitoredItem = CreateMonitoredItem(node);
        if (monitoredItem == null)
        {
            return Task.CompletedTask;
        }

        _subscription.AddItem(monitoredItem);
        return _subscription.ApplyChangesAsync(cancellationToken);
    }

    private MonitoredItem CreateMonitoredItem(OpcUaNode node)
    {
        if (node.MonitoredItem != null)
        {
            return node.MonitoredItem;
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

        if (nodeId.IdType == IdType.Numeric && int.Parse(nodeId.Identifier.ToString(), CultureInfo.InvariantCulture) == Variables.Server_ServerStatus
            && nodeId.NamespaceIndex == 0)
        {
            ServerStatus = ((ServerStatusDataType)((ExtensionObject)value).Body).State;
        }

        var innerNodeId = new NodeId(nodeId.Identifier, nodeId.NamespaceIndex);
        var node = State.GetNode(innerNodeId.ExpandedNodeIdString());
        if (node != null && node.Subscribed)
        {
            node.ReceivedMessage(value);
        }

        Received?.Invoke(this, msg);
    }

    #region Read and write nodes

    internal Task WriteNode(OpcUaNode node, object payload, CancellationToken cancellationToken = default)
    {
        return State.WriteNodeAsync(node, payload, cancellationToken);
    }

    /// <inheritdoc/>
    public void WriteNode(string nodeId, object payload)
    {
        var node = State.GetNode(nodeId);
        WriteNode(node, payload).GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public Task WriteNodeAsync(string nodeId, object payload, CancellationToken cancellationToken = default)
    {
        var node = State.GetNode(nodeId);
        return State.WriteNodeAsync(node, payload, cancellationToken);
    }

    internal async Task OnWriteNode(OpcUaNode node, object payload, CancellationToken cancellationToken = default)
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

        var writeResult = await _session.WriteAsync(null, [valueToBeWritten], cancellationToken);

        if (writeResult.Results != null)
        {
            if (writeResult.Results.First().Code != 0)
            {
                Logger.Log(LogLevel.Warning, "There was an error when trying to write a value to node {NodeId}", node.NodeId);
            }
        }
    }
    /// <inheritdoc/>
    public object ReadNode(string nodeId)
    {
        return ReadNodeDataValue(nodeId).GetAwaiter().GetResult().Result.Value;
    }

    /// <inheritdoc />
    public async Task<object> ReadNodeAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        var result = await ReadNodeDataValue(nodeId, cancellationToken);
        return result.Result?.Value;
    }

    private async Task<DataValueResult> ReadNodeDataValue(string nodeId, CancellationToken cancellationToken = default)
    {
        var value = await State.ReadValueAsync(nodeId, cancellationToken);
        if (!value.Success)
        {
            if (value.Error?.Exception != null)
            {
                Logger.Log(LogLevel.Error, value.Error.Exception, "Error reading node data.");
                return null;
            }
        }
        return value;
    }

    internal async Task<DataValueResult> OnReadValueOfNode(string identifier, CancellationToken cancellationToken)
    {
        var node = State.GetNode(identifier);
        if (node == null)
        {
            return DataValueResult.WithError($"The node \"{identifier}\" was not found");
        }
        // TODO: This has to be tested:
        //if (node.NodeClass != NodeClass.Variable)
        //{
        //    return DataValueResult.WithError($"The node \"{identifier}\" was not of type 'variable'");
        //}

        var nodeId = ExpandedNodeId.ToNodeId(node.NodeId, _session.NamespaceUris);
        var value = await _session.ReadValueAsync(nodeId, cancellationToken);
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
        if (payload is not OpcUaMessage)
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
    internal async Task<string> ReadNodeAsString(string nodeId)
    {
        try
        {
            var value = await ReadNodeDataValue(nodeId);
            if (value == null)
            {
                return "There was an error, when trying to read the value of the node. Please look into the log for further information";
            }

            return value.ToString();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to read node as string");
            return e.Message;
        }
    }

    /// <summary>
    /// Method to write values to a node over the UI for testing
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="valueString"></param>
    /// <param name="cancellationToken"></param>
    [EntrySerialize]
    internal async Task WriteNode(string identifier, string valueString, CancellationToken cancellationToken)
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
        var currentValue = await _session.ReadValueAsync(nodeId, cancellationToken);
        var value = CreateValue(currentValue.WrappedValue.TypeInfo.BuiltInType, valueString);

        await State.WriteNodeAsync(node, value, CancellationToken.None);

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
                    return short.Parse(stringValue, CultureInfo.InvariantCulture);
                case BuiltInType.Enumeration:
                case BuiltInType.Integer:
                case BuiltInType.Int32:
                    return int.Parse(stringValue, CultureInfo.InvariantCulture);
                case BuiltInType.Int64:
                    return long.Parse(stringValue, CultureInfo.InvariantCulture);
                case BuiltInType.UInt16:
                    return ushort.Parse(stringValue, CultureInfo.InvariantCulture);
                case BuiltInType.UInteger:
                case BuiltInType.UInt32:
                    return uint.Parse(stringValue, CultureInfo.InvariantCulture);
                case BuiltInType.UInt64:
                    return ulong.Parse(stringValue, CultureInfo.InvariantCulture);
                case BuiltInType.DateTime:
                    return DateTime.Parse(stringValue, CultureInfo.InvariantCulture);
                case BuiltInType.Guid:
                case BuiltInType.String:
                    return stringValue;
                case BuiltInType.Number:
                case BuiltInType.Float:
                case BuiltInType.Double:
                    return double.Parse(stringValue, CultureInfo.InvariantCulture);
                case BuiltInType.Byte:
                    return byte.Parse(stringValue, CultureInfo.InvariantCulture);
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
        var result = _nodesFlat.Where(x => x.Value.DisplayName.Contains(displayName, StringComparison.CurrentCultureIgnoreCase) || x.Value.DisplayName.ToLower().Equals(displayName.ToLower()))
            .Select(x => x.Key).ToList();

        return result;
    }

    /// <inheritdoc/>
    [EntrySerialize]
    public Task RebrowseNodesAsync(CancellationToken cancellationToken)
    {
        return State.Semaphore.ExecuteAsync(async () => await RebrowseNodesAsync(cancellationToken), cancellationToken);
    }

    /// <summary>
    /// Subscribe Nodes directly using the driver
    /// </summary>
    /// <param name="identifier"></param>
    [EntrySerialize]
    internal void SubscribeNode(string identifier)
    {
        AddSubscriptionAsync(identifier);
    }

    #endregion

    internal async Task ReadDeviceSetAsync(CancellationToken cancellationToken)
    {
        // TODO: Read nodes here
        var node = _nodesFlat.Select(x => x.Value).FirstOrDefault(x => x.DisplayName != null && x.DisplayName.Equals("DeviceSet"));
        if (node == null)
        {
            return;
        }

        DeviceSet = [];
        var tasks = node.Nodes.Select(async subNode =>
        {
            if (subNode.DisplayName.Equals("DeviceFeatures"))
            {
                return;
            }

            if (subNode.NodeClass != NodeClass.Object)
            {
                return;
            }

            var deviceType = new DeviceType()
            {
                Name = subNode.DisplayName
            };
            var properties = deviceType.GetType().GetProperties();
            var subTasks = subNode.Nodes.Select(async subSubNode =>
            {
                var propertyName = subSubNode.DisplayName;
                var property = properties.FirstOrDefault(x => x.Name.Equals(propertyName));
                if (property == null)
                {
                    return;
                }

                var value = ((await ReadNodeAsync(subSubNode.NodeId.ToString(), cancellationToken)) ?? 0).ToString();
                if (property.PropertyType == typeof(int))
                {
                    property.SetValue(deviceType, int.Parse(value, CultureInfo.InvariantCulture));
                }
                else
                {
                    property.SetValue(deviceType, value);
                }
            });

            await Task.WhenAll(subTasks);

            DeviceSet.Add(deviceType);
        });

        await Task.WhenAll(tasks);
    }

    internal void PublishRunningState()
    {
        ChangedToRunningState?.Invoke(this, new EventArgs());
    }

    /// <inheritdoc/>
    public event EventHandler<object> Received;

    /// <summary>
    /// Invoked if the driver changes it's state to `Running`
    /// </summary>
    public event EventHandler ChangedToRunningState;
}
