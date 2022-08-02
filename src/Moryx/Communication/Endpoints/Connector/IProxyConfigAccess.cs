// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication.Endpoints
{
    /// <summary>
    /// Enables the access to the used proxy configuration
    /// </summary>
    public interface IProxyConfigAccess
    {
        /// <summary>
        /// Used proxy configuration
        /// </summary>
        IProxyConfig ProxyConfig { get; }
    }
}
