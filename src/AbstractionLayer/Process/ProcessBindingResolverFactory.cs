using System;
using System.Text.RegularExpressions;
using Marvin.AbstractionLayer.Identity;
using Marvin.Bindings;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Default factory that can create resolvers for <see cref="IProductType"/>, <see cref="ProductInstance"/>, <see cref="Process"/>
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
                    return new DelegateResolver(source => ((IProcess)source).Recipe);
                case "Product":
                case "ProductType":
                    return new ProductResolver();
                case "Article":
                case "ProductInstance":
                    return new DelegateResolver(source => (source as ProductionProcess)?.ProductInstance);
                default:
                    return null;
            }
        }

        private readonly Regex _lastActivityRegex = new Regex(@"LastActivity(?<typed>\[(?<name>\w+)\])?");

        /// <inheritdoc />
        protected override IBindingResolverChain AddToChain(IBindingResolverChain resolver, string property)
        {
            // Special function '.LastActivity[MountActivity]'
            var lastActivity = _lastActivityRegex.Match(property);
            if (lastActivity.Success)
            {
                return lastActivity.Groups["typed"].Success
                    ? resolver.Extend(new DelegateResolver(source => (source as IProcess)?.LastActivity(lastActivity.Groups["name"].Value)))
                    : resolver.Extend(new DelegateResolver(source => (source as IProcess)?.LastActivity()));
            }

            resolver = resolver.Extend(new PartLinkShortCut());

            if (property == nameof(IIdentity.Identifier))
                return resolver.Extend(new IdentifierResolver());

            if (property == nameof(ProductType))
                return resolver.Extend(new ProductResolver());

            return base.AddToChain(resolver, property);
        }

        /// <summary>
        /// Special resolver that can detect <see cref="IProductPartLink{T}"/> and skip the additional reference to
        /// <see cref="IProductPartLink{T}.Product"/>.
        /// </summary>
        private class PartLinkShortCut : BindingResolverBase
        {
            protected override object Resolve(object source)
            {
                var partLink = source as IProductPartLink;
                if (partLink == null)
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

                return linkResult != null ? (object)partLink : partLink.Product;
            }

            protected override bool Update(object source, object value)
            {
                throw new InvalidOperationException("PartLinks cannot be updated.");
            }
        }

        /// <summary>
        /// Uses the <see cref="IIdentifiableObject"/> interface to resolve the identifier
        /// of <see cref="IIdentity"/>
        /// </summary>
        private class IdentifierResolver : BindingResolverBase
        {
            protected override object Resolve(object source)
            {
                return (source as IIdentifiableObject)?.Identity.Identifier;
            }

            protected override bool Update(object source, object value)
            {
                throw new InvalidOperationException("Identifier cannot be updated.");
            }
        }

        /// <summary>
        /// Resolver to extract the product of a <see cref="ProductionProcess"/>
        /// </summary>
        private class ProductResolver : BindingResolverBase
        {
            protected sealed override object Resolve(object source)
            {
                var process = source as IProcess;
                if (process != null)
                {
                    var product = (process.Recipe as IProductRecipe)?.Product;
                    return product;
                }

                var article = source as ProductInstance;
                if (article != null)
                {
                    return article.ProductType;
                }

                // If our shortcuts do not work, use ReflectionResolver instead
                // Due the protection level of Resolve in the base class and the implementation
                // of IBindingResolver.Resolve which calls the whole chain the call order is important here.
                // First the value of the replacement is called and then the replacement is placed into the chain.
                var replacement = new ReflectionResolver(nameof(ProductInstance.ProductType));
                var resolvedValue = ((IBindingResolverChain) replacement).Resolve(source);
                this.Replace(replacement);
                return resolvedValue;
            }

            protected override bool Update(object source, object value)
            {
                throw new InvalidOperationException("Products cannot be updated!");
            }
        }
    }
}