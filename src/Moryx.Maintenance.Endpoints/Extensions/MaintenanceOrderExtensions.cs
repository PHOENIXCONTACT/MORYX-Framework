// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.Maintenance.Endpoints.Dtos;
using Moryx.Maintenance.Management.Models;
using Moryx.Serialization;

namespace Moryx.Maintenance.Endpoints.Extensions;

internal static class MaintenanceOrderExtensions
{

    public static Entry ToEntry(this MaintenanceOrderResponse dto, ICustomSerialization serialization)
    {
        return EntryConvert.EncodeObject(dto, serialization);
    }

    public static MaintenanceOrderResponse ToOrderEntry(this MaintenanceOrderModel dto)
        => new()
        {
            Resource = dto.Resource.Name,
            Description = dto.Description,
            Block = dto.Block,
            IsActive = dto.IsActive,
            Created = dto.Created,
            Instructions = [.. dto.Instructions],
            Interval = dto.Interval.Interval,
        };
}
