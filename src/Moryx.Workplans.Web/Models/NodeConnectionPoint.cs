// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Web.Models;

/// <summary>
/// Exchange type for node connection point
/// </summary>
public class NodeConnectionPoint
{
    /// <inheritdoc cref="NodeConnectionPoint.Index" />
    public int Index { get; set; }

    /// <inheritdoc cref="NodeConnectionPoint.Name" />
    public string Name { get; set; }

    /// <inheritdoc cref="NodeConnectionPoint.Connections" />
    public List<NodeConnector> Connections { get; set; } = new List<NodeConnector>();
}
