// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// DTO for a service endpoint
    /// Will contain the endpoint, version and minimal client version
    /// </summary>
    [DataContract]
    public class ServiceEndpoint
    {
        /// <summary>
        /// The endpoint of the service
        /// </summary>
        [DataMember]
        public string Endpoint { get; set; }

        /// <summary>
        /// The version of this service
        /// </summary>
        [DataMember]
        public string Version { get; set; }

        /// <summary>
        /// The minimal client version of this service
        /// </summary>
        [DataMember]
        public string MinClientVersion { get; set; }
    }
}
