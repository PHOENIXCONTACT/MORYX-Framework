// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Media.Server.Endpoint
{
    /// <summary>
    /// Wcf Model of <see cref="ContentDescriptor"/>
    /// </summary>
    [DataContract]
    internal class ContentDescriptorModel
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
        /// Wcf variant models of the descriptor
        /// </summary>
        [DataMember]
        public List<VariantDescriptorModel> Variants { get; set; }
    }
}

