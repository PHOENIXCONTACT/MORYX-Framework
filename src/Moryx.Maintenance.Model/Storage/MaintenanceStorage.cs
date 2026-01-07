// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Maintenance.Model.Entities;
using Moryx.Model.Repositories;
using Moryx.Maintenance.Model.Mappers;
namespace Moryx.Maintenance.Model.Storage;

/// <summary>
/// Storage operations for maintenance orders
/// </summary>
public class MaintenanceStorage
{
    /// <summary>
    /// Saves the given maintenance order
    /// </summary>
    /// <param name="uow">Unit of work for database operations</param>
    /// <param name="dto">Maintenance order data transfer object</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Saved maintenance order entity</returns>
    public static async Task<MaintenanceOrderEntity> SaveAsync(IUnitOfWork uow, MaintenanceOrder dto, CancellationToken cancellationToken)
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

    /// <summary>
    /// Loads the maintenance order model from the given entity
    /// </summary>
    /// <returns></returns> /// <returns>Saved maintenance order model</returns>
    public static MaintenanceOrder Load(MaintenanceOrderEntity entity, IEnumerable<IMaintainableResource> resources)
    {
        var model = entity.ToModel(resources);
        return model;
    }

    /// <summary>
    /// Deletes the maintenance order with the given id
    /// </summary>
    /// <param name="uow">Unit of work for database operations</param>
    /// <param name="id">Id of the maintenance order to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public static async Task DeleteAsync(IUnitOfWork uow, long id, CancellationToken cancellationToken)
    {
        var repos = uow.GetRepository<IMaintenanceOrderRepository>();
        var entity = repos.GetByKey(id);
        repos.Remove(entity);
        await uow.SaveChangesAsync(cancellationToken);
    }

}
