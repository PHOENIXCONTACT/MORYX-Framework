// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Moryx.AbstractionLayer.Resources.Endpoints
{
    /// <summary>
    /// DTO that represents the reference of one resource with another
    /// </summary>
    [DataContract]
    public class ResourceReferenceModel
    {
        /// <summary>
        /// Name of the reference, usually name of the property
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Targets of this reference
        /// </summary>
        [DataMember]
        public List<ResourceModel> Targets { get; set; } = new List<ResourceModel>();
    }
}
