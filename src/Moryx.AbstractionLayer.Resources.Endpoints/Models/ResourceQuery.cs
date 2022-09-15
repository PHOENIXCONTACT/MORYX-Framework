// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.AbstractionLayer.Resources.Endpoints
{
    /// <summary>
    /// Filter object to fetch resources from the server
    /// </summary>
    [DataContract]
    public class ResourceQuery
    {
        /// <summary>
        /// Type constraints to filter instances
        /// </summary>
        [DataMember]
        public string[] Types { get; set; }

        /// <summary>
        /// Reference condition to filter instances
        /// </summary>
        [DataMember]
        public ReferenceFilter ReferenceCondition { get; set; }

        /// <summary>
        /// Flag if included references should be processed recursively
        /// </summary>
        [DataMember]
        public bool ReferenceRecursion { get; set; }

        /// <summary>
        /// References that should be included in the response
        /// </summary>
        [DataMember]
        public ReferenceFilter[] IncludedReferences { get; set; }
    }
}
