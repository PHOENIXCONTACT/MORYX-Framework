// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Model.Repositories;
using Moryx.Operators.Management.Model;
using Moryx.Tools;

namespace Moryx.Operators.Management;

internal static class OperatorStorage
{
    public static OperatorEntity Save(IUnitOfWork uow, OperatorData operatorData)
    {
        using var context = (OperatorsContext)uow.DbContext;

        var entity = context.OperatorEntities.Find(operatorData.Id);
        entity ??= context.OperatorEntities.Add(new OperatorEntity()).Entity;

        entity.Identifier = operatorData.Identifier;
        entity.FirstName = operatorData.FirstName;
        entity.LastName = operatorData.LastName;
        entity.Pseudonym = operatorData.Pseudonym;

        if (AttandancesChanged(operatorData, entity))
            SaveAttandances(operatorData, context, entity);

        uow.SaveChanges();
        return entity;
    }

    private static bool AttandancesChanged(OperatorData operatorData, OperatorEntity entity) =>
        !((operatorData.AssignedResources.Count == entity.AssignedResources.Count) &&
        operatorData.AssignedResources.All(r => entity.AssignedResources.Any(e => e.ResourceId == r.Id)));

    private static void SaveAttandances(OperatorData operatorData, OperatorsContext context, OperatorEntity entity)
    {
        var toBeAdded = operatorData.AssignedResources
                        .Where(r => !entity.AssignedResources.Any(e => e.ResourceId == r.Id))
                        .Select(r => new ResourceLinkEntity() { ResourceId = r.Id });
        context.ResourceLinkEntities.AddRange(toBeAdded);

        var toBeRemoved = entity.AssignedResources
            .Where(e => !operatorData.AssignedResources.Any(r => r.Id == e.ResourceId));
        context.ResourceLinkEntities.RemoveRange(toBeRemoved);

        entity.AssignedResources ??= [];
        entity.AssignedResources.RemoveRange(toBeRemoved);
        entity.AssignedResources.AddRange(toBeAdded);
    }

    public static OperatorData Load(OperatorEntity entity, ResourceManagement resources)
    {
        var operatorData = entity.ToData(resources);
        return operatorData;
    }

    internal static void Delete(IUnitOfWork<OperatorsContext> uow, OperatorData operatorData)
    {
        using var context = uow.DbContext;

        var entity = context.OperatorEntities.First(o => o.Id == operatorData.Id);
        context.Remove(entity);

        uow.SaveChanges();
    }
}
