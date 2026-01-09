// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Container;

/// <summary>
/// Interface for all container bootstrapper
/// Can be castle or Unity
/// </summary>
public interface IContainer
{
    /// <summary>
    /// Resolve this named dependency
    /// </summary>
    object Resolve(Type service, string name);

    /// <summary>
    /// Resolve all implementations of this contract
    /// </summary>
    /// <param name="service">Service to resolve implementation for</param>
    /// <returns></returns>
    Array ResolveAll(Type service);

    /// <summary>
    /// Get all implementations for a given component interface
    /// </summary>
    /// <returns></returns>
    IEnumerable<Type> GetRegisteredImplementations(Type service);

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

    /// <summary>
    /// Destroy the internal container and all registered objects
    /// </summary>
    void Destroy();
}