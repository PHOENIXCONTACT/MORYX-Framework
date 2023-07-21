// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Moryx.Container
{
    internal class ComponentRegistrator : IComponentRegistrator
    {
        private readonly IDictionary<Type, string> _strategies;

        /// <summary>
        /// Internal windsor container for registration
        /// </summary>
        protected internal IWindsorContainer Container { get; set; }

        public ComponentRegistrator(IDictionary<Type, string> strategies)
        {
            _strategies = strategies;
        }

        /// <summary>
        /// Method to determine if this component shall be installed
        /// </summary>
        public virtual bool ShallInstall(Type foundType)
        {
            var regAtt = foundType.GetCustomAttribute<ComponentAttribute>();
            var facAtt = foundType.GetCustomAttribute<PluginFactoryAttribute>();

            return (regAtt != null || facAtt != null) && NotRegisteredYet(foundType, regAtt);
        }

        /// <summary>
        /// Check if the type was not registered yet
        /// </summary>
        /// <param name="foundType">Type that must be checked for suitability to register</param>
        /// <param name="regAtt"></param>
        /// <returns>True if the component was not registered before</returns>
        protected bool NotRegisteredYet(Type foundType, ComponentAttribute regAtt)
        {
            var name = string.IsNullOrEmpty(regAtt?.Name) ? foundType.FullName : regAtt.Name;
            return !Container.Kernel.HasComponent(name);
        }

        public void Register(Type type)
        {
            if (type.IsInterface)
            {
                RegisterFactory(type);
            }
            else
            {
                Register(type, GetComponentServices(type));
            }
        }

        /// <summary>
        /// Get all services of this component
        /// </summary>
        public static Type[] GetComponentServices(Type type)
        {
            var att = type.GetCustomAttribute<ComponentAttribute>();
            if (att != null)
                return att.Services.Any() ? att.Services : new[] { type };

            var interfaces = type.GetInterfaces();
            return interfaces.Any() ? interfaces : new[] { type };
        }

        public void Register(Type type, Type[] services)
        {
            var regAtt = type.GetCustomAttribute<ComponentAttribute>();
            Register(type, services, regAtt?.Name, regAtt?.LifeStyle ?? LifeCycle.Singleton);
        }


        public void Register(Type type, Type[] services, string name)
        {
            Register(type, services, name, LifeCycle.Singleton);
        }


        public void Register(Type type, Type[] services, string name, LifeCycle lifeCycle)
        {
            var registration = BuildRegistration(type, services, name, lifeCycle);

            Container.Register(registration);
        }

        /// <summary>
        /// Build registration from arguments
        /// </summary>
        protected virtual ComponentRegistration<object> BuildRegistration(Type type, Type[] services, string name, LifeCycle lifeCycle)
        {
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

            return registration;
        }

        /// <summary>
        /// Determine a possible override for this member. Base implementatin checks for named attribute
        /// </summary>
        protected virtual ServiceOverride OverrideDependency(string dependencyName, Type dependencyType, ICustomAttributeProvider attributeProvider)
        {
            var atts = attributeProvider.GetCustomAttributes(typeof(NamedAttribute), false);
            var dependency = atts.Any() ? Dependency.OnComponent(dependencyName, ((NamedAttribute)atts[0]).ComponentName) : null;

            if (dependency == null && _strategies.ContainsKey(dependencyType))
                dependency = Dependency.OnComponent(dependencyName, _strategies[dependencyType]);

            return dependency;
        }


        public void RegisterFactory(Type factoryInterface)
        {
            var facAtt = factoryInterface.GetCustomAttribute<PluginFactoryAttribute>();
            RegisterFactory(factoryInterface, null, facAtt?.Selector);
        }


        public void RegisterFactory(Type factoryInterface, string name)
        {
            RegisterFactory(factoryInterface, name, null);
        }

        public virtual void RegisterFactory(Type factoryInterface, string name, Type selector)
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

            Container.Register(registration);
        }
    }
}
