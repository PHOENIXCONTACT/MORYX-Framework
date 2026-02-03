// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Drivers.OpcUa.Nodes;
using Opc.Ua;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa;

internal interface IOpcUaNodeReader
{
    /// <summary>
    /// Reads node attributes from an OPC UA server
    /// </summary>
    /// <param name="nodeId"></param>
    /// <param name="namespaceTable"></param>
    /// <param name="session"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<OpcUaNode> ReadNodeAsync(string nodeId, NamespaceTable namespaceTable, ISession session, CancellationToken cancellationToken);
}
