// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ServiceModel;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Host factory interface for container dependency
    /// </summary>
    public interface ITypedHostFactory
    {
        /// <summary>
        /// Create a wcf service host for a specific service
        /// </summary>
        /// <param name="contract">Service contract interface</param>
        /// <returns>Host instance for endpoint configuration</returns>
        ServiceHost CreateServiceHost(Type contract);
    }
}
