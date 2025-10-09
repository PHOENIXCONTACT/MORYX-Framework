// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Operators;
using System.Collections.Generic;
using System.Linq;

namespace Moryx.Shifts.Management
{
    internal static class EntityExtensions
    {
        public static ShiftType ToType(this ShiftTypeEntity entity)
        {
            return new ShiftType(entity.Name)
            {
                Id = entity.Id,
                StartTime = entity.StartTime,
                Endtime = entity.Endtime,
                Periode = entity.Periode
            };
        }

        public static void Update(this ShiftTypeEntity entity, ShiftType type)
        {
            entity.Name = type.Name;
            entity.StartTime = type.StartTime;
            entity.Endtime = type.Endtime;
            entity.Periode = type.Periode;
        }

        public static Shift ToShift(this ShiftEntity entity, ShiftType type)
        {
            return new Shift(type)
            {
                Id = entity.Id,
                Date = entity.Date
            };
        }

        public static Shift ToShift(this ShiftEntity entity, IEnumerable<ShiftType> types)
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

        public static void Update(this ShiftEntity entity, Shift shift)
        {
            entity.Date = shift.Date;
            entity.ShiftTypeId = shift.Type.Id;
        }

        public static ShiftAssignement ToAssignement(this ShiftAssignementEntity entity, Shift shift, IResource resource, Operator @operator)
        {
            return new ShiftAssignement(resource, @operator, shift)
            {
                Id = entity.Id,
                Note = entity.Note,
                AssignedDays = entity.AssignedDays,
                Priority = entity.Priority
            };
        }

        public static void Update(this ShiftAssignementEntity entity, ShiftAssignement assignement)
        {
            entity.ShiftId = assignement.Shift.Id;
            entity.ResourceId = assignement.Resource.Id;
            entity.OperatorIdentifier = assignement.Operator.Identifier;
            entity.Note = assignement.Note;
            entity.Priority = assignement.Priority;
            entity.AssignedDays = assignement.AssignedDays;
        }

        public static ShiftAssignement ToAssignement(this ShiftAssignementEntity entity, IEnumerable<Shift> shifts,
            ResourceManagement resources, IOperatorManagement operators)
        {
            var shift = shifts.SingleOrDefault(s => s.Id == entity.ShiftId) ??
                throw new KeyNotFoundException($"{nameof(ShiftAssignementEntity)} -Id: {entity.Id}- is referencing " +
                $"a {nameof(Shift)} -Id: {entity.ShiftId}- that was not loaded into memory.");

            var resource = resources.GetResource<IResource>(entity.ResourceId) ??
                throw new KeyNotFoundException($"{nameof(ShiftAssignementEntity)} -Id: {entity.Id}- is referencing " +
                $"a {nameof(Resource)} -Id: {entity.ResourceId}- that was not available in the {nameof(ResourceManagement)}. " +
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
