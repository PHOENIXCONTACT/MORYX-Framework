// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Bindings;

/// <summary>
/// Extensions to double link resolver objects
/// </summary>
public static class BindingResolverExtension
{
    extension(IBindingResolverChain resolver)
    {
        /// <summary>
        /// Extend current resolver with next resolver
        /// </summary>
        /// <returns>The next resolver for fluent API</returns>
        public IBindingResolverChain Extend(IBindingResolverChain nextResolver)
        {
            resolver.NextResolver = nextResolver;
            nextResolver.PreviousResolver = resolver;
            return nextResolver;
        }

        /// <summary>
        /// Append resolver to a fully constructed chain
        /// </summary>
        /// <returns>The new end of the chain</returns>
        public IBindingResolverChain Append(IBindingResolverChain nextResolver)
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
        public IBindingResolverChain Insert(IBindingResolverChain insertedResolver)
        {
            var next = resolver.NextResolver;
            resolver.Extend(insertedResolver);
            if (next != null)
                insertedResolver.Extend(next);

            return insertedResolver;
        }

        /// <summary>
        /// Replace resolver with a different instance
        /// </summary>
        public IBindingResolverChain Replace(IBindingResolverChain replaceWith)
        {
            resolver.PreviousResolver.Extend(replaceWith);
            return replaceWith.Extend(resolver.NextResolver);
        }

        /// <summary>
        /// Remove a binding resolver and link its previous and next resolver directly
        /// </summary>
        /// <returns>The next resolver</returns>
        public IBindingResolverChain Remove()
        {
            return resolver.PreviousResolver.Extend(resolver.NextResolver);
        }
    }
}
