// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Shifts.Management.Model
{
    public interface IShiftRepository : IRepository<ShiftEntity>
    {
        ShiftEntity CreateFromContext(ShiftCreationContext context);
    }

    public abstract class ShiftRepository : ModificationTrackedRepository<ShiftEntity>, IShiftRepository
    {
        public ShiftEntity CreateFromContext(ShiftCreationContext context)
        {
            return DbSet.Add(From(context)).Entity;
        }

        private static ShiftEntity From(ShiftCreationContext context)
        {
            return new ShiftEntity()
            {
                ShiftTypeId = context.Type.Id,
                Date = context.Date,
            };
        }
    }
}

