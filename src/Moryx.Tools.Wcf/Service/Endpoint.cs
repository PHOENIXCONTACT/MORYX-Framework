// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Information about a service to be used by a client for automatic configuration.
    /// </summary>
    [DataContract]
    public class Endpoint
    {
        /// <summary>
        /// Interface of this endpoint
        /// </summary>
        [DataMember]
        public string Service { get; set; }

        /// <summary>
        /// The binding type of the service.
        /// </summary>
        [DataMember]
        public ServiceBindingType Binding { get; set; }

        /// <summary>
        /// Relative path of the service endpoint
        /// </summary>
        [DataMember]
        public string Path { get; set; }

        /// <summary>
        /// The URL of the service.
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// The service's version
        /// </summary>
        [DataMember]
        public string Version { get; set; }
        
        /// <summary>
        /// <c>True</c> if service requires authentication
        /// </summary>
        [DataMember]
        public bool RequiresAuthentication { get; set; }
    }
}
