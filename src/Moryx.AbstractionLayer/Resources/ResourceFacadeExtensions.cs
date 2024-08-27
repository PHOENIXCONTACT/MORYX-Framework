// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Additional overloads for the resource facade APIs as well as facade version bridge
    /// </summary>
    public static class ResourceFacadeExtensions
    {
        /// <summary>
        /// Read data from a resource
        /// </summary>
        public static TResult Read<TResult>(this IResourceManagement facade, long resourceId, Func<Resource, TResult> accessor)
        {
            return facade.Read(resourceId, accessor);
        }

        /// <summary>
        /// Read data from a resource
        /// </summary>
        public static TResult Read<TResult>(this IResourceManagement facade, IResource proxy, Func<Resource, TResult> accessor)
        {
            return facade.Read(proxy.Id, accessor);
        }

        /// <summary>
        /// Modify the resource. 
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="modifier">Modifier delegate, must return <value>true</value> in order to save changes</param>
        /// <param name="facade"></param>
        public static void Modify(this IResourceManagement facade, long resourceId, Func<Resource, bool> modifier)
        {
            facade.Modify(resourceId, modifier);
        }

        /// <summary>
        /// Modify the resource. 
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="modifier">Modifier delegate, must return <value>true</value> in order to save changes</param>
        /// <param name="facade"></param>
        public static void Modify(this IResourceManagement facade, IResource proxy, Func<Resource, bool> modifier)
        {
            facade.Modify(proxy.Id, modifier);
        }

        /// <summary>
        /// Modify the resource. 
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="modifier">Modifier delegate, must return <value>true</value> in order to save changes</param>
        /// <param name="facade"></param>
        /// <param name="context"></param>
        public static void Modify<TContext>(this IResourceManagement facade, IResource proxy, Func<Resource, TContext, bool> modifier, TContext context)
        {
            facade.Modify(proxy.Id, resource => modifier(resource, context));
        }

        /// <summary>
        /// Create a resource with typed initializer 
        /// </summary>
        /// <param name="facade"></param>
        /// <param name="initializer"></param>
        public static long Create<TResource>(this IResourceManagement facade, Action<TResource> initializer)
            where TResource : Resource
        {
            return facade.Create(typeof(TResource), r => initializer((TResource)r));
        }
    }
}