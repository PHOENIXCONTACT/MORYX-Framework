// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.Drivers.InOut;
using Moryx.AbstractionLayer.Drivers.Message;

namespace Moryx.Drivers.OpcUa;

/// <summary>
/// Opc Ua Client
/// </summary>
public interface IOpcUaDriver : IDriver, IMessageDriver<object>, IMessageDriver<OpcUaMessage>, IInOutDriver<object, object>
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
    /// <param name="NodeId">NodeId</param>
    /// <returns>If node doesn't exists or there was an error, when trying to read
    /// the node, the return value will be null</returns>       
    object ReadNode(string NodeId);

    /// <summary>
    /// Rebrowse Nodes
    /// </summary>
    void RebrowseNodes();
}

public interface IOpcUaDriver2 : IOpcUaDriver
{

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
    /// Invokes the method of a method noe
    /// </summary>
    /// <param name="nodeId"></param>
    /// <param name="parameters"></param>
    /// <returns>returns null, if type of the node doesn't fit or node not found</returns>
    List<object> InvokeMethod(string nodeId, object[] parameters);
}

public static class OpcUaDriverExtensions
{
    public static OpcUaNode GetNode(this IOpcUaDriver driver, string nodeId)
    {
        if (driver is IOpcUaDriver2 d2)
        {
            return d2.GetNode(nodeId);
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    public static void WriteNode(this IOpcUaDriver driver, string nodeId, object payload)
    {
        if (driver is IOpcUaDriver2 d2)
        {
            d2.WriteNode(nodeId, payload);
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    public static List<object> InvokeMethod(this IOpcUaDriver driver, string nodeId, object[] parameters)
    {
        if (driver is IOpcUaDriver2 d2)
        {
            return d2.InvokeMethod(nodeId, parameters);
        }
        else
        {
            throw new NotSupportedException();
        }
    }

}
