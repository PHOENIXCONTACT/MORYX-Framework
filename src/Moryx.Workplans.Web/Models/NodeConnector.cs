// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Web.Models;

/// <summary>
/// Exchange type for workplan node connection 
/// </summary>
public class NodeConnector
{
    /// <inheritdoc cref="NodeConnector.NodeId" />
    public long NodeId { get; set; }

    /// <inheritdoc cref="NodeConnector.Index" />
    public int Index { get; set; }
}
