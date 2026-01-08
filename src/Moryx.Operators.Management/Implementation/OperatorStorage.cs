// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Model.Repositories;
using Moryx.Operators.Management.Model;
using Moryx.Tools;

namespace Moryx.Operators.Management;

internal static class OperatorStorage
{
    public static OperatorEntity Save(IUnitOfWork<OperatorsContext> uow, OperatorData operatorData)
    {
        var operatorsRepo = uow.GetRepository<IOperatorEntityRepository>();

        var entity = operatorsRepo.GetByKey(operatorData.Id)
                     ?? operatorsRepo.Create();

        entity.Identifier = operatorData.Identifier;
        entity.FirstName = operatorData.FirstName;
        entity.LastName = operatorData.LastName;
        entity.Pseudonym = operatorData.Pseudonym;

        if (AttendancesChanged(operatorData, entity))
            SaveAttendances(operatorData, uow, entity);

        uow.SaveChanges();
        return entity;
    }

    private static bool AttendancesChanged(OperatorData operatorData, OperatorEntity entity) =>
        !((operatorData.AssignedResources.Count == entity.AssignedResources.Count) &&
          operatorData.AssignedResources.All(r => entity.AssignedResources.Any(e => e.ResourceId == r.Id)));

    private static void SaveAttendances(OperatorData operatorData, IUnitOfWork<OperatorsContext> uow, OperatorEntity entity)
    {
        var resourceLinksRepo = uow.GetRepository<IResourceLinkEntityRepository>();

        var toBeAdded = operatorData.AssignedResources
            .Where(r => entity.AssignedResources.All(e => e.ResourceId != r.Id)).ToArray();

        foreach (var assignedResources in toBeAdded)
        {
            var resourceLinkEntity = resourceLinksRepo.Create();
            resourceLinkEntity.ResourceId = assignedResources.Id;
            entity.AssignedResources.Add(resourceLinkEntity);
        }

        var toBeRemoved = entity.AssignedResources
            .Where(e => operatorData.AssignedResources.All(r => r.Id != e.ResourceId)).ToArray();

        resourceLinksRepo.RemoveRange(toBeRemoved);
        entity.AssignedResources.RemoveRange(toBeRemoved);
    }

    public static OperatorData Load(OperatorEntity entity, IResourceManagement resources)
    {
        var operatorData = entity.ToData(resources);
        return operatorData;
    }

    internal static void Delete(IUnitOfWork<OperatorsContext> uow, OperatorData operatorData)
    {
        var operatorRepo = uow.GetRepository<IOperatorEntityRepository>();

        var entity = operatorRepo.GetByKey(operatorData.Id);
        operatorRepo.Remove(entity);

        uow.SaveChanges();
    }
}
