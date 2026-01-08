// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Shifts
{
    /// <summary>
    /// Class representing the context for creating a shift.
    /// </summary>
    public class ShiftCreationContext
    {
        /// <summary>
        /// The type of the shift.
        /// </summary>
        public ShiftType Type { get; set; }

        /// <summary>
        /// The date of the shift.
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// Creates a new instance with the given parameters
        /// </summary>
        public ShiftCreationContext(ShiftType type)
        {
            Type = type;
        }
    }
}
