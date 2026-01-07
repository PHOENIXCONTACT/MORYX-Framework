// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.VisualInstructions;
using Moryx.Model;

namespace Moryx.Maintenance.Model.Entities;

public class MaintenanceOrderEntity : ModificationTrackedEntityBase
{
    public long ResourceId { get; set; }

    public string? IntervalType { get; set; }

    public string? IntervalData { get; set; }

    public string? Description { get; set; }

    public List<VisualInstruction> Instructions { get; set; } = [];


    public bool Block { get; set; }

    public bool IsActive { get; set; }

    public virtual List<AcknowledgementEntity> Acknowledgements { get; set; } = [];
}
