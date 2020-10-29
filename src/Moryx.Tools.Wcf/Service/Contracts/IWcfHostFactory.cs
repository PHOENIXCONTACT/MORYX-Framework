// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Logging;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Host factory to create dependency injection enabled web services
    /// </summary>
    public interface IWcfHostFactory
    {
        /// <summary>
        /// Creates a new configured service host.
        /// </summary>
        /// <typeparam name="TContract">Contract type of the host.</typeparam>
        /// <param name="config">Configuration for the host.</param>
        /// <param name="hostFactory">The host factory which will be used to create a host of type T.</param>
        /// <param name="logger">The logger for the service host.</param>
        /// <returns>A new created configured service host.</returns>
        IConfiguredServiceHost CreateHost<TContract>(HostConfig config, ITypedHostFactory hostFactory, IModuleLogger logger);

        /// <summary>
        /// Creates a new configured service host.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contract">Contract type of the host.</param>
        /// <param name="config">Configuration for the host.</param>
        /// <param name="hostFactory">The host factory which will be used to create a host of type T.</param>
        /// <param name="logger">The logger for the service host.</param>
        /// <returns>A new created configured service host.</returns>
        IConfiguredServiceHost CreateHost(Type contract, HostConfig config, ITypedHostFactory hostFactory, IModuleLogger logger);
    }
}
