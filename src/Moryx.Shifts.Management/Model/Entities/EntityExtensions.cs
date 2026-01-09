// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Operators;

namespace Moryx.Shifts.Management.Model;

internal static class EntityExtensions
{
    extension(ShiftTypeEntity entity)
    {
        public ShiftType ToType()
        {
            return new ShiftType(entity.Name)
            {
                Id = entity.Id,
                StartTime = entity.StartTime,
                Endtime = entity.Endtime,
                Periode = entity.Periode
            };
        }

        public void Update(ShiftType type)
        {
            entity.Name = type.Name;
            entity.StartTime = type.StartTime;
            entity.Endtime = type.Endtime;
            entity.Periode = type.Periode;
        }
    }

    extension(ShiftEntity entity)
    {
        public Shift ToShift(ShiftType type)
        {
            return new Shift(type)
            {
                Id = entity.Id,
                Date = entity.Date
            };
        }

        public Shift ToShift(IEnumerable<ShiftType> types)
        {
            var type = types.SingleOrDefault(t => t.Id == entity.ShiftTypeId) ??
                       throw new KeyNotFoundException($"{nameof(ShiftEntity)} -Id: {entity.Id}- is referencing a {nameof(ShiftType)} " +
                                                      $"-Id: {entity.ShiftTypeId}- that was not loaded into memory.");

            return new Shift(type)
            {
                Id = entity.Id,
                Date = entity.Date
            };
        }

        public void Update(Shift shift)
        {
            entity.Date = shift.Date;
            entity.ShiftTypeId = shift.Type.Id;
        }
    }

    extension(ShiftAssignementEntity entity)
    {
        public ShiftAssignement ToAssignement(Shift shift, IResource resource, Operator @operator)
        {
            return new ShiftAssignement(resource, @operator, shift)
            {
                Id = entity.Id,
                Note = entity.Note,
                AssignedDays = entity.AssignedDays,
                Priority = entity.Priority
            };
        }

        public void Update(ShiftAssignement assignement)
        {
            entity.ShiftId = assignement.Shift.Id;
            entity.ResourceId = assignement.Resource.Id;
            entity.OperatorIdentifier = assignement.Operator.Identifier;
            entity.Note = assignement.Note;
            entity.Priority = assignement.Priority;
            entity.AssignedDays = assignement.AssignedDays;
        }

        public ShiftAssignement ToAssignement(IEnumerable<Shift> shifts,
            IResourceManagement resources, IOperatorManagement operators)
        {
            var shift = shifts.SingleOrDefault(s => s.Id == entity.ShiftId) ??
                        throw new KeyNotFoundException($"{nameof(ShiftAssignementEntity)} -Id: {entity.Id}- is referencing " +
                                                       $"a {nameof(Shift)} -Id: {entity.ShiftId}- that was not loaded into memory.");

            var resource = resources.GetResource<IResource>(entity.ResourceId) ??
                           throw new KeyNotFoundException($"{nameof(ShiftAssignementEntity)} -Id: {entity.Id}- is referencing " +
                                                          $"a {nameof(Resource)} -Id: {entity.ResourceId}- that was not available in the {nameof(IResourceManagement)}. " +
                                                          $"This might happen when the resource database was reset while the shifts database was not.");

            var @operator = operators.Operators.FirstOrDefault(x => x.Identifier == entity.OperatorIdentifier) ??
                            throw new KeyNotFoundException($"{nameof(ShiftAssignementEntity)} -Id: {entity.Id}- is referencing " +
                                                           $"an {nameof(Operator)} -Identifier: {entity.OperatorIdentifier}- that was not available in the {nameof(IOperatorManagement)}. " +
                                                           $"This might happen when the operators database was reset while the shifts database was not.");

            return new ShiftAssignement(resource, @operator, shift)
            {
                Id = entity.Id,
                Note = entity.Note,
                AssignedDays = entity.AssignedDays,
                Priority = entity.Priority
            };
        }
    }
}