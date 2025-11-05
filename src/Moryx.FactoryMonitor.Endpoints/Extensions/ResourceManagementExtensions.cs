// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Factory;
using Moryx.FactoryMonitor.Endpoints.Exceptions;
using Moryx.FactoryMonitor.Endpoints.Properties;

namespace Moryx.FactoryMonitor.Endpoints.Extensions
{
    internal static class ResourceManagementExtensions
    {
        public static IManufacturingFactory GetRootFactory(this IResourceManagement resourceManagement)
        {
            var rootFactory = resourceManagement.GetResource<IManufacturingFactory>(x => (x as ManufacturingFactory).Parent is null);

            return rootFactory is null ? throw new NoRootFactoryException(Strings.ResourceManagementExtensions_NoRootFactoryException) : rootFactory;
        }
    }
}

