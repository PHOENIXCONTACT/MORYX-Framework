// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Workplans
{
    /// <summary>
    /// Describes which type of output an out is
    /// </summary>
    public enum OutputType
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown,
        /// <summary>
        /// Success
        /// </summary>
        Success,
        /// <summary>
        /// Failure
        /// </summary>
        Failure,
        /// <summary>
        /// Alternative failure
        /// </summary>
        Alternative,
    }

    /// <summary>
    /// Representation of each output index
    /// </summary>
    [DataContract]
    public class OutputDescription
    {
        /// <summary>
        /// Output type of this output
        /// </summary>
        [DataMember]
        public OutputType OutputType { get; set; }

        /// <summary>
        /// Name of this result
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Value used to compare a result object in order to find the right output
        /// </summary>
        [DataMember]
        public long MappingValue { get; set; }
    }
}
