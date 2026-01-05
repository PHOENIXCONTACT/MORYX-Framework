// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.Json;
using Moryx.Maintenance.IntervalTypes;
using Moryx.Maintenance.Model.Entities;

namespace Moryx.Maintenance.Model.Mappers;

public static class MaintenanceOrderMapper
{
    public static MaintenanceOrderEntity ToEntity(this MaintenanceOrder model)
    {
        var entity = new MaintenanceOrderEntity
        {
            Description = model.Description,
            Instructions = [.. model.Instructions],
            Created = model.Created,
            IsActive = model.IsActive,
            Id = model.Id,
            Block = model.Block,
            IntervalType = model.Interval?.GetType().AssemblyQualifiedName,
            IntervalData = JsonSerializer.Serialize(model.Interval),
            ResourceId = model.Resource?.Id ?? -1
        };

        return entity;
    }
    public static MaintenanceOrder ToModel(this MaintenanceOrderEntity entity, IEnumerable<IMaintainableResource> resources)
    {
        var model = new MaintenanceOrder
        {
            Resource = resources.FirstOrDefault(x => x.Id == entity.ResourceId),
            Description = entity.Description,
            Instructions = entity.Instructions,
            Created = entity.Created,
            IsActive = entity.IsActive,
            Acknowledgements = [.. entity.Acknowledgements.Select(x => x.ToModel())],
            Id = entity.Id,
            Block = entity.Block,
            Interval = !string.IsNullOrEmpty(entity.IntervalType) && !string.IsNullOrEmpty(entity.IntervalData)
            ? JsonSerializer.Deserialize(entity.IntervalData!, Type.GetType(entity.IntervalType)!) as IntervalBase
            : null
        };
        model.IsActive = entity.IsActive;

        return model;
    }
}
