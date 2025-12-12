// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.Drivers.Message;
using Moryx.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa;

/// <summary>
/// MessageChannel representing an Opc Ua node
/// Because it is possible to get channels before the driver is running and has
/// browsed all nodes, only o
/// </summary>
public class OpcUaNode : IMessageChannel
{
    private IOpcUaDriver _driver => (IOpcUaDriver)Driver;

    private const int DefaultNamespaceLength = 5;
    private readonly IModuleLogger _logger;

    /// <inheritdoc/>
    public IDriver Driver { get; private set; }

    /// <inheritdoc/>
    public string Identifier => CreateExpandedNodeId(NodeId.Format(CultureInfo.InvariantCulture));

    /// <summary>
    /// Id including the namespaceUri
    /// </summary>
    public ExpandedNodeId NodeId { get; private set; }

    /// <summary>
    /// Display Name of the Opc Ua Node
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Class of the node like object, variable and method
    /// </summary>
    public NodeClass NodeClass { get; set; }

    /// <summary>
    /// Browse Name of the Opc Ua Node
    /// </summary>
    public QualifiedName BrowseName { get; set; }

    /// <summary>
    /// Represents, if the received event of the node was subscribed
    /// </summary>
    public bool Subscribed => _received != null;

    /// <summary>
    /// Description of the node
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// List of all Subnodes. This property is null, when the node is no object node
    /// </summary>
    public List<OpcUaNode> Nodes { get; set; }

    private event EventHandler<object> _received;

    /// <summary>
    /// Event fill be raised, when the node is a variable node and their value on the server changed
    /// </summary>
    public event EventHandler<object> Received
    {
        add
        {
            _driver.AddSubscription(this);
            _received += value;
        }
        remove
        {
            _received -= value;
        }
    }

    /// <summary>
    /// Converts a NodeId string into an expanded NodeId string that matches the OPC UA specification;
    /// </summary>
    public static string CreateExpandedNodeId(string nodeId)
    {
        var result = nodeId.Trim();
        result = OpcUaNodeHelpers.NodeIdWithNamespaceIndexRegex().Replace(result, "");
        result = RemoveNamespaceIndexZero(result);
        return result;
    }

    private static string RemoveNamespaceIndexZero(string nodeId)
    {
        if (nodeId.StartsWith("ns=0;"))
        {
            return nodeId[DefaultNamespaceLength..];
        }
        return nodeId;
    }

    /// <summary>
    /// Converts a NodeId into an expanded NodeId string that matches the OPC UA specification;
    /// </summary>
    public static string CreateExpandedNodeId(NodeId nodeId)
    {
        return CreateExpandedNodeId(nodeId.ToString());
    }

    internal MonitoredItem MonitoredItem { get; set; }

    #region Constructors
    private OpcUaNode(IOpcUaDriver driver, IModuleLogger logger)
    {
        Driver = driver;
        _logger = logger;

    }

    /// <inheritdoc />
    public OpcUaNode(IOpcUaDriver driver, IModuleLogger logger, string namespaceUri, string nodeIdValue)
        : this(driver, logger)
    {
        NodeId = new ExpandedNodeId(nodeIdValue, namespaceUri);
    }

    /// <inheritdoc />
    public OpcUaNode(IOpcUaDriver driver, IModuleLogger logger, ExpandedNodeId nodeId, NamespaceTable namespaceTable)
        : this(driver, logger)
    {
        NodeId = new ExpandedNodeId(nodeId.Identifier, nodeId.NamespaceIndex, namespaceTable.GetString(nodeId.NamespaceIndex), nodeId.ServerIndex);
    }

    /// <inheritdoc />
    public OpcUaNode(IOpcUaDriver driver, IModuleLogger logger, string identifier)
    {
        Driver = driver;
        NodeId = ExpandedNodeId.Parse(identifier);
        _logger = logger;
    }
    #endregion

    internal void UpdateNodeId(NamespaceTable namespaceTable)
    {
        NodeId = new ExpandedNodeId(NodeId.Identifier, (ushort)namespaceTable.GetIndex(NodeId.NamespaceUri), NodeId.NamespaceUri, NodeId.ServerIndex);
    }

    /// <summary>
    /// Write a value to the Node. At the moment this only works for variable nodes
    /// TODO 6.2: Object Nodes should be able to send whole objects containing the values of the variable nodes included in this object
    /// </summary>
    /// <param name="payload">value, which should be written to the node</param>
    public void Send(object payload)
    {
        if (NodeClass != NodeClass.Variable)
        {
            _logger?.Log(LogLevel.Error, "It is tried to read the value of node {NodeId}, but the node is no variable node", NodeId);
            return;
        }
        _driver.WriteNode(Identifier, payload);
    }

    /// <summary>
    /// Not implemented yet
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task SendAsync(object payload, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    internal void ReceivedMessage(object payload)
    {
        _received?.Invoke(this, payload);
    }
}
