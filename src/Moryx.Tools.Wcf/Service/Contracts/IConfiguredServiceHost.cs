// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Communication.Endpoints;
using Moryx.Modules;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Wcf host wrapper created by the host factory
    /// </summary>
    [Obsolete("The WCF specific host interface was replaced by the general IEndpointHost in MORYX")]
    public interface IConfiguredServiceHost : IPlugin
    {
    }
}
