// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Web.Models;

/// <summary>
/// Exchange type for node connection point
/// </summary>
public class NodeConnectionPoint
{
    /// <summary>
    /// Index of the connection point
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Name of the connection
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// List of connectors
    /// </summary>
    public List<NodeConnector> Connections { get; set; } = new List<NodeConnector>();
}
