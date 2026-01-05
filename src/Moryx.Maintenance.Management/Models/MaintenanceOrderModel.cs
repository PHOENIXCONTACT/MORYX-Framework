// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Maintenance.Management.Models;

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
