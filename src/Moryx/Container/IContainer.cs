// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;

namespace Moryx.Container
{
    /// <summary>
    /// Interface for all container bootstrapper
    /// Can be castle or Unity
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Destroy the internal container and all registered objects
        /// </summary>
        void Destroy();

        #region Resolve

        /// <summary>
        /// Resolve an instance of the given service
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Instance of type</returns>
        T Resolve<T>();

        /// <summary>
        /// Resolve this dependency
        /// </summary>
        object Resolve(Type service);

        /// <summary>
        /// Resolve a named instance of the given service
        /// </summary>
        T Resolve<T>(string name);

        /// <summary>
        /// Resolve this named dependency
        /// </summary>
        object Resolve(Type service, string name);

        /// <summary>
        /// Resolve all implementations of this contract
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns></returns>
        T[] ResolveAll<T>();

        /// <summary>
        /// Resolve all implementations of this contract
        /// </summary>
        /// <param name="service">Service to resolve implementation for</param>
        /// <returns></returns>
        Array ResolveAll(Type service);
        #endregion

        /// <summary>
        /// Get all implementations for a given component interface
        /// </summary>
        /// <returns></returns>
        IEnumerable<Type> GetRegisteredImplementations(Type componentInterface);

        /// <summary>
        /// Full registration method
        /// </summary>
        void Register(Type type, Type[] services, string name, LifeCycle lifeCycle);

        /// <summary>
        /// Register a factory interface for automatic implementation
        /// </summary>
        void RegisterFactory(Type factoryInterface, string name, Type selector);

        /// <summary>
        /// Register instance for given services in the container
        /// </summary>
        void RegisterInstance(Type[] services, object instance, string name);

        #region Extensions

        /// <summary>
        /// 
        /// </summary>
        void Extend<TExtension>() where TExtension : new();

        #endregion
    }
}
