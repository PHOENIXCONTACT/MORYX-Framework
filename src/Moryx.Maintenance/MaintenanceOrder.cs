// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.VisualInstructions;

namespace Moryx.Maintenance;

/// <summary>
/// Describes a maintenance order that can be sent to a machine
/// </summary>
public class MaintenanceOrder
{
    /// <summary>
    /// OrderId of the Order
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Resource that needs the maintenance
    /// </summary>
    public IMaintainableResource? Resource { get; set; }

    /// <summary>
    /// Description of the maintenance
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Recurring interval at which the maintenance should be performed
    /// </summary>
    public IntervalBase? Interval { get; set; }

    /// <summary>
    /// List of instruction to perform a maintenance on <see cref="Resource"/>
    /// </summary>
    public IEnumerable<VisualInstruction> Instructions { get; set; } = [];

    /// <summary>
    /// Blocks the resource if the maintenance is due
    /// </summary>
    public bool Block { get; set; }

    /// <summary>
    /// Status of the maintenance order
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indicates if the maintenance has already started
    /// </summary>
    public bool MaintenanceStarted { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// List of acknowledgement (aka. performed maintenance)
    /// </summary>
    public IEnumerable<Acknowledgement> Acknowledgements { get; set; } = [];

}
