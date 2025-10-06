// Copyright (c) 2024, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Operators;

namespace Moryx.Shifts
{
    /// <summary>
    /// Class representing the context for creating a shift assignment.
    /// </summary>
    public class ShiftAssignementCreationContext
    {
        /// <summary>
        /// The resource of the shift assignment.
        /// </summary>
        public IResource Resource { get; set; }

        /// <summary>
        /// The shift of the shift assignment.
        /// </summary>
        public Shift Shift { get; set; }

        /// <summary>
        /// The operator of the shift assignment.
        /// </summary>
        public Operator Operator { get; set; }

        /// <summary>
        /// The note of the shift assignment.
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// The priority of the shift assignment.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// The assigned days of the shift assignment.
        /// </summary>
        public AssignedDays AssignedDays { get; set; }

        /// <summary>
        /// Creates a new instance with the given parameters
        /// </summary>
        /// <param name="operator"></param>
        public ShiftAssignementCreationContext(Shift shift, IResource resource, Operator @operator)
        {
            Shift = shift;
            Resource = resource;
            Operator = @operator;
        }
    }
}
