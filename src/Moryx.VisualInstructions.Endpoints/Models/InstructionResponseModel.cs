// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Serialization;

namespace Moryx.VisualInstructions.Endpoints
{
    /// <summary>
    /// Response from the client
    /// </summary>
    [DataContract(Name = "InstructionResponse")]
    public class InstructionResponseModel
    {
        /// <summary>
        /// Instruction identifier
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// Inputs by the user
        /// </summary>
        [DataMember]
        public Entry Inputs { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        [DataMember]
        public string Result { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        [DataMember]
        public InstructionResultModel SelectedResult { get; set; }
    }
}
