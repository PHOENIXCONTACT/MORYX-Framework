// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Moryx.Tools;

namespace Moryx.Container
{
    /// <summary>
    /// Class to load component specific windsor Container
    /// </summary>
    public class CastleContainer : IContainer
    {

        #region Constructors

        private readonly IWindsorContainer _container;

        private readonly IDictionary<Type, string> _strategies;

        /// <summary>
        /// Create instance without strategies
        /// </summary>
        public CastleContainer()
            : this(new Dictionary<Type, string>())
        {
        }
        /// <summary>
        /// Create container with strategies
        /// </summary>
        /// <param name="strategies"></param>
        public CastleContainer(IDictionary<Type, string> strategies)
        {
            _strategies = strategies;

            // Boot up the container 
            _container = new WindsorContainer();

            _container.AddFacility<TypedFactoryFacility>();
            _container.AddFacility<MoryxFacility>();

            // Self registration for framework functionality
            RegisterInstance(new[] { typeof(IContainer) }, this, null);
        }

        #endregion

        /// <see cref="IContainer"/>
        public virtual void Destroy()
        {
            _container.Dispose();
        }

        /// <summary>
        /// Resolve an instance of the given service
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Instance of type</returns>
        public virtual T Resolve<T>()
        {
            var service = typeof(T);
            return _strategies.ContainsKey(service)
                ? Resolve<T>(_strategies[service])
                : _container.Kernel.HasComponent(typeof(T)) ? _container.Resolve<T>() : default(T);
        }

        /// <summary>
        /// Resolve this dependency
        /// </summary>
        public virtual object Resolve(Type service)
        {
            return _strategies.ContainsKey(service)
                ? Resolve(service, _strategies[service])
                : _container.Kernel.HasComponent(service) ? _container.Resolve(service) : null;
        }

        /// <summary>
        /// Resolve a named instance of the given service
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Instance of type</returns>
        public T Resolve<T>(string name)
        {
            return _container.Kernel.HasComponent(name) ? _container.Resolve<T>(name) : default(T);
        }

        /// <summary>
        /// Resolve this named dependency
        /// </summary>
        public object Resolve(Type service, string name)
        {
            return _container.Kernel.HasComponent(name) ? _container.Resolve(name, service) : null;
        }

        /// <summary>
        /// Resolve all implementations of this contract
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns></returns>
        public T[] ResolveAll<T>()
        {
            return _container.ResolveAll<T>();
        }

        /// <summary>
        /// Resolve all implementations of this contract
        /// </summary>
        public Array ResolveAll(Type service)
        {
            return _container.ResolveAll(service);
        }

        /// <summary>
        /// Get all implementations for a given component interface
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> GetRegisteredImplementations(Type componentInterface)
        {
            return _container.Kernel.GetHandlers(componentInterface).Select(handler => handler.ComponentModel.Implementation);
        }

        #region Register methods

        /// <summary>
        /// Register a component in the container
        /// </summary>
        public void Register(Type type, Type[] services, string name, LifeCycle lifeCycle)
        {
            // Make sure component is not registered yet
            var componentName = name ?? type.FullName;
            if (_container.Kernel.HasComponent(componentName))
                return;

            var registration = Component.For(services).ImplementedBy(type);

            // Register name
            if (!string.IsNullOrEmpty(name))
            {
                registration.Named(name);
            }

            // Register life style
            switch (lifeCycle)
            {
                case LifeCycle.Transient:
                    registration.LifestyleTransient();
                    break;
                case LifeCycle.Singleton:
                    registration.LifestyleSingleton();
                    break;
            }

            // Optionally override property injection
            foreach (var property in registration.Implementation.GetProperties())
            {
                // Check if this property has an override
                var dependency = OverrideDependency(property.Name, property.PropertyType, property);

                // Override property injection for this property if found
                if (dependency != null)
                    registration.DependsOn(dependency);
            }

            // Override Constructor injection as well
            foreach (var constructorParameter in registration.Implementation.GetConstructors().SelectMany(constructor => constructor.GetParameters()))
            {
                // Check if this paramter has an override
                var dependency = OverrideDependency(constructorParameter.Name, constructorParameter.ParameterType, constructorParameter);

                // Override constructor injection for this property if found
                if (dependency != null)
                    registration.DependsOn(dependency);
            }

            _container.Register(registration);
        }

        /// <summary>
        /// Determine a possible override for this member. Base implementatin checks for named attribute
        /// </summary>
        private ServiceOverride OverrideDependency(string dependencyName, Type dependencyType, ICustomAttributeProvider attributeProvider)
        {
            var atts = attributeProvider.GetCustomAttributes(typeof(NamedAttribute), false);
            var dependency = atts.Any() ? Dependency.OnComponent(dependencyName, ((NamedAttribute)atts[0]).ComponentName) : null;

            if (dependency == null && _strategies.ContainsKey(dependencyType))
                dependency = Dependency.OnComponent(dependencyName, _strategies[dependencyType]);

            return dependency;
        }

        /// <summary>
        /// Register factory interface
        /// </summary>
        public void RegisterFactory(Type factoryInterface, string name, Type selector)
        {
            var registration = Component.For(factoryInterface);

            if (!string.IsNullOrEmpty(name))
            {
                registration.Named(name);
            }

            if (selector == null)
            {
                registration.AsFactory();
            }
            else
            {
                // Make sure selector is registered in the container
                // TODO: Super dirty hack to use interfaces in component selectors
                var selectorName = selector.IsClass ? selector.FullName : $"{selector.Namespace}.{selector.Name.Substring(1)}";
                registration.AsFactory(config => config.SelectedWith(selectorName));
            }

            _container.Register(registration);
        }

        /// <summary>
        /// Register instance in the container
        /// </summary>
        public void RegisterInstance(Type[] services, object instance, string name)
        {
            var registration = Component.For(services).Instance(instance);

            if (!string.IsNullOrEmpty(name))
                registration.Named(name);

            _container.Register(registration);
        }

        #endregion

        #region SetInstances

        public void Extend<TExtension>() where TExtension : new()
        {
            var facilityType = typeof(IFacility);
            if (!facilityType.IsAssignableFrom(typeof(TExtension)))
                throw new InvalidOperationException("The underlying container only supports " + facilityType.FullName + "!");

            var facility = (IFacility)new TExtension();
            _container.AddFacility(facility);
        }

        #endregion
    }
}
