// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moryx.Container
{
    /// <summary>
    /// Service collection based implementation of the MORYX <see cref="IContainer"/> interface
    /// </summary>
    public class ServiceCollectionContainer : ServiceCollection, IContainer
    {
        /// <summary>
        /// Collection of registered services
        /// </summary>
        public IServiceCollection Services { get; private set; }

        /// <summary>
        /// Service provider used for resolution
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Strategies extracted from the configuration
        /// </summary>
        public IDictionary<Type, string> Strategies { get; private set; }

        public ServiceCollectionContainer(IDictionary<Type, string> strategies) : this(strategies, new ServiceCollection())
        {            
        }

        public ServiceCollectionContainer(IDictionary<Type, string> strategies, IServiceCollection services)
        {
            Strategies = strategies;
            Services = services;
        }

        /// <summary>
        /// Complete registration and build the provider
        /// </summary>
        public void BuildProvider()
        {
            ServiceProvider = Services.BuildServiceProvider();
        }


        public IEnumerable<Type> GetRegisteredImplementations(Type componentInterface)
        {
            throw new NotImplementedException();
        }

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public void LoadComponents<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public void LoadComponents<T>(Predicate<Type> condition) where T : class
        {
            throw new NotImplementedException();
        }

        public IContainer Register<TService, TComp>()
            where TService : class
            where TComp : TService
        {
            throw new NotImplementedException();
        }

        public IContainer Register<TService, TComp>(string name, LifeCycle lifeCycle)
            where TService : class
            where TComp : TService
        {
            throw new NotImplementedException();
        }

        public IContainer Register<TFactory>() where TFactory : class
        {
            throw new NotImplementedException();
        }

        public IContainer Register<TFactory>(string name) where TFactory : class
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>()
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type service)
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>(string name)
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type service, string name)
        {
            throw new NotImplementedException();
        }

        public T[] ResolveAll<T>()
        {
            throw new NotImplementedException();
        }

        public Array ResolveAll(Type service)
        {
            throw new NotImplementedException();
        }

        public IContainer SetInstance<T>(T instance) where T : class
        {
            throw new NotImplementedException();
        }

        public IContainer SetInstance<T>(T instance, string name) where T : class
        {
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
