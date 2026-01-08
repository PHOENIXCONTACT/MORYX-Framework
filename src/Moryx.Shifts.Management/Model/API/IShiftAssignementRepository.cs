// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Shifts.Management.Model
{
    public interface IShiftAssignementRepository : IRepository<ShiftAssignementEntity>
    {
        ShiftAssignementEntity CreateFromContext(ShiftAssignementCreationContext context);
    }

    public abstract class ShiftAssignementRepository : ModificationTrackedRepository<ShiftAssignementEntity>, IShiftAssignementRepository
    {
        public ShiftAssignementEntity CreateFromContext(ShiftAssignementCreationContext context)
        {
            return DbSet.Add(From(context)).Entity;
        }

        private static ShiftAssignementEntity From(ShiftAssignementCreationContext context)
        {
            return new ShiftAssignementEntity()
            {
                ShiftId = context.Shift.Id,
                ResourceId = context.Resource.Id,
                OperatorIdentifier = context.Operator.Identifier,
                Priority = context.Priority,
                Note = context.Note,
                AssignedDays = context.AssignedDays,
            };
        }
    }
}

