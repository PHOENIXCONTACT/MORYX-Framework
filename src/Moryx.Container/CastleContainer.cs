// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
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
        /// <summary>
        /// Internal windsor container doing the real DI
        /// </summary>
        protected IWindsorContainer Container { get; private set; }

        /// <summary>
        /// Registrator used to evaluate attributes
        /// </summary>
        protected IComponentRegistrator Registrator { get; }

        #region Constructors

        /// <summary>
        /// Create new container instance with default registrator
        /// </summary>
        public CastleContainer()
            : this(new ComponentRegistrator())
        {
        }

        /// <summary>
        /// Constructor to modify the applied registrator
        /// </summary>
        /// <param name="registrator">Registrator replacement</param>
        internal CastleContainer(ComponentRegistrator registrator)
        {
            // Boot up the container and give it to the registrator
            Container = registrator.Container = new WindsorContainer();
            Registrator = registrator;

            Container.AddFacility<TypedFactoryFacility>();
            Container.AddFacility<MoryxFacility>();

            // Self registration for framework functionality
            SetInstance<IContainer>(this);
        }

        #endregion

        /// <see cref="IContainer"/>
        public virtual void Destroy()
        {
            Container.Dispose();
            Container = null;
        }

        /// <summary>
        /// Execute the installer for this assembly
        /// </summary>
        /// <param name="installer"></param>
        public IContainer ExecuteInstaller(IContainerInstaller installer)
        {
            installer.Install(Registrator);

            return this;
        }

        /// <summary>
        /// Resolve an instance of the given service
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Instance of type</returns>
        public virtual T Resolve<T>()
        {
            return Container.Kernel.HasComponent(typeof(T)) ? Container.Resolve<T>() : default(T);
        }

        /// <summary>
        /// Resolve this dependency
        /// </summary>
        public virtual object Resolve(Type service)
        {
            return Container.Kernel.HasComponent(service) ? Container.Resolve(service) : null;
        }

        /// <summary>
        /// Resolve a named instance of the given service
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Instance of type</returns>
        public T Resolve<T>(string name)
        {
            return Container.Kernel.HasComponent(name) ? Container.Resolve<T>(name) : default(T);
        }

        /// <summary>
        /// Resolve this named dependency
        /// </summary>
        public object Resolve(Type service, string name)
        {
            return Container.Kernel.HasComponent(name) ? Container.Resolve(name, service) : null;
        }

        /// <summary>
        /// Resolve all implementations of this contract
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns></returns>
        public T[] ResolveAll<T>()
        {
            return Container.ResolveAll<T>();
        }

        /// <summary>
        /// Resolve all implementations of this contract
        /// </summary>
        public Array ResolveAll(Type service)
        {
            return Container.ResolveAll(service);
        }

        /// <summary>
        /// Get all implementations for a given component interface
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> GetRegisteredImplementations(Type componentInterface)
        {
            return Container.Kernel.GetHandlers(componentInterface).Select(handler => handler.ComponentModel.Implementation);
        }

        #region LoadComponents
        private static Type[] _knownTypes;

        /// <summary>
        /// Load all implementations of type from currently known types
        /// KnownTypes: Types in default framework folders and deeper.
        /// </summary>
        public void LoadComponents<T>() where T : class
        {
            LoadComponents<T>(null);
        }

        /// <summary>
        /// Loads all implementations of type from the currently known types
        /// KnownTypes: Types in default framework folders and deeper.
        /// </summary>
        public virtual void LoadComponents<T>(Predicate<Type> condition) where T : class
        {
            if (_knownTypes == null)
            {
                _knownTypes = ReflectionTool.GetAssemblies()
                    .Where(a => a.GetCustomAttribute<ComponentLoaderIgnoreAttribute>() == null)
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.GetCustomAttribute<RegistrationAttribute>(true) != null).ToArray();
            }

            foreach (var type in _knownTypes.Where(type => typeof(T).IsAssignableFrom(type)))
            {
                if(Registrator.ShallInstall(type) && (condition?.Invoke(type) ?? true))
                    Registrator.Register(type);
            }
        }

        #endregion

        #region Register methods

        ///
        public IContainer Register<TService, TComp>()
            where TService : class
            where TComp : TService
        {
            Registrator.Register(typeof(TComp), new[] { typeof(TService) });
            return this;
        }

        ///
        public IContainer Register<TService, TComp>(string name, LifeCycle lifeCycle)
            where TService : class
            where TComp : TService
        {
            Registrator.Register(typeof(TComp), new[] { typeof(TService) }, name, lifeCycle);
            return this;
        }

        ///
        public IContainer Register<TFactory>() where TFactory : class
        {
            Registrator.RegisterFactory(typeof(TFactory));
            return this;
        }

        ///
        public IContainer Register<TFactory>(string name) where TFactory : class
        {
            var factoryType = typeof(TFactory);
            var att = typeof(TFactory).GetCustomAttribute<FactoryRegistrationAttribute>();
            Registrator.RegisterFactory(factoryType, name, att?.Selector);
            return this;
        }

        #endregion

        #region SetInstances

        /// <summary>
        /// Set instance of service
        /// </summary>
        /// <typeparam name="T">Type of service</typeparam>
        /// <param name="instance">Instance implementing the service</param>
        public IContainer SetInstance<T>(T instance) where T : class
        {
            if (instance != null)
            {
                Container.Register(Component.For<T>().Instance(instance));
            }
            return this;
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
                Container.Register(Component.For<T>().Instance(instance).Named(name));
            }
            return this;
        }

        public void Extend<TExtension>() where TExtension : new()
        {
            if (!typeof(IFacility).IsAssignableFrom(typeof(TExtension)))
                throw new InvalidOperationException();

            var facility = (IFacility)new TExtension();
            Container.AddFacility(facility);
        }

        #endregion
    }
}
