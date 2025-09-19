using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Moryx.Bindings
{
    /// <summary>
    /// Create resolve for a specific instruction
    /// </summary>
    public static class TextBindingResolverFactory
    {
        /// <summary>
        /// Regex for binding.
        /// It matches strings of type "Product.Id" or "Recipe".
        /// </summary>
        private const string BindingRegex = @"\{(?<binding>([A-Za-z_][A-Za-z0-9_]*)(\[\d+\])?(\.([A-Za-z_][A-Za-z0-9_]*)(\[\d+\])?)*)?(?<hasFormat>:(?<format>\w+))?\}";

        /// <summary>
        /// Create resolver for this instruction
        /// </summary>
        public static ITextBindingResolver Create(string parameterWithBindings, IBindingResolverFactory resolverFactory)
        {
            //null check to make sure the parameterWithBinding is not null
            if (parameterWithBindings == null)
                return new NullResolver(parameterWithBindings);

            // Parse bindings
            var matches = Regex.Matches(parameterWithBindings, BindingRegex);

            // No bindings, no resolver
            if (matches.Count == 0)
                return new NullResolver(parameterWithBindings);

            // Otherwise use the awesome regex resolver
            return BuildRegexResolver(matches, parameterWithBindings, resolverFactory);
        }

        /// <summary>
        /// For images or text without parameters we return the original object
        /// </summary>
        private class NullResolver : ITextBindingResolver
        {
            private readonly string _source;

            public NullResolver(string source)
            {
                _source = source;
            }

            /// <summary>
            /// Instructions does not contain any bindings and can be returned directly
            /// </summary>
            public string Resolve(object source)
            {
                return _source;
            }
        }

        /// <summary>
        /// Build a regex resolver
        /// </summary>
        private static RegexResolver BuildRegexResolver(MatchCollection matches, string input, IBindingResolverFactory resolverFactory)
        {
            var propertyResolvers = new Dictionary<string, IBindingResolver>();
            for (int index = 0; index < matches.Count; index++)
            {
                var match = matches[index];
                var binding = match.Groups["binding"].Value;

                var resolver = (IBindingResolverChain)resolverFactory.Create(binding);
                if (resolver == null) // Could not be resolved, just keep for later
                    continue;

                if (match.Groups["hasFormat"].Success)
                {
                    // Add additional formatter to the end of the chain
                    var format = match.Groups["format"].Value;
                    resolver.Append(new FormatBindingResolver(format));
                }
                propertyResolvers[match.Value] = resolver;
            }

            return new RegexResolver(input, propertyResolvers);
        }

        /// <summary>
        /// Resolver that uses the regex matches to resolve bindings in the instruction
        /// </summary>
        private class RegexResolver : ITextBindingResolver
        {
            private readonly string _source;
            private readonly IDictionary<string, IBindingResolver> _resolvers;

            public RegexResolver(string source, IDictionary<string, IBindingResolver> resolvers)
            {
                _source = source;
                _resolvers = resolvers;
            }

            /// <summary>
            /// Use the <see cref="IBindingResolver"/>s that were extracted using the 
            /// binding regex.
            /// </summary>
            public string Resolve(object source)
            {
                var result = _source;
                foreach (var resolver in _resolvers)
                {
                    var resolvedReference = resolver.Value.Resolve(source);
                    result = resolvedReference is null ? 
                             result : result.Replace(resolver.Key, resolvedReference.ToString());
                }
                return result;
            }
        }
    }
}
