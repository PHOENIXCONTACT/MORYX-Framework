// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Drivers.OpcUa.Nodes;
using Moryx.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa;

/// <summary>
/// Default implementation of <see cref="IOpcUaNodeReader"/> that incorporates `Opc.Ua` libraries
/// provided by the OPC UA Foundation.
/// </summary>
/// <param name="logger"></param>
/// <param name="driver"></param>
public class OpcUaNodeReader(IModuleLogger logger, IOpcUaDriver driver) : IOpcUaNodeReader
{
    private readonly IOpcUaDriver _driver = driver;

    private OpcUaNode ConvertToNode(Node node, NamespaceTable namespaceTable)
    {
        var opcUaNode = new OpcUaNode(_driver, logger, node.NodeId, namespaceTable)
        {
            DisplayName = node.DisplayName.ToString(),
            BrowseName = node.BrowseName
        };
        switch (node.NodeClass)
        {
            case NodeClass.Object:
                opcUaNode.NodeClass = NodeClass.Object;
                break;
            case NodeClass.Method:
                opcUaNode.NodeClass = NodeClass.Method;
                break;
            case NodeClass.Variable:
                opcUaNode.NodeClass = NodeClass.Variable;
                break;
            default: return null;
        }

        return opcUaNode;
    }

    /// <inheritdoc/>
    public async Task<OpcUaNode> ReadNodeAsync(string nodeId, NamespaceTable namespaceTable, ISession session, CancellationToken cancellationToken)
    {
        try
        {
            var node = await session.ReadNodeAsync(nodeId, cancellationToken);
            if (node == null)
            {
                return null;
            }
            var result = ConvertToNode(node, namespaceTable);
            return result;
        }
        catch
        {
            return null;
        }
    }
}
