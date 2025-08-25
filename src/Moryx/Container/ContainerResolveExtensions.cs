// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Moryx.Container
{
    /// <summary>
    /// Extensions to replace the overloads of <see cref="IContainer.Resolve(Type, string)"/>
    /// </summary>
    public static class ContainerResolveExtensions
    {
        /// <summary>
        /// Resolve an instance of the given service
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Instance of type</returns>
        public static T Resolve<T>(this IContainer container)
        {
            return (T)container.Resolve(typeof(T), null);
        }

        /// <summary>
        /// Resolve this dependency
        /// </summary>
        public static object Resolve(this IContainer container, Type service)
        {
            return container.Resolve(service, null);
        }

        /// <summary>
        /// Resolve a named instance of the given service
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Instance of type</returns>
        public static T Resolve<T>(this IContainer container, string name)
        {
            return (T)container.Resolve(typeof(T), name);
        }

        /// <summary>
        /// Resolve all implementations of this contract
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns></returns>
        public static T[] ResolveAll<T>(this IContainer container)
        {
            return (T[])container.ResolveAll(typeof(T));
        }
    }
}
