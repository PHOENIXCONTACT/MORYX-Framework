// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Wcf
{
    internal interface IEndpointCollector
    {
        void AddEndpoint(string endpoint, ServiceVersionAttribute endpointVersion);

        void AddService(Type service, ServiceBindingType binding, string serviceUrl, ServiceVersionAttribute version,
            bool requiresAuthentication);

        void RemoveEndpoint(string endpoint);

        void RemoveService(Type service);

        bool ClientSupported(string service, string clientVersion);

        string GetServerVersion(string endpoint);

        ServiceConfig GetServiceConfiguration(string service);

        ServiceEndpoint[] AllEndpoints();
    }
}