// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Maintenance.Model.Entities;

namespace Moryx.Maintenance.Model.Mappers;

public static class AcknowledgementMapper
{
    public static AcknowledgementEntity ToEntity(this Acknowledgement model)
    {
        return new AcknowledgementEntity
        {
            Id = model.Id,
            Created = model.Created,
            Description = model.Description,
            OperatorId = model.OperatorId,
        };
    }

    public static Acknowledgement ToModel(this AcknowledgementEntity entity)
    {
        var model = new Acknowledgement
        {
            Id = entity.Id,
            Created = entity.Created,
            Description = entity.Description,
            OperatorId = entity.OperatorId
        };
        return model;
    }
}
