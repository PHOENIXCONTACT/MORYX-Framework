// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Container;

/// <summary>
/// Extensions to replace the overloads of <see cref="IContainer.Resolve(Type, string)"/>
/// </summary>
public static class ContainerResolveExtensions
{
    extension(IContainer container)
    {
        /// <summary>
        /// Resolve an instance of the given service
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Instance of type</returns>
        public T Resolve<T>()
        {
            return (T)container.Resolve(typeof(T), null);
        }

        /// <summary>
        /// Resolve this dependency
        /// </summary>
        public object Resolve(Type service)
        {
            return container.Resolve(service, null);
        }

        /// <summary>
        /// Resolve a named instance of the given service
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Instance of type</returns>
        public T Resolve<T>(string name)
        {
            return (T)container.Resolve(typeof(T), name);
        }

        /// <summary>
        /// Resolve all implementations of this contract
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns></returns>
        public T[] ResolveAll<T>()
        {
            return (T[])container.ResolveAll(typeof(T));
        }
    }
}