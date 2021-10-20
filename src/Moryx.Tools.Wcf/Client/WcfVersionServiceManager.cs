// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Communication;
using Moryx.Communication.Endpoints;

namespace Moryx.Tools.Wcf
{
    internal class WcfVersionServiceManager : VersionServiceManager<Endpoint>
    {
        public WcfVersionServiceManager(IProxyConfig proxyConfig, string host, int port) : base(proxyConfig, host, port)
        {
        }
    }
}
