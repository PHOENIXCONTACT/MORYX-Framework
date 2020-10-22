// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Communication;

namespace Moryx.Tools.Wcf
{
    internal interface IVersionServiceManager : IDisposable
    {
        /// <summary>
        /// Checks if the service manager is already initialized
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Will initialize the service manager
        /// </summary>
        void Initialize(IProxyConfig proxyConfig, string host, int port);

        /// <summary>
        /// Available endpoints for this service type
        /// </summary>
        Endpoint[] ActiveEndpoints();

        /// <summary>
        /// Available endpoints for this service type
        /// </summary>
        Endpoint[] ServiceEndpoints(string service);
    }
}
