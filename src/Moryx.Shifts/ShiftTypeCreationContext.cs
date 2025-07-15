// Copyright (c) 2024, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Shifts
{
    /// <summary>
    /// Class representing the context for creating a shift type.
    /// </summary>
    public class ShiftTypeCreationContext
    {
        /// <summary>
        /// The name of the shift type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The start time of the shift type.
        /// </summary>
        public TimeOnly StartTime { get; set; }

        /// <summary>
        /// The duration of the shift type.
        /// </summary>
        public TimeOnly EndTime { get; set; }

        /// <summary>
        /// The period of the shift type in days.
        /// </summary>
        public byte Periode { get; set; }

        /// <summary>
        /// Creates a new context with the given parameters
        /// </summary>
        public ShiftTypeCreationContext(string name)
        {
            Name = name;
        }
    }
}
