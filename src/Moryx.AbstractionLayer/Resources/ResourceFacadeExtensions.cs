// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Additional overloads for the resource facade APIs as well as facade version bridge
    /// </summary>
    public static class ResourceFacadeExtensions
    {
        /// <summary>
        /// Read data from a resource by the given resource-proxy
        /// </summary>
        /// <param name="facade">Extended facade</param>
        /// <param name="proxy">Resource proxy reference</param>
        /// <param name="accessor">Accessor delegate for the resource</param>
        /// <remarks>
        /// The <param name="accessor"></param> action uses the actual resource instance, not wrapped by proxy.
        /// As a result, all internal members, including properties and
        /// methods not exposed through interfaces, are accessible.
        ///
        /// This API is intended primarily for endpoint controllers that must export or
        /// inspect the full internal state of a resource.
        ///
        /// Because the returned objects are the originals, the API consumer is responsible for keeping and watching the life-cycle.
        /// Use with extreme caution. Do not keep the instance in memory for later usage.
        /// </remarks>
        public static TResult ReadUnsafe<TResult>(this IResourceManagement facade, IResource proxy, Func<Resource, TResult> accessor)
        {
            return facade.ReadUnsafe(proxy.Id, accessor);
        }

        /// <summary>
        /// Modify the resource by the given resource-proxy
        /// </summary>
        /// <param name="facade">Extended facade</param>
        /// <param name="proxy">Resource proxy reference</param>
        /// <param name="modifier">Modifier delegate, must return <value>true</value> in order to save changes</param>
        /// <remarks>
        /// The <param name="modifier"></param> action uses the actual resource instance, not wrapped by proxy.
        /// As a result, all internal members, including properties and
        /// methods not exposed through interfaces, are accessible.
        ///
        /// This API is intended primarily for endpoint controllers that must export or
        /// inspect the full internal state of a resource.
        ///
        /// Because the returned objects are the originals, the API consumer is responsible for keeping and watching the life-cycle.
        /// Use with extreme caution. Do not keep the instance in memory for later usage.
        /// </remarks>
        public static void ModifyUnsafe(this IResourceManagement facade, IResource proxy, Func<Resource, bool> modifier)
        {
            facade.ModifyUnsafe(proxy.Id, modifier);
        }

        /// <summary>
        /// Modify the resource.
        /// </summary>
        /// <param name="facade">Extended facade</param>
        /// <param name="proxy">Resource proxy reference</param>
        /// <param name="modifier">Modifier delegate, must return <value>true</value> in order to save changes</param>
        /// <param name="context">Additional context, used within the <param cref="modifier"/></param>
        /// <remarks>
        /// The <param name="modifier"></param> action uses the actual resource instance, not wrapped by proxy.
        /// As a result, all internal members, including properties and
        /// methods not exposed through interfaces, are accessible.
        ///
        /// This API is intended primarily for endpoint controllers that must export or
        /// inspect the full internal state of a resource.
        ///
        /// Because the returned objects are the originals, the API consumer is responsible for keeping and watching the life-cycle.
        /// Use with extreme caution. Do not keep the instance in memory for later usage.
        /// </remarks>
        public static void ModifyUnsafe<TContext>(this IResourceManagement facade, IResource proxy, Func<Resource, TContext, bool> modifier, TContext context)
        {
            facade.ModifyUnsafe(proxy.Id, resource => modifier(resource, context));
        }

        /// <summary>
        /// Create and initialize a resource with typed initializer
        /// </summary>
        /// <remarks>
        /// The <param name="initializer"></param> action uses the actual resource instance, not wrapped by proxy.
        /// As a result, all internal members, including properties and
        /// methods not exposed through interfaces, are accessible.
        ///
        /// This API is intended primarily for endpoint controllers that must export or
        /// inspect the full internal state of a resource.
        ///
        /// Because the returned objects are the originals, the API consumer is responsible for keeping and watching the life-cycle.
        /// Use with extreme caution. Do not keep the instance in memory for later usage.
        /// </remarks>
        /// <param name="facade">Extended facade</param>
        /// <param name="initializer">Initializer delegate for the resource</param>
        public static long CreateUnsafe<TResource>(this IResourceManagement facade, Action<TResource> initializer)
            where TResource : Resource
        {
            return facade.CreateUnsafe(typeof(TResource), r => initializer((TResource)r));
        }
    }
}
