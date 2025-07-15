// Copyright (c) 2024, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Shifts
{
    /// <summary>
    /// Class representing the event arguments for a shift change.
    /// </summary>
    public class ShiftsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The type of change in the shift.
        /// </summary>
        public ShiftChange Change { get; set; }

        /// <summary>
        /// The shift that was changed.
        /// </summary>
        public Shift Shift { get; set; }

        /// <summary>
        /// Creates new event args with the given <paramref name="change"/> and <paramref name="shift"/> parameters
        /// </summary>
        public ShiftsChangedEventArgs(ShiftChange change, Shift shift)
        {
            Change = change;
            Shift = shift;
        }
    }
}
