// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Shifts.Endpoints.Models
{
    /// <summary>
    /// Class representing the context for creating a shift assignment.
    /// </summary>
    [DataContract]
    public class ShiftAssignementCreationContextModel
    {
        /// <summary>
        /// The resource of the shift assignment.
        /// </summary>
        [DataMember]
        public long ResourceId { get; set; }

        /// <summary>
        /// The shift of the shift assignment.
        /// </summary>
        [DataMember]
        public long ShiftId { get; set; }

        /// <summary>
        /// The operator of the shift assignment.
        /// </summary>
        [DataMember]
        public string? OperatorIdentifier { get; set; }

        /// <summary>
        /// The note of the shift assignment.
        /// </summary>
        [DataMember]
        public string? Note { get; set; }

        /// <summary>
        /// The priority of the shift assignment.
        /// </summary>
        [DataMember]
        public int Priority { get; set; }

        /// <summary>
        /// The assigned days of the shift assignment.
        /// </summary>
        [DataMember]
        public AssignedDays AssignedDays { get; set; }
    }
}

