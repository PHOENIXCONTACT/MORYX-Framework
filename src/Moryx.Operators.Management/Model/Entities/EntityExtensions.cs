// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Operators.Attendances;

namespace Moryx.Operators.Management.Model;

internal static class EntityExtensions
{
    public static OperatorData ToData(this OperatorEntity entity, IResourceManagement resources)
    {
        var data = new OperatorData(entity.Identifier)
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Pseudonym = entity.Pseudonym,
        };
        var assignables = resources.GetResources<IOperatorAssignable>()
            .Where(r => entity.AssignedResources.Any(e => e.ResourceId == r.Id));
        data.AssignedResources.AddRange(assignables);
        return data;
    }
}
