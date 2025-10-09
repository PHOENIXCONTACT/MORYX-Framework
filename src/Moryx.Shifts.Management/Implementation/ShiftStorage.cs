// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Model.Repositories;
using Moryx.Operators;
using Moryx.Shifts.Management.Model;
using Moryx.Tools;
using System.Collections.Generic;
using System.Linq;

namespace Moryx.Shifts.Management
{
    [Component(LifeCycle.Singleton, typeof(IShiftStorage))]
    internal class ShiftStorage : IShiftStorage, ILoggingComponent
    {
        #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IModuleLogger Logger { get; set; }

        public ResourceManagement Resources { get; set; }

        public IOperatorManagement Operators { get; set; }

        public IUnitOfWorkFactory<ShiftsContext> UnitOfWorkFactory { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        #endregion

        #region Lifecylce

        public void Start()
        {
            //Todo: Add parallel operation with cancelation tokens to mark shifts as obsolete
            //when they lie in the past as soon as the IShiftHistory facade is available
        }

        public void Stop()
        {
        }

        #endregion

        #region IShiftStorage

        public IEnumerable<ShiftAssignement> GetAssignements(IEnumerable<Shift> shifts)
        {
            using var uow = UnitOfWorkFactory.Create();
            var repo = uow.GetRepository<IShiftAssignementRepository>();
            var assignements = repo.GetAll().Where(s => s.State != ShiftState.Obsolete) //repo.Linq.Where(a => a.State != ShiftState.Obsolete)
                .Select(e => e.ToAssignement(shifts, Resources, Operators)).ToList();
            Logger.LogDebug("Retrieved {count} {name}s from the database.", assignements.Count, nameof(ShiftAssignement));
            return assignements;
        }

        public IEnumerable<Shift> GetShifts(IEnumerable<ShiftType> types)
        {
            using var uow = UnitOfWorkFactory.Create();
            var repo = uow.GetRepository<IShiftRepository>();
            var shifts = repo.GetAll().Where(s => s.State != ShiftState.Obsolete) // repo.Linq.Where(s => s.State != ShiftState.Obsolete)
                .Select(e => e.ToShift(types)).ToList();
            Logger.LogDebug("Retrieved {count} {name}s from the database.", shifts.Count, nameof(Shift));
            return shifts;
        }

        public IEnumerable<ShiftType> GetTypes()
        {
            using var uow = UnitOfWorkFactory.Create();
            var repo = uow.GetRepository<IShiftTypeRepository>();
            var types = repo.GetAll().Select(e => e.ToType()).ToList();
            Logger.LogDebug("Retrieved {count} {name}s from the database.", types.Count, nameof(ShiftType));
            return types;
        }

        public Shift Create(ShiftCreationContext context)
        {
            using var uow = UnitOfWorkFactory.Create();
            var repo = uow.GetRepository<IShiftRepository>();
            var entity = repo.CreateFromContext(context);
            uow.SaveChanges();
            return entity.ToShift(context.Type);
        }

        public void Update(Shift shift)
        {
            using var uow = UnitOfWorkFactory.Create();
            var repo = uow.GetRepository<IShiftRepository>();
            repo.GetByKey(shift.Id).Update(shift);
            uow.SaveChanges();
        }

        public void Delete(Shift shift)
        {
            using var uow = UnitOfWorkFactory.Create();
            var assignementsRepo = uow.GetRepository<IShiftAssignementRepository>();
            assignementsRepo.Linq.Where(a => a.ShiftId == shift.Id)
                .ForEach(a => assignementsRepo.Remove(a));

            var repo = uow.GetRepository<IShiftRepository>();
            var entity = repo.GetByKey(shift.Id);
            repo.Remove(entity);
            uow.SaveChanges();
        }

        public ShiftType Create(ShiftTypeCreationContext context)
        {
            using var uow = UnitOfWorkFactory.Create();
            var repo = uow.GetRepository<IShiftTypeRepository>();
            var entity = repo.CreateFromContext(context);
            uow.SaveChanges();
            return entity.ToType();
        }

        public void Update(ShiftType type)
        {
            using var uow = UnitOfWorkFactory.Create();
            var repo = uow.GetRepository<IShiftTypeRepository>();
            repo.GetByKey(type.Id).Update(type);
            uow.SaveChanges();
        }

        public void Delete(ShiftType type)
        {
            using var uow = UnitOfWorkFactory.Create();
            var shiftsRepo = uow.GetRepository<IShiftRepository>();
            shiftsRepo.Linq.Where(s => s.ShiftTypeId == type.Id)
                .ForEach(s => s.State = ShiftState.Obsolete);

            var repo = uow.GetRepository<IShiftTypeRepository>();
            var entity = repo.GetByKey(type.Id);
            repo.Remove(entity);
            uow.SaveChanges();
        }

        public ShiftAssignement Create(ShiftAssignementCreationContext context)
        {
            using var uow = UnitOfWorkFactory.Create();
            var repo = uow.GetRepository<IShiftAssignementRepository>();
            var entity = repo.CreateFromContext(context);
            uow.SaveChanges();
            return entity.ToAssignement(context.Shift, context.Resource, context.Operator);
        }

        public void Update(ShiftAssignement assignement)
        {
            using var uow = UnitOfWorkFactory.Create();
            var repo = uow.GetRepository<IShiftAssignementRepository>();
            repo.GetByKey(assignement.Id).Update(assignement);
            uow.SaveChanges();
        }

        public void Delete(ShiftAssignement assignement)
        {
            using var uow = UnitOfWorkFactory.Create();
            var repo = uow.GetRepository<IShiftAssignementRepository>();
            var entity = repo.GetByKey(assignement.Id);
            repo.Remove(entity);
            uow.SaveChanges();
        }
        #endregion
    }
}
