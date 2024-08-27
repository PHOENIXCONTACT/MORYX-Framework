// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Communication.Endpoints
{
    /// <summary>
    /// Host factory to create dependency injection enabled web services
    /// </summary>
    public interface IEndpointHosting
    {
        /// <summary>
        /// Activate hosting within a container. Makes <see cref="IEndpointHostFactory"/> available
        /// </summary>
        void ActivateHosting(IContainer container);
    }
}
