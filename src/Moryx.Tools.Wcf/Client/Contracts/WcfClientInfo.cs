// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Information about clients managed by an <see cref="IWcfClientFactory"/> implementation
    /// </summary>
    public class WcfClientInfo
    {
        #region Fields

        internal const string Unknown = "Unknown";

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfClientInfo"/> class.
        /// </summary>
        internal WcfClientInfo(long id, string service, string clientVersion)
        {
            Id = id;
            Service = service;
            ClientVersion = clientVersion;
            if (!string.IsNullOrEmpty(clientVersion))
            {
                var asVersion = Version.Parse(ClientVersion);
                MinServerVersion = $"{asVersion.Major}.{asVersion.Minor}.0";
            }

            ServerVersion = Unknown;
            Uri = Unknown;

            State = ConnectionState.New;
        }

        /// <summary>
        /// Identifier of this client
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// The service the client wants to connect to.
        /// </summary>
        public string Service { get; }

        /// <summary>
        /// The URI of this service.
        /// </summary>
        public string Uri { get; internal set; }

        /// <summary>
        /// The client's version.
        /// </summary>
        public string ClientVersion { get; }

        /// <summary>
        /// The minimum version of the service the client needs to operate with.
        /// </summary>
        public string MinServerVersion { get; }

        /// <summary>
        /// The service's version as retrieved from the server.
        /// </summary>
        public string ServerVersion { get; internal set; }

        /// <summary>
        /// The number of unsuccessful retries.
        /// </summary>
        public int Tries { get; internal set; }

        /// <summary>
        /// The current state of the client.
        /// </summary>
        public ConnectionState State { get; internal set; }

        /// <summary>
        /// Creates a copy of this WcfClientInfo object.
        /// </summary>
        public WcfClientInfo Clone()
        {
            return (WcfClientInfo) MemberwiseClone();
        }
    }
}
