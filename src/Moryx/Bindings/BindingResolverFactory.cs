// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.RegularExpressions;

namespace Moryx.Bindings
{
    /// <summary>
    /// Default factory to create resolvers for binding strings
    /// </summary>
    public class BindingResolverFactory : IBindingResolverFactory
    {
        /// <summary>
        /// Regex for binding.
        /// It extracts two groups "base" and "property". The Group "property" is optional.
        /// It matches strings of type "Product.Id" or "Recipe".
        /// 
        /// To test and extend, please use this link: https://regex101.com/r/tpjBIw/4
        /// </summary>
        private readonly Regex _bindingRegex = new Regex(@"(?<base>\w+)(?:\.(?<property>\w+(?:\[\w+\])?))*");

        /// <summary>
        /// Create a property resolver by base key and forward the property info
        /// </summary>
        public IBindingResolver Create(string binding)
        {
            // Frist split binding string
            var match = _bindingRegex.Match(binding);
            var baseKey = match.Groups["base"].Value;
            var properties = match.Groups["property"].Captures;

            // First we need a BaseKeyResolver to extract the base value
            var baseResolver = CreateBaseResolver(baseKey);
            if (baseResolver == null || properties.Count == 0)
                return baseResolver;

            var resolver = baseResolver;
            for (int index = 0; index < properties.Count; index++)
            {
                resolver = AddToChain(resolver, properties[index].Value);
            }

            return baseResolver;
        }

        /// <summary>
        /// Resolver to extract base key object
        /// </summary>
        protected virtual IBindingResolverChain CreateBaseResolver(string baseKey)
        {
            return new ReflectionResolver(baseKey);
        }

        private readonly Regex _propertyRegex = new Regex(@"(?<property>\w+)(?<indexer>\[(?<index>\w+)\])?");
        /// <summary>
        /// Create resolver for a property
        /// </summary>
        protected virtual IBindingResolverChain AddToChain(IBindingResolverChain resolver, string property)
        {
            var match = _propertyRegex.Match(property);

            // Double link new resolver
            resolver = resolver.Extend(new ReflectionResolver(match.Groups["property"].Value));

            // Insert element for index resolution
            if (match.Groups["indexer"].Success)
            {
                resolver = resolver.Extend(new IndexResolver(match.Groups["index"].Value));
            }

            return resolver;
        }
    }
}
