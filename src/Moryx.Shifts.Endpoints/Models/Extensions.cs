// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Operators;

namespace Moryx.Shifts.Endpoints
{
    internal static class Extensions
    {
        public static ShiftModel ToModel(this Shift shift) => 
            new() { Id = shift.Id, TypeId = shift.Type.Id, Date = shift.Date };

        public static ShiftTypeModel ToModel(this ShiftType type) => 
            new() { Id = type.Id, Name = type.Name, StartTime = type.StartTime, Endtime = type.Endtime, Periode = type.Periode };

        public static ShiftAssignementModel ToModel(this ShiftAssignement assignement) => 
            new() { Id = assignement.Id, ShiftId = assignement.Shift.Id, ResourceId = assignement.Resource.Id, OperatorIdentifier = assignement.Operator.Identifier, 
                Note = assignement.Note, Priority = assignement.Priority, AssignedDays = assignement.AssignedDays };

        public static Shift FromModel(this ShiftModel model, ShiftType type) => 
            new(type) { Id = model.Id, Date = model.Date };

        public static ShiftType FromModel(this ShiftTypeModel model) =>
            new(model.Name!) { Id = model.Id, StartTime = model.StartTime, Endtime = model.Endtime, Periode = model.Periode };
        
        public static ShiftAssignement FromModel(this ShiftAssignementModel model, Shift shift, IResource resource, Operator @operator) =>
            new(resource, @operator, shift) { Id = model.Id, Priority = model.Priority, Note = model.Note, AssignedDays = model.AssignedDays };

        public static ShiftCreationContext FromModel(this ShiftCreationContextModel model, ShiftType type) => 
            new(type) { Date = model.Date };

        public static ShiftTypeCreationContext FromModel(this ShiftTypeCreationContextModel model) => 
            new(model.Name!) { StartTime = model.StartTime, EndTime = model.Endtime, Periode = model.Periode };

        public static ShiftAssignementCreationContext FromModel(this ShiftAssignementCreationContextModel model, Shift shift, IResource resource, Operator @operator) =>
            new(shift, resource, @operator) { Priority = model.Priority, Note = model.Note, AssignedDays = model.AssignedDays };
    }
}

