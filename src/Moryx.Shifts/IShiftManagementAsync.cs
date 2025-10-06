// Copyright (c) 2024, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moryx.Shifts
{
    /// <summary>
    /// Interface of the shift management facade covering all available CRUD actions 
    /// to manage <see cref="Shift"/>s, <see cref="ShiftType"/>s and <see cref="ShiftAssignement"/>s.
    /// </summary>
    public interface IShiftManagementAsync
    {
        /// <summary>
        /// Gets the list of shifts.
        /// </summary>
        IReadOnlyList<Shift> Shifts { get; }

        /// <summary>
        /// Creates a shift.
        /// </summary>
        Task<Shift> CreateShiftAsync(ShiftCreationContext shift);

        /// <summary>
        /// Updates a shift.
        /// </summary>
        Task UpdateShiftAsync(Shift shift);

        /// <summary>
        /// Deletes a shift.
        /// </summary>
        Task DeleteShiftAsync(long id);

        /// <summary>
        /// Gets the list of shift types.
        /// </summary>
        IReadOnlyList<ShiftType> ShiftTypes { get; }

        /// <summary>
        /// Creates a shift type.
        /// </summary>
        Task<ShiftType> CreateShiftTypeAsync(ShiftTypeCreationContext shiftType);

        /// <summary>
        /// Updates a shift type.
        /// </summary>
        Task UpdateShiftTypeAsync(ShiftType shiftType);

        /// <summary>
        /// Deletes a shift type.
        /// </summary>
        Task DeleteShiftTypeAsync(long id);

        /// <summary>
        /// Gets the list of shift assignments.
        /// </summary>
        IReadOnlyList<ShiftAssignement> ShiftAssignements { get; }

        /// <summary>
        /// Creates a shift assignment.
        /// </summary>
        Task<ShiftAssignement> CreateShiftAssignementAsync(ShiftAssignementCreationContext shiftAssignement);

        /// <summary>
        /// Deletes a shift assignment.
        /// </summary>
        Task DeleteShiftAssignementAsync(long id);

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
