// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Communication.Endpoints
{
    /// <summary>
    /// Host factory to create dependency injection enabled web services
    /// </summary>
    public interface IEndpointHostFactory
    {
        /// <summary>
        /// Create host instance for a service
        /// </summary>
        IEndpointHost CreateHost(Type endpoint, object config);
    }
}
