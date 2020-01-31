// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Information about a service to be used by a client for automatic configuration.
    /// </summary>
    [DataContract]
    public class ServiceConfig
    {
        /// <summary>
        /// The binding type of the service.
        /// </summary>
        [DataMember]
        public ServiceBindingType Binding { get; set; }

        /// <summary>
        /// The URL of the service.
        /// </summary>
        [DataMember]
        public string ServiceUrl { get; set; }

        /// <summary>
        /// The service's version
        /// </summary>
        [DataMember]
        public string ServerVersion { get; set; }

        /// <summary>
        /// The minimum supported client version.
        /// </summary>
        [DataMember]
        public string MinClientVersion { get; set; }

        /// <summary>
        /// <c>True</c> if service requires authentication
        /// </summary>
        [DataMember]
        public bool RequiresAuthentication { get; set; }
    }
}
