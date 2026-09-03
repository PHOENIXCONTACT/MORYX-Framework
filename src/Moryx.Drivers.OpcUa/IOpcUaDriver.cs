// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers.InOut;
using Moryx.AbstractionLayer.Drivers.Message;
using Moryx.Drivers.OpcUa.Nodes;

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
    /// Will be replaced by <see cref="IOpcUaDriverAddSubscription.AddSubscriptionAsync(string, CancellationToken)"/>
    /// </summary>
    /// <param name="node">OpcUaNode to be subscribed</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    [Obsolete("This will be removed in a future version.")]
    Task AddSubscriptionAsync(OpcUaNode node, CancellationToken cancellationToken = default);

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
    /// This feature will be removed in a future version without any replacement.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    [Obsolete("Browsing nodes won't be a feature anymore and removed in a future version")]
    Task RebrowseNodesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns an opcUaNode to a string
    /// </summary>
    /// <param name="nodeId">NodeId of the requested node</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <returns>If node doesn't exists, return value is null</returns>
    Task<OpcUaNode> GetNodeAsync(string nodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Write a value to a node
    /// </summary>
    /// <param name="nodeId">Id of the representing OpcUaNode</param>
    /// <param name="payload">Value to be written to the node</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    Task WriteNodeAsync(string nodeId, object payload, CancellationToken cancellationToken = default);
}

/// <summary>
/// Opc Ua Client
/// </summary>
public interface IOpcUaDriverAddSubscription : IOpcUaDriver
{
    /// <summary>
    /// Subscribes to a variable node. Nothing happens, if the node is not a variable
    /// </summary>
    /// <param name="node">Node ID to be subscribed</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    Task AddSubscriptionAsync(string node, CancellationToken cancellationToken = default);
}
