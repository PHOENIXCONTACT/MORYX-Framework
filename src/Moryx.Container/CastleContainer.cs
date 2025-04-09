// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

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
            _container.AddFacility<MoryxFacility>(mf => mf.AddStrategies(strategies));

            // Self registration for framework functionality
            RegisterInstance([typeof(IContainer)], this, null);
        }

        #endregion

        /// <summary>
        /// Resolve this named dependency
        /// </summary>
        public object Resolve(Type service, string name)
        {
            if(name == null && _strategies.ContainsKey(service))
                name = _strategies[service];
            
            // Resolve by name if given or determined
            if (name != null && _container.Kernel.HasComponent(name))
                return _container.Resolve(name, service);

            // Resolve by type if found
            if (_container.Kernel.HasComponent(service))
                return _container.Resolve(service);

            // Otherwise return null
            return null;
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
        public IEnumerable<Type> GetRegisteredImplementations(Type service)
        {
            return _container.Kernel.GetHandlers(service).Select(handler => handler.ComponentModel.Implementation);
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

            _container.Register(registration);
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

        /// <see cref="IContainer"/>
        public virtual void Destroy()
        {
            _container.Dispose();
        }
    }
}
