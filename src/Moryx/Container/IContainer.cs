// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
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

        #region LoadComponents

        /// <summary>
        /// Load all implementations of type from currently known types
        /// KnownTypes: Types in default framework folders and deeper.
        /// </summary>
        void LoadComponents<T>()
            where T : class;

        /// <summary>
        /// Load all implementations of type from currently known types
        /// KnownTypes: Types in default framework folders and deeper.
        /// </summary>
        void LoadComponents<T>(Predicate<Type> condition)
            where T : class;

        #endregion

        #region Register

        /// <summary>
        /// Execute the installer
        /// </summary>
        /// <param name="installer"></param>
        IContainer ExecuteInstaller(IContainerInstaller installer);

        /// <summary>
        /// Register external type in local container
        /// </summary>
        IContainer Register<TService, TComp>()
            where TComp : TService
            where TService : class;

        /// <summary>
        /// Register external type in local container
        /// </summary>
        IContainer Register<TService, TComp>(string name, LifeCycle lifeCycle)
            where TComp : TService
            where TService : class;

        /// <summary>
        /// Register factory interface
        /// </summary>
        /// <typeparam name="TFactory"></typeparam>
        IContainer Register<TFactory>()
            where TFactory : class;

        /// <summary>
        /// Register factory interface
        /// </summary>
        /// <typeparam name="TFactory"></typeparam>
        IContainer Register<TFactory>(string name)
            where TFactory : class;

        #endregion

        #region Set instance

        /// <summary>
        /// Set instance of service
        /// </summary>
        /// <typeparam name="T">Type of service</typeparam>
        /// <param name="instance">Instance implementing the service</param>
        IContainer SetInstance<T>(T instance) where T : class;

        /// <summary>
        /// Set globally imported instance with name
        /// </summary>
        /// <typeparam name="T">Type of service</typeparam>
        /// <param name="instance">Instance to register</param>
        /// <param name="name">Name of instance</param>
        IContainer SetInstance<T>(T instance, string name) where T : class;

        #endregion

        #region Extensions

        void Extend<TExtension>() where TExtension : new();

        #endregion
    }
}
