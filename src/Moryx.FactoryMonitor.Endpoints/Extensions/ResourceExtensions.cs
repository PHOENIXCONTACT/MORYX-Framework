// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Factory;

namespace Moryx.FactoryMonitor.Endpoints.Extensions
{
    internal static class ResourceExtensions
    {
        public static Resource GetFactory(this Resource resource)
        {
            var parent = resource.Parent;
            if (parent is null)
            {
                return null;
            }

            if (parent is IManufacturingFactory)
            {
                return parent;
            }
            else
            {
                return GetFactory(parent);
            }
        }
    }
}

