// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Bindings;

namespace Moryx.AbstractionLayer.Activities
{
    /// <summary>
    /// Default factory that can create resolvers based on activities for
    /// <see cref="IActivity"/>, <see cref="ProductType"/>, <see cref="ProductInstance"/>, <see cref="Process"/> and <see cref="IRecipe"/>.
    /// It also creates a resolver for unknown keys.
    /// </summary>
    public class ActivityBindingResolverFactory : ProcessBindingResolverFactory
    {
        /// <inheritdoc />
        protected override IBindingResolverChain CreateBaseResolver(string baseKey)
        {
            switch (baseKey)
            {
                case "Activity":
                    return new NullResolver();
                case "Tracing":
                    return new DelegateResolver(source => ((IActivity)source).Tracing);
                case "Process":
                    return new DelegateResolver(source => ((IActivity)source).Process);
                case "Recipe":
                    return new DelegateResolver(source => ((IActivity)source).Process.Recipe);
                case "Product":
                case "ProductType":
                    return new ProductResolver(baseKey);
                case "Article":
                case "ProductInstance":
                    return new DelegateResolver(source => (((IActivity)source).Process as ProductionProcess)?.ProductInstance);
                default:
                    return null;
            }
        }
    }
}
