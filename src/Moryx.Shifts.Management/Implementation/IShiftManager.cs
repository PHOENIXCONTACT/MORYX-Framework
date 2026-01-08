// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Shifts.Management
{
    internal interface IShiftManager : IPlugin
    {
        IReadOnlyList<Shift> Shifts { get; }
        Shift CreateShift(ShiftCreationContext context);
        void UpdateShift(Shift shift);
        void DeleteShift(Shift shift);

        IReadOnlyList<ShiftType> ShiftTypes { get; }
        ShiftType CreateShiftType(ShiftTypeCreationContext context);
        void UpdateShiftType(ShiftType type);
        void DeleteShiftType(ShiftType type);

        IReadOnlyList<ShiftAssignement> ShiftAssignements { get; }
        ShiftAssignement CreateShiftAssignement(ShiftAssignementCreationContext context);
        void UpdateShiftAssignement(ShiftAssignement assignement);
        void DeleteShiftAssignement(ShiftAssignement assignement);

        event EventHandler<ShiftsChangedEventArgs> ShiftsChanged;
        event EventHandler<ShiftTypesChangedEventArgs> ShiftTypesChanged;
        event EventHandler<ShiftAssignementsChangedEventArgs> ShiftAssignementsChanged;
    }
}

