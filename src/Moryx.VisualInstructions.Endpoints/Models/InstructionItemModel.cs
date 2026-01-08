// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.VisualInstructions;

namespace Moryx.VisualInstructions.Endpoints
{
    /// <summary>
    /// An instruction can have multiple items
    /// </summary>
    [DataContract]
    public class InstructionItemModel
    {
        /// <summary>
        /// Type of the <see cref="Content"/> property
        /// </summary>
        [DataMember]
        public InstructionContentType ContentType { get; set; }

        /// <summary>
        /// Content of the instruction item
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// Content of the preview
        /// </summary>
        [DataMember]
        public string Preview { get; set; }
    }
}
