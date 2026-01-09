// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace Moryx.Container;

/// <summary>
/// Extensions on the reduced <see cref="IContainer"/> interface
/// </summary>
public static class ContainerRegistrationExtensions
{
    extension(IContainer container)
    {
        /// <summary>
        /// Register external type in local container
        /// </summary>
        public IContainer Register<TService, TComp>()
            where TComp : TService
            where TService : class
        {
            var type = typeof(TComp);
            var regAtt = type.GetCustomAttribute<ComponentAttribute>();
            container.Register(type, [typeof(TService)], regAtt?.Name, regAtt?.LifeStyle ?? LifeCycle.Singleton);

            return container;
        }

        /// <summary>
        /// Register external type in local container
        /// </summary>
        public IContainer Register<TService, TComp>(string name, LifeCycle lifeCycle)
            where TComp : TService
            where TService : class
        {
            container.Register(typeof(TComp), [typeof(TService)], name, lifeCycle);

            return container;
        }

        /// <summary>
        /// Register type and determine factory and services automatically
        /// </summary>
        public IContainer Register(Type type)
        {
            if (type.IsInterface)
            {
                container.RegisterFactory(type);
            }
            else
            {
                var services = GetComponentServices(type);
                container.Register(type, services);
            }

            return container;
        }
    }

    /// <summary>
    /// Get all services of this component
    /// </summary>
    public static Type[] GetComponentServices(Type type)
    {
        var att = type.GetCustomAttribute<ComponentAttribute>();
        if (att != null)
            return att.Services.Any() ? att.Services : [type];

        var interfaces = type.GetInterfaces();
        return interfaces.Any() ? interfaces : [type];
    }

    /// <param name="container">Container to register in</param>
    extension(IContainer container)
    {
        /// <summary>
        /// Register a type for different services
        /// </summary>
        public IContainer Register(Type type, Type[] services)
        {
            var regAtt = type.GetCustomAttribute<ComponentAttribute>();
            container.Register(type, services, regAtt?.Name, regAtt?.LifeStyle ?? LifeCycle.Singleton);

            return container;
        }

        /// <summary>
        /// Register named component for the services
        /// </summary>
        public IContainer Register(Type type, Type[] services, string name)
        {
            container.Register(type, services, name, LifeCycle.Singleton);

            return container;
        }

        /// <summary>
        /// Register a factory by generic interface
        /// </summary>
        public IContainer Register<TFactory>() where TFactory : class
        {
            var att = typeof(TFactory).GetCustomAttribute<PluginFactoryAttribute>();
            container.RegisterFactory(typeof(TFactory), null, att?.Selector);
            return container;
        }

        /// <summary>
        /// Register a named factory
        /// </summary>
        public IContainer Register<TFactory>(string name) where TFactory : class
        {
            var factoryType = typeof(TFactory);
            var att = typeof(TFactory).GetCustomAttribute<PluginFactoryAttribute>();
            container.RegisterFactory(factoryType, name, att?.Selector);

            return container;
        }

        /// <summary>
        /// Register factory by type
        /// </summary>
        public IContainer RegisterFactory(Type factoryInterface)
        {
            var facAtt = factoryInterface.GetCustomAttribute<PluginFactoryAttribute>();
            container.RegisterFactory(factoryInterface, null, facAtt?.Selector);

            return container;
        }

        /// <summary>
        /// Register named factory by type
        /// </summary>
        public IContainer RegisterFactory(Type factoryInterface, string name)
        {
            container.RegisterFactory(factoryInterface, name, null);

            return container;
        }

        /// <summary>
        /// Set instance of service
        /// </summary>
        /// <typeparam name="T">Type of service</typeparam>
        /// <param name="instance">Instance implementing the service</param>
        public IContainer SetInstance<T>(T instance) where T : class
        {
            if (instance != null)
            {
                container.RegisterInstance([typeof(T)], instance, null);
            }
            return container;
        }

        /// <summary>
        /// Set globally imported instance with name
        /// </summary>
        /// <typeparam name="T">Type of service</typeparam>
        /// <param name="instance">Instance to register</param>
        /// <param name="name">Name of instance</param>
        public IContainer SetInstance<T>(T instance, string name) where T : class
        {
            if (instance != null)
            {
                container.RegisterInstance([typeof(T)], instance, name);
            }
            return container;
        }
    }
}
