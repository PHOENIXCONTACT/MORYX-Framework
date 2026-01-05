// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.ControlSystem.VisualInstructions;

namespace Moryx.Maintenance;

/// <summary>
/// Represent a start maintenance 'command'
/// </summary>
public class MaintenanceOrderStart
{
    /// <summary>
    /// Id of the maintenance Order
    /// </summary>
    public long OrderId { get; set; }

    /// <summary>
    /// List of instruction to perform a maintenance
    /// </summary>
    public IEnumerable<VisualInstruction> Instructions { get; set; } = [];
}
