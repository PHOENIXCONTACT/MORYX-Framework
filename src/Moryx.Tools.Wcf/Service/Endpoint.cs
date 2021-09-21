// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Information about a service to be used by a client for automatic configuration.
    /// </summary>
    [DataContract]
    public class Endpoint : Communication.Endpoints.Endpoint
    {
        /// <summary>
        /// The binding type of the service.
        /// </summary>
        [DataMember]
        public ServiceBindingType Binding { get; set; }
    }
}
