// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Media.Endpoints.Models
{
    public class ContentDescriptorModel
    {
        /// <summary>
        /// Unique Id of the descriptor
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the original file
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Variant models of the descriptor
        /// </summary>
        [DataMember]
        public VariantDescriptor[] Variants { get; set; }

        /// <summary>
        /// Master variant
        /// </summary>
        [DataMember]
        public VariantDescriptor Master { get; set; }
    }
}

