// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Media.Server.Endpoint
{
    [DataContract]
    internal class VariantDescriptorModel : FileDescriptorModel
    {
        /// <summary>
        /// Internal name of the variant
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether this variant is the master or not
        /// </summary>
        [DataMember]
        public bool IsMaster { get; set; }

        /// <summary>
        /// UTC date when the descriptor was created
        /// </summary>
        [DataMember]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Preview of this variant
        /// </summary>
        [DataMember]
        public PreviewDescriptorModel Preview { get; set; }
    }
}

