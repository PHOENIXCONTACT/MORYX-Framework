// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Opc.Ua;

namespace Moryx.Drivers.OpcUa.Nodes;

internal static class NodeIdExtensions
{
    extension(NodeId nodeId)
    {
        public string ExpandedNodeIdString()
        {
            return OpcUaNode.CreateExpandedNodeId(nodeId.ToString());
        }
    }

    extension(ExpandedNodeId expandedNodeId)
    {
        public string ExpandedNodeIdString()
        {
            return OpcUaNode.CreateExpandedNodeId(expandedNodeId.ToString());
        }

        public NodeId ToNodeId(NamespaceTable namespaceTable)
        {
            return ExpandedNodeId.ToNodeId(expandedNodeId, namespaceTable);
        }
    }
}
