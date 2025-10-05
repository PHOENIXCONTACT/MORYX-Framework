// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

using Opc.Ua;
using System.Collections.Generic;

namespace Moryx.Drivers.OpcUa.Nodes;

/// <summary>
/// Class to show Opc Ua Object Nodes on the UI
/// </summary>
public class OpcUaObjectDisplayNode(ExpandedNodeId nodeId) : OpcUaDisplayNode(nodeId)
{

    /// <summary>
    /// Subnodes of an object node
    /// </summary>
    public List<OpcUaDisplayNode> Nodes { get; set; } = [];
}
