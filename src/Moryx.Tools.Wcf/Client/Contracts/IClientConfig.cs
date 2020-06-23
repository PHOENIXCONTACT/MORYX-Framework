// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Configuration of a WCF client.
    /// </summary>
    public interface IClientVersionConfig
    {
        /// <summary>
        /// The version of the client a dotted quad (eg. 1.0.0.0)
        /// </summary>
        string ClientVersion { get; }

        /// <summary>
        /// The minimum version of the service this client can work with as a dotted quad (eg. 1.0.0.0)
        /// </summary>
        string MinServerVersion { get; }
    }

    /// <summary>
    /// Full configuration for an wcf service client. 
    /// </summary>
    public interface IClientConfig : IClientVersionConfig
    {
        /// <summary>
        /// The WCF binding type
        /// </summary>
        BindingType BindingType { get; }

        /// <summary>
        /// The end point to connect to
        /// </summary>
        string Endpoint { get; }

        /// <summary>
        /// The host to connect to. This property may be left empty if the global WCF configuration shall be used instead.
        /// </summary>
        string Host { get; }

        /// <summary>
        /// The TCP port to connect to.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// <c>True</c> if authentification is needed.
        /// </summary>
        bool RequiresAuthentification { get; }

        /// <summary>
        /// Username for bindings with credential type "Basic"
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// Password for bindings with credential type "Basic"
        /// </summary>
        string Password { get; }
    }
}
