// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Wcf
{
    internal interface IEndpointCollector
    {
        Endpoint[] AllEndpoints { get; }

        void AddEndpoint(string address, Endpoint endpoint);

        void RemoveEndpoint(string address);
    }
}