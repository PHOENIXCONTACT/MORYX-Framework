// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Configuration of a WCF client.
    /// </summary>
    [DataContract]
    public class ClientConfig
    {
        ///
        [DataMember]
        public string ClientVersion { get; set; }

        ///
        [DataMember]
        public bool CheckVersion { get; set; }

        ///
        [DataMember]
        public ServiceBindingType BindingType { get; set; }

        ///
        [DataMember]
        public string Endpoint { get; set; }

        ///
        [DataMember]
        public string Host { get; set; }

        ///
        [DataMember]
        public int Port { get; set; }

        ///
        [DataMember]
        public bool RequiresAuthentification { get; set; }

        ///
        [DataMember]
        public string UserName { get; set; }

        ///
        [DataMember]
        public string Password { get; set; }
    }
}
