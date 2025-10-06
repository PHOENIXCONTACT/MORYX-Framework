// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;
using System.Runtime.Serialization;

namespace Moryx.ControlSystem.VisualInstructions.Endpoints
{
    /// <summary>
    /// Model class for an visual instruction
    /// </summary>
    [DataContract]
    public class InstructionModel
    {
        public InstructionModel()
        {
            PossibleResults = Array.Empty<string>();
            Results = Array.Empty<InstructionResultModel>();
        }

        /// <summary>
        /// Runtime unique identifier of this instruction
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// Name of the sender
        /// </summary>
        [DataMember]
        public string Sender { get; set; }

        /// <summary>
        /// Type of the instruction, Display or Execute
        /// </summary>
        [DataMember]
        public InstructionType Type { get; set; }

        /// <summary>
        /// Items of the instruction
        /// </summary>
        [DataMember]
        public InstructionItemModel[] Items { get; set; }

        /// <summary>
        /// Optional inputs by the user
        /// </summary>
        [DataMember]
        public Entry Inputs { get; set; }

        /// <summary>
        /// Results of the instruction
        /// </summary>
        [DataMember]
        [Obsolete("Use the result objects in 'Results' property instead!")]
        public string[] PossibleResults { get; set; }

        /// <summary>
        /// Possible results with key and display values
        /// </summary>
        [DataMember]
        public InstructionResultModel[] Results { get; set; }
    }
}

