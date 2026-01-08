// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Opc.Ua;

namespace Moryx.Drivers.OpcUa;

/// <summary>
/// Class to show Opc Ua Object Nodes on the UI
/// </summary>
internal class OpcUaObjectDisplayNode(ExpandedNodeId nodeId) : OpcUaDisplayNode(nodeId)
{

    /// <summary>
    /// Subnodes of an object node
    /// </summary>
    public List<OpcUaDisplayNode> Nodes { get; set; } = [];
}
