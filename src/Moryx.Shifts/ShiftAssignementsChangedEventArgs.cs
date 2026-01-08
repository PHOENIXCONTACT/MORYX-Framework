// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Shifts
{
    /// <summary>
    /// Class representing the event arguments for a shift assignment change.
    /// </summary>
    public class ShiftAssignementsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The type of change in the shift assignment.
        /// </summary>
        public ShiftAssignementChange Change { get; set; }

        /// <summary>
        /// The shift assignment that was changed.
        /// </summary>
        public ShiftAssignement Assignement { get; set; }

        /// <summary>
        /// Creates new event args with the given <paramref name="change"/> and <paramref name="assignement"/> parameters
        /// </summary>
        public ShiftAssignementsChangedEventArgs(ShiftAssignementChange change, ShiftAssignement assignement)
        {
            Change = change;
            Assignement = assignement;
        }
    }
}
