// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using Opc.Ua;

namespace Moryx.Drivers.OpcUa;

/// <summary>
/// Class to show Opc Ua Nodes on the UI, since OpcUaNode is not serializable
/// </summary>
internal class OpcUaDisplayNode
{
    private readonly ExpandedNodeId _nodeId;
    public OpcUaDisplayNode(ExpandedNodeId nodeId)
    {
        _nodeId = nodeId;
    }

    public OpcUaDisplayNode()
    {

    }

    /// <summary>
    /// Id of the Opc Ua Node
    /// </summary>
    public string NodeId => _nodeId == null ? string.Empty : _nodeId.ToString();

    /// <summary>
    /// Display Name of the Opc Ua Node
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Browser Name of the Opc Ua Node
    /// </summary>
    public string BrowseName { get; set; }

    /// <summary>
    /// Class Type of the Opc Ua Node
    /// </summary>
    public NodeClass ClassType { get; set; }

    /// <summary>
    /// Description of the Opc Ua Node
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Returns string representive of the NodeIf
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var id = new ExpandedNodeId(_nodeId.Identifier, _nodeId.NamespaceIndex, "", _nodeId.ServerIndex);
        return id.Format(CultureInfo.InvariantCulture) + " - " + DisplayName;
    }
}
