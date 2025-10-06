// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Communication.Endpoints
{
    /// <summary>
    /// Extensions for the module container to activate hosting
    /// </summary>
    public static class ContainerExtension
    {
        /// <summary>
        /// Activate hosting and register the necessary components
        /// </summary>
        public static IContainer ActivateHosting(this IContainer container, IEndpointHosting hosting)
        {
            hosting.ActivateHosting(container);
            return container;
        }
    }
}