// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Moryx.Shifts.Endpoints
{
    /// <summary>
    /// Class representing a shift type.
    /// </summary>
    [DataContract]
    public class ShiftTypeModel
    {
        /// <summary>
        /// The ID of the shift type.
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// The name of the shift type.
        /// </summary>
        [DataMember]
        public string? Name { get; set; }

        /// <summary>
        /// The start time of the shift type.
        /// </summary>
        [DataMember]
        //[DataType(DataType.Time)]
        public TimeOnly StartTime { get; set; }

        /// <summary>
        /// The end time of the shift type.
        /// </summary>
        [DataMember]
        //[DataType(DataType.Time)]
        public TimeOnly Endtime { get; set; }

        /// <summary>
        /// The period of the shift type.
        /// </summary>
        [DataMember]
        public byte Periode { get; set; }
    }
}
