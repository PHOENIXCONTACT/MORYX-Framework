// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.VisualInstructions;

namespace Moryx.Maintenance.Management.Models;

/// <summary>
/// Model for a maintenance order
/// </summary>
/// <param name="Id"></param>
/// <param name="Resource"></param>
/// <param name="Description"></param>
/// <param name="Interval"></param>
/// <param name="Instructions"></param>
/// <param name="Block"></param>
/// <param name="IsActive"></param>
/// <param name="Created"></param>
/// <param name="Acknowledgements"></param>
/// <param name="MaintenanceStarted"></param>
public record MaintenanceOrderModel(
    long Id,
    ResourceModel Resource,
    string? Description,
    IntervalModel Interval,
    List<VisualInstruction> Instructions,
    bool Block,
    bool IsActive,
    DateTime Created,
    List<Acknowledgement> Acknowledgements,
    bool MaintenanceStarted);
