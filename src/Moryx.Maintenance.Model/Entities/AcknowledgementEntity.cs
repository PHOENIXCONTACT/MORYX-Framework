// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;

namespace Moryx.Maintenance.Model.Entities;

public class AcknowledgementEntity : ModificationTrackedEntityBase
{

    public long OperatorId { get; set; }
    public string? Description { get; set; }

}
