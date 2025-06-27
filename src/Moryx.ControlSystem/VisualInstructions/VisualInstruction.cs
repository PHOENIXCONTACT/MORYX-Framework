// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Serialization;

namespace Moryx.ControlSystem.VisualInstructions
{
    /// <summary>
    /// Type of instruction
    /// </summary>
    public enum InstructionContentType
    {
        /// <summary>
        /// Default value. Should only be used in clients
        /// </summary>
        Unknown,
        /// <summary>
        /// Simple text instruction
        /// </summary>
        Text,
        /// <summary>
        /// Media instruction
        /// </summary>
        Media
    }

    /// <summary>
    /// Interface of a single instruction
    /// </summary>
    [DataContract]
    public class VisualInstruction
    {
        /// <summary>
        /// Type of instruction
        /// </summary>
        [EntrySerialize, DataMember]
        public InstructionContentType Type { get; set; }

        /// <summary>
        /// Content of the instruction
        /// </summary>
        [EntrySerialize, DataMember]
        public string Content { get; set; }

        /// <summary>
        /// Smaller/shorter preview for the instruction
        /// </summary>
        [DataMember, EntrySerialize]
        public string Preview { get; set; }
    }

    /// <summary>
    /// Interface for classes that define instructions
    /// </summary>
    public interface IVisualInstructions
    {
        /// <summary>
        /// All instructions for this activity
        /// </summary>
        VisualInstruction[] Instructions { get; }
    }
}
