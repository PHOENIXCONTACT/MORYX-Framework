// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication
{
    /// <summary>
    /// Proxy configuration to be used by WCF clients with BasicHTTP binding.
    /// </summary>
    public interface IProxyConfig
    {
        /// <summary>
        /// <c>True</c>, if a proxy shall be used or <c>false otherwise.</c>
        /// </summary>
        bool EnableProxy { get; }

        /// <summary>
        /// <c>True</c>, if the default proxy configuration of the machine shall be used.
        /// The <see cref="Address"/> and <see cref="Port"/> properties are ignored then.
        /// </summary>
        bool UseDefaultWebProxy { get; }

        /// <summary>
        /// The IP address or hostname of the proxy to be used.
        /// This property is ignored if <see cref="UseDefaultWebProxy"/> is <c>true</c>.
        /// </summary>
        string Address { get; }

        /// <summary>
        /// The TCP port  of the proxy to be used.
        /// This property is ignored if <see cref="UseDefaultWebProxy"/> is <c>true</c>.
        /// </summary>
        int Port { get; }
    }
}
