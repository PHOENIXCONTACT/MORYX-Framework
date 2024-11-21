// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication.Endpoints
{
    internal class WebVersionServiceManager : VersionServiceManager<Endpoint>
    {
        public WebVersionServiceManager(IProxyConfig proxyConfig, string host, int port) : base(proxyConfig, host, port)
        {
        }
    }
}