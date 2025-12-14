// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Shifts.Management.Model
{
    public interface IShiftTypeRepository : IRepository<ShiftTypeEntity>
    {
        ShiftTypeEntity CreateFromContext(ShiftTypeCreationContext context);
    }

    public abstract class ShiftTypeRepository : ModificationTrackedRepository<ShiftTypeEntity>, IShiftTypeRepository
    {
        public ShiftTypeEntity CreateFromContext(ShiftTypeCreationContext context)
        {
            return DbSet.Add(From(context)).Entity;
        }

        private static ShiftTypeEntity From(ShiftTypeCreationContext context)
        {
            return new ShiftTypeEntity()
            {
                Name = context.Name,
                StartTime = context.StartTime,
                Endtime = context.EndTime,
                Periode = context.Periode,
            };
        }
    }
}

