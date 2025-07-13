// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Runtime.Serialization;

namespace Moryx.Media
{
    /// <summary>
    /// Descriptor for an variant
    /// </summary>
    [DataContract]
    public sealed class VariantDescriptor : FileDescriptor
    {
        /// <summary>
        /// Creates a new instance of <see cref="VariantDescriptor"/>
        /// </summary>
        public VariantDescriptor()
        {
            Preview = new PreviewDescriptor();
        }

        /// <summary>
        /// Name of the variant
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Date of the creation
        /// </summary>
        [DataMember]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Preview information for this variant
        /// </summary>
        [DataMember]
        public PreviewDescriptor Preview { get; set; }
    }
}
