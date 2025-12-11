// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.RegularExpressions;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Bindings;

namespace Moryx.AbstractionLayer.Processes
{
    /// <summary>
    /// Default factory that can create resolvers for <see cref="ProductType"/>, <see cref="ProductInstance"/>, <see cref="Process"/>
    /// and <see cref="IRecipe"/>. It also creates a resolver for unknown keys.
    /// </summary>
    public class ProcessBindingResolverFactory : BindingResolverFactory
    {
        /// <inheritdoc />
        protected override IBindingResolverChain CreateBaseResolver(string baseKey)
        {
            switch (baseKey)
            {
                case "Process":
                    return new NullResolver();
                case "Recipe":
                    return new DelegateResolver(source => ((Process)source).Recipe);
                case "Product":
                case "ProductType":
                    return new ProductResolver(baseKey);
                case "Article":
                case "ProductInstance":
                    return new DelegateResolver(source => (source as ProductionProcess)?.ProductInstance);
                default:
                    return null;
            }
        }

        private readonly Regex _lastActivityRegex = new(@"LastActivity(?<typed>\[(?<name>\w+)\])?");

        /// <inheritdoc />
        protected override IBindingResolverChain AddToChain(IBindingResolverChain resolver, string property)
        {
            // Special function '.LastActivity[MountActivity]'
            var lastActivity = _lastActivityRegex.Match(property);
            if (lastActivity.Success)
            {
                return lastActivity.Groups["typed"].Success
                    ? resolver.Extend(new DelegateResolver(source => (source as Process)?.LastActivity(lastActivity.Groups["name"].Value)))
                    : resolver.Extend(new DelegateResolver(source => (source as Process)?.LastActivity()));
            }

            resolver = resolver.Extend(new PartLinkShortCut());

            if (property == nameof(IIdentity.Identifier))
                return resolver.Extend(new IdentifierShortCut());

            if (property == nameof(ProductType) || property == nameof(ProductInstance.Type))
                return resolver.Extend(new ProductResolver(property));

            return base.AddToChain(resolver, property);
        }
    }

    /// <summary>
    /// Special resolver that can detect <see cref="ProductPartLink{T}"/> and skip the additional reference to
    /// <see cref="ProductPartLink{T}.Product"/>.
    /// </summary>
    public class PartLinkShortCut : BindingResolverBase
    {
        /// <inheritdoc />
        protected override object Resolve(object source)
        {
            if (!(source is ProductPartLink partLink))
            {
                // Our object is not a part link, so we leave the chain
                this.Remove();
                return source;
            }

            // Object is part link
            // 1. Try to read from object
            var linkResult = NextResolver?.Resolve(partLink);

            // 2. Try to read from product
            var prodResult = NextResolver?.Resolve(partLink.Product);

            // 3. Make sure we do not have a naming conflict
            if (linkResult != null && prodResult != null)
            {
                throw new InvalidOperationException("Binding value inconclusive on part link and product!");
            }

            return linkResult != null ? partLink : partLink.Product;
        }

        /// <inheritdoc />
        protected override bool Update(object source, object value)
        {
            throw new InvalidOperationException("PartLinks cannot be updated.");
        }
    }

    /// <summary>
    /// Uses the <see cref="IIdentifiableObject"/> interface to resolve the identifier
    /// of <see cref="IIdentity"/>
    /// </summary>
    public class IdentifierShortCut : BindingResolverBase
    {
        /// <inheritdoc />
        protected override object Resolve(object source)
        {
            return (source as IIdentifiableObject)?.Identity?.Identifier;
        }

        /// <inheritdoc />
        protected override bool Update(object source, object value)
        {
            throw new InvalidOperationException("Identifier cannot be updated.");
        }
    }

    /// <summary>
    /// Resolver to extract the product of a <see cref="ProductionProcess"/>
    /// </summary>
    public class ProductResolver : BindingResolverBase
    {
        private readonly string _fallbackProperty;

        /// <summary>
        /// Creates a new <see cref="ProductResolver"/>
        /// </summary>
        /// <param name="fallbackProperty"></param>
        public ProductResolver(string fallbackProperty)
        {
            _fallbackProperty = fallbackProperty;
        }

        /// <inheritdoc />
        protected sealed override object Resolve(object source)
        {
            switch (source)
            {
                case Process process:
                {
                    var product = (process.Recipe as IProductRecipe)?.Product;
                    return product;
                }
                case Activity activity:
                {
                    var product = (activity.Process.Recipe as IProductRecipe)?.Product;
                    return product;
                }
                case ProductInstance instance:
                    return instance.Type;
            }

            // If our shortcuts do not work, use ReflectionResolver instead
            // Due the protection level of Resolve in the base class and the implementation
            // of IBindingResolver.Resolve which calls the whole chain the call order is important here.
            // First the value of the replacement is called and then the replacement is placed into the chain.
            var replacement = new ReflectionResolver(_fallbackProperty);
            var resolvedValue = ((IBindingResolverChain)replacement).Resolve(source);
            this.Replace(replacement);
            return resolvedValue;
        }

        /// <inheritdoc />
        protected override bool Update(object source, object value)
        {
            throw new InvalidOperationException("Products cannot be updated!");
        }
    }
}
