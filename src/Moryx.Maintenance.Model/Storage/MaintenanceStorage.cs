// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Maintenance.Model.API;
using Moryx.Maintenance.Model.Entities;
using Moryx.Model.Repositories;
using Moryx.Maintenance.Model.Mappers;
namespace Moryx.Maintenance.Model.Storage;

public class MaintenanceStorage
{
    public static async Task<MaintenanceOrderEntity> Save(IUnitOfWork uow, MaintenanceOrder dto)
    {
        var repos = uow.GetRepository<IMaintenanceOrderRepository>();
        var entity = repos.GetByKey(dto.Id);
        entity ??= new MaintenanceOrderEntity();

        var toEntity = dto.ToEntity();
        entity.Block = toEntity.Block;
        entity.IsActive = toEntity.IsActive;
        entity.Acknowledgements = toEntity.Acknowledgements;
        entity.Description = toEntity.Description;
        entity.Instructions = toEntity.Instructions;
        entity.IntervalData = toEntity.IntervalData;
        entity.IntervalType = toEntity.IntervalType;
        entity.ResourceId = toEntity.ResourceId;
        try
        {
            if (entity.Id <= 0)
            {
                entity = repos.Add(entity);
            }
            await uow.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
        return entity;
    }

    public static MaintenanceOrder Load(MaintenanceOrderEntity entity, IEnumerable<IMaintainableResource> resources)
    {
        var model = entity.ToModel(resources);
        return model;
    }

    public static async Task Delete(IUnitOfWork uow, long id)
    {
        var repos = uow.GetRepository<IMaintenanceOrderRepository>();
        var entity = repos.GetByKey(id);
        repos.Remove(entity);
        await uow.SaveChangesAsync();
    }

}
