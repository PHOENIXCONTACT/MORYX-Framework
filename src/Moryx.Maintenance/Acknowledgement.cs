// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.Serialization;

namespace Moryx.Maintenance;

/// <summary>
/// Describes a maintenance that was acknowledge by a resource
/// </summary>
public class Acknowledgement
{
    /// <summary>
    /// Id of the Acknowledgement order
    /// </summary>
    [EntrySerialize]
    public long Id { get; set; }

    /// <summary>
    /// Operator working on the machine/resource during the acknowledgement 
    /// </summary>
    [EntrySerialize]
    public long OperatorId { get; set; }

    /// <summary>
    /// Description of the acknowledgement
    /// </summary>
    [EntrySerialize]
    public string? Description { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    [EntrySerialize]
    public DateTime Created { get; set; }
}
