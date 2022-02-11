// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
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
            if (facade is IResourceModification modification)
                return modification.Read(resourceId, accessor);

            throw new NotSupportedException("Instance of resource management does not support resource modification");
        }

        /// <summary>
        /// Read data from a resource
        /// </summary>
        public static TResult Read<TResult>(this IResourceManagement facade, IResource proxy, Func<Resource, TResult> accessor)
        {
            if(facade is IResourceModification modification)
                return modification.Read(proxy.Id, accessor);

            throw new NotSupportedException("Instance of resource management does not support resource modification");
        }

        /// <summary>
        /// Modify the resource. 
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="modifier">Modifier delegate, must return <value>true</value> in order to save changes</param>
        /// <param name="facade"></param>
        public static void Modify(this IResourceManagement facade, long resourceId, Func<Resource, bool> modifier)
        {
            if (facade is IResourceModification modification)
                modification.Modify(resourceId, modifier);
            else
                throw new NotSupportedException("Instance of resource management does not support resource modification");
        }

        /// <summary>
        /// Modify the resource. 
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="modifier">Modifier delegate, must return <value>true</value> in order to save changes</param>
        /// <param name="facade"></param>
        public static void Modify(this IResourceManagement facade, IResource proxy, Func<Resource, bool> modifier)
        {
            if (facade is IResourceModification modification)
                modification.Modify(proxy.Id, modifier);
            else
                throw new NotSupportedException("Instance of resource management does not support resource modification");
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
            if (facade is IResourceModification modification)
                modification.Modify(proxy.Id, resource => modifier(resource, context));
            else
                throw new NotSupportedException("Instance of resource management does not support resource modification");
        }
    }
}