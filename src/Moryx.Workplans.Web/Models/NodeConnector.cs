// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Web.Models;

/// <summary>
/// Exchange type for workplan node connection 
/// </summary>
public class NodeConnector
{
    /// <summary>
    /// Node if of the connector.
    /// </summary>
    public long NodeId { get; set; }

    /// <summary>
    /// Index of the connector
    /// </summary>
    public int Index { get; set; }
}
