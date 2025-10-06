// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Shifts.Endpoints
{
    /// <summary>
    /// Class representing a shift.
    /// </summary>
    [DataContract]
    public class ShiftModel
    {
        /// <summary>
        /// The ID of the shift.
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// The type of the shift.
        /// </summary>
        [DataMember]
        public long TypeId { get; set; }

        /// <summary>
        /// The date of the shift.
        /// </summary>
        [DataMember]
        //[DataType(DataType.Date)]
        public DateOnly Date { get; set; }
    }
}
