// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers.InOut;
using Moryx.AbstractionLayer.Drivers.Message;

namespace Moryx.Drivers.OpcUa;

/// <summary>
/// Opc Ua Client
/// </summary>
public interface IOpcUaDriver : IMessageDriver, IInOutDriver
{
    // TODO 6.3: Subscriptions with different publishing and sampling intervals can be created
    // TODO 6.2: Subscriptions to ObjectNodes are possible
    /// <summary>
    /// Subscribes to a variable node. Nothing happens, if the node is not a variable
    /// </summary>
    /// <param name="node">OpcUaNode to be subscribed</param>
    void AddSubscription(OpcUaNode node);

    /// <summary>
    /// Read the value of a Node
    /// </summary>
    /// <param name="nodeId">NodeId</param>
    /// <returns>If node doesn't exists or there was an error, when trying to read
    /// the node, the return value will be null</returns>
    object ReadNode(string nodeId);

    /// <summary>
    /// Read the value of a Node
    /// </summary>
    /// <param name="nodeId">NodeId</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <returns>If node doesn't exists or there was an error, when trying to read
    /// the node, the return value will be null</returns>
    Task<object> ReadNodeAsync(string nodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rebrowse Nodes
    /// </summary>
    void RebrowseNodes();

    /// <summary>
    /// Returns an opcUaNode to a string
    /// </summary>
    /// <param name="nodeId">nodeid</param>
    /// <returns>If node doesn't exists, return value is null</returns>
    OpcUaNode GetNode(string nodeId);

    /// <summary>
    /// Write a value to a node
    /// </summary>
    /// <param name="nodeId">id of the representing OpcUaNode</param>
    /// <param name="payload">value to be written to the node</param>
    void WriteNode(string nodeId, object payload);

    /// <summary>
    /// Write a value to a node
    /// </summary>
    /// <param name="nodeId">id of the representing OpcUaNode</param>
    /// <param name="payload">value to be written to the node</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    Task WriteNodeAsync(string nodeId, object payload, CancellationToken cancellationToken = default);
}
