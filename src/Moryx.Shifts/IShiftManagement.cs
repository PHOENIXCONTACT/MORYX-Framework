// Copyright (c) 2024, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;

namespace Moryx.Shifts
{
    /// <summary>
    /// Interface of the shift management facade covering all available CRUD actions 
    /// to manage <see cref="Shift"/>s, <see cref="ShiftType"/>s and <see cref="ShiftAssignement"/>s.
    /// </summary>
    public interface IShiftManagement
    {
        /// <summary>
        /// Gets the list of shifts.
        /// </summary>
        IReadOnlyList<Shift> Shifts { get; }

        /// <summary>
        /// Creates a shift.
        /// </summary>
        Shift CreateShift(ShiftCreationContext context);
    
        /// <summary>
        /// Updates a shift.
        /// </summary>
        void UpdateShift(Shift shift);
    
        /// <summary>
        /// Deletes a shift.
        /// </summary>
        void DeleteShift(long id);

        /// <summary>
        /// Gets the list of shift types.
        /// </summary>
        IReadOnlyList<ShiftType> ShiftTypes { get; }
    
        /// <summary>
        /// Creates a shift type.
        /// </summary>
        ShiftType CreateShiftType(ShiftTypeCreationContext context);
    
        /// <summary>
        /// Updates a shift type.
        /// </summary>
        void UpdateShiftType(ShiftType type);
    
        /// <summary>
        /// Deletes a shift type.
        /// </summary>
        void DeleteShiftType(long id);

        /// <summary>
        /// Gets the list of shift assignments.
        /// </summary>
        IReadOnlyList<ShiftAssignement> ShiftAssignements { get; }
    
        /// <summary>
        /// Creates a shift assignment.
        /// </summary>
        ShiftAssignement CreateShiftAssignement(ShiftAssignementCreationContext context);

        /// <summary>
        /// Updates a shift assignement.
        /// </summary>
        void UpdateShiftAssignement(ShiftAssignement assignement);

        /// <summary>
        /// Deletes a shift assignment.
        /// </summary>
        void DeleteShiftAssignement(long id);

        /// <summary>
        /// Event triggered when a shift is changed.
        /// </summary>
        event EventHandler<ShiftsChangedEventArgs> ShiftsChanged;
    
        /// <summary>
        /// Event triggered when a shift type is changed.
        /// </summary>
        event EventHandler<ShiftTypesChangedEventArgs> TypesChanged;
    
        /// <summary>
        /// Event triggered when a shift assignment is changed.
        /// </summary>
        event EventHandler<ShiftAssignementsChangedEventArgs> AssignementsChanged;
    }
}
