// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Shifts.Management
{
    internal interface IShiftStorage : IPlugin
    {
        IEnumerable<ShiftAssignement> GetAssignements(IEnumerable<Shift> _shifts);
        IEnumerable<Shift> GetShifts(IEnumerable<ShiftType> _types);
        IEnumerable<ShiftType> GetTypes();

        Shift Create(ShiftCreationContext context);
        void Update(Shift shift);
        void Delete(Shift shift);

        ShiftType Create(ShiftTypeCreationContext context);
        void Update(ShiftType type);
        void Delete(ShiftType type);

        ShiftAssignement Create(ShiftAssignementCreationContext context);
        void Update(ShiftAssignement assignement);
        void Delete(ShiftAssignement assignement);
    }
}
