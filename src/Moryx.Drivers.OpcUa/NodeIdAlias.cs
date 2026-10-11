// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Drivers.OpcUa;

/// <summary>
/// Key-Value pair of an alias for a node id
/// </summary>
public class NodeIdAlias
{
    /// <summary>
    /// Alias name
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Node id to be aliased
    /// </summary>
    public string NodeId { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Alias}={NodeId}";
    }
}
