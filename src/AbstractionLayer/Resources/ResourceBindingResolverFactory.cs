// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Bindings;

namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Default factory that can create resolvers for IResource.
    /// </summary>
    public class ResourceBindingResolverFactory : BindingResolverFactory
    {
        /// <inheritdoc />
        protected override IBindingResolverChain CreateBaseResolver(string baseKey)
        {
            switch (baseKey)
            {
                case "Resource":
                    return new NullResolver();
                default:
                    return null;
            }
        }
    }
}
