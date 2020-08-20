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
        /// Checks the client support for the given version
        /// </summary>
        bool CheckClientSupport(string service, string clientVersion);

        /// <summary>
        /// Will return the server version vor a given endpoint
        /// </summary>
        string GetServerVersion(string endpoint);

        /// <summary>
        /// Will return the complete configuration for the service
        /// </summary>
        ServiceConfiguration GetServiceConfiguration(string service);

        /// <summary>
        /// Check if clients and server versions match
        /// </summary>
        /// <returns>True if two sided version check is sucessful</returns>
        bool Match(IClientConfig config, string version);

        /// <summary>
        /// Check if clients and server versions match
        /// </summary>
        /// <returns>True if two sided version check is sucessful</returns>
        bool Match(WcfClientInfo clientInfo, ServiceConfiguration sericeConfiguration);
    }
}
