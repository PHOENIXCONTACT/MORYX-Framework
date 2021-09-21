// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;

namespace Moryx.Communication.Endpoints
{
    /// <summary>
    /// Host factory to create dependency injection enabled web services
    /// </summary>
    public interface IEndpointHosting
    {
        /// <summary>
        /// Activate hosting within a container. Makes <see cref="IEndpointHostFactory"/> availabe
        /// </summary>
        void ActivateHosting(IContainer container);
    }
}
