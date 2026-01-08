// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;

namespace Moryx.Shifts
{
    public static class ShiftManagementExtensions
    {
        public static Shift? GetShift(this IShiftManagement source, long id) => source.Shifts.SingleOrDefault(s => s.Id == id);

        public static IEnumerable<Shift> GetShifts(this IShiftManagement source, DateOnly? earliestDate, DateOnly? latestDate)
        {
            IEnumerable<Shift> shifts = source.Shifts;
            if (!earliestDate.HasValue && !latestDate.HasValue) return shifts;

            if (earliestDate.HasValue)
            {
                shifts = shifts.Where(s => s.Date.AddDays(s.Type.Periode) >= earliestDate);
            }
            if (latestDate.HasValue)
            {
                shifts = shifts.Where(s => s.Date <= latestDate);
            }
            return shifts;
        }

        public static ShiftType? GetShiftType(this IShiftManagement source, long id) => source.ShiftTypes.SingleOrDefault(t => t.Id == id);

        public static IEnumerable<ShiftAssignement> GetShiftAssignements(this IShiftManagement source, DateOnly? earliestDate, DateOnly? latestDate)
        {
            if (!earliestDate.HasValue && !latestDate.HasValue) return source.ShiftAssignements;

            IEnumerable<Shift> shifts = source.GetShifts(earliestDate, latestDate);
            return source.ShiftAssignements.Where(a => shifts.Contains(a.Shift));
        }
    }
}

