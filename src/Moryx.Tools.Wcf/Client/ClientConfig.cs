// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Config for the client
    /// </summary>
    [DataContract]
    public class ClientVersionConfig : IClientVersionConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientVersionConfig"/> class.
        /// </summary>
        public ClientVersionConfig()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientVersionConfig"/> class.
        /// </summary>
        public ClientVersionConfig(string clientVersion, string minServerVersion)
        {
            ClientVersion = clientVersion;
            MinServerVersion = minServerVersion;
        }

        ///
        [DataMember]
        public string ClientVersion { get; set; }

        ///
        [DataMember]
        public string MinServerVersion { get; set; }
    }

    /// <summary>
    /// Configuration of a WCF client.
    /// </summary>
    [DataContract]
    public class ClientConfig : ClientVersionConfig, IClientConfig
    {
        ///
        [DataMember]
        public BindingType BindingType { get; set; }

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
