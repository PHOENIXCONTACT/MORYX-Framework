namespace Marvin.Bindings
{
    /// <summary>
    /// Extension to double link resolver objects
    /// </summary>
    public static class BindingResolverExtension
    {
        /// <summary>
        /// Extend current resolver with next resolver
        /// </summary>
        /// <returns>The next resolver for fluent API</returns>
        public static IBindingResolverChain Extend(this IBindingResolverChain resolver, IBindingResolverChain nextResolver)
        {
            resolver.NextResolver = nextResolver;
            nextResolver.PreviousResolver = resolver;
            return nextResolver;
        }

        /// <summary>
        /// Append resolver to a fully constructed chain
        /// </summary>
        /// <returns>The new end of the chain</returns>
        public static IBindingResolverChain Append(this IBindingResolverChain resolver, IBindingResolverChain nextResolver)
        {
            var last = resolver;
            while (last.NextResolver != null)
            {
                last = last.NextResolver;
            }
            last.Extend(nextResolver);
            return nextResolver;
        }

        /// <summary>
        /// Insert resolve into the chain
        /// </summary>
        public static IBindingResolverChain Insert(this IBindingResolverChain resolver, IBindingResolverChain insertedResolver)
        {
            var next = resolver.NextResolver;
            resolver.Extend(insertedResolver);
            return insertedResolver.Extend(next);
        }

        /// <summary>
        /// Replace resolver with a different instance
        /// </summary>
        public static IBindingResolverChain Replace(this IBindingResolverChain toReplace, IBindingResolverChain replaceWith)
        {
            toReplace.PreviousResolver.Extend(replaceWith);
            return replaceWith.Extend(toReplace.NextResolver);
        }

        /// <summary>
        /// Remove a binding resolver and link its previous and next resolver directly
        /// </summary>
        /// <returns>The next resolver</returns>
        public static IBindingResolverChain Remove(this IBindingResolverChain resolver)
        {
            return resolver.PreviousResolver.Extend(resolver.NextResolver);
        }
    }
}