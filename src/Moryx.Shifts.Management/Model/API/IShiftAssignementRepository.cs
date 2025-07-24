﻿// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;
using System.Threading.Tasks;

namespace Moryx.Shifts.Management.Model
{
    public interface IShiftAssignementRepository : IRepository<ShiftAssignementEntity>
    {
        ShiftAssignementEntity CreateFromContext(ShiftAssignementCreationContext context);

        Task<ShiftAssignementEntity> CreateFromContextAsync(ShiftAssignementCreationContext context);
    }

    public abstract class ShiftAssignementRepository : ModificationTrackedRepository<ShiftAssignementEntity>, IShiftAssignementRepository
    {
        public ShiftAssignementEntity CreateFromContext(ShiftAssignementCreationContext context)
        {
            return DbSet.Add(From(context)).Entity;
        }

        public async Task<ShiftAssignementEntity> CreateFromContextAsync(ShiftAssignementCreationContext context)
        {
            var entry = await DbSet.AddAsync(From(context));
            return entry.Entity;
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

