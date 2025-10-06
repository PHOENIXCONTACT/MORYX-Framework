// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Media.Server.Endpoint
{
    /// <summary>
    /// Result information of an add request
    /// </summary>
    [DataContract]
    internal class ContentAddingInfoModel
    {
        /// <summary>
        /// Created or existent descriptor
        /// </summary>
        [DataMember]
        public ContentDescriptorModel ContentDescriptor { get; set; }

        /// <summary>
        /// Created variant
        /// </summary>
        [DataMember]
        public VariantDescriptorModel VariantDescriptor { get; set; }

        /// <summary>
        /// Concretely result of the add request
        /// </summary>
        [DataMember]
        public ContentAddingResult Result { get; set; }
    }
}

