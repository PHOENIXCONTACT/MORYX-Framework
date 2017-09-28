using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;

namespace Marvin.Container
{
    /// <summary>
    /// Module internal castle container
    /// </summary>
    public class LocalContainer : CastleContainer
    {
        private readonly IDictionary<Type, string> _strategies;

        /// <summary>
        /// Default constructor without strategies
        /// </summary>
        public LocalContainer() : this(new Dictionary<Type, string>())
        {
        }

        /// <summary>
        /// Constructor for the local castle container.
        /// </summary>
        /// <param name="strategies">Configuration for the container.</param>
        public LocalContainer(IDictionary<Type, string> strategies)
            : base(new LocalRegistrator(strategies))
        {
            _strategies = strategies;
        }

        /// <inheritdoc />
        public override T Resolve<T>()
        {
            var service = typeof(T);
            return _strategies.ContainsKey(service) ? Resolve<T>(_strategies[service]) : base.Resolve<T>();
        }

        /// <inheritdoc />
        public override object Resolve(Type service)
        {
            return _strategies.ContainsKey(service) ? Resolve(service, _strategies[service]) : base.Resolve(service);
        }

        /// <seealso cref="IContainer"/>
        public override void LoadComponents<T>(Predicate<Type> condition)
        {
            // Save all current handlers
            var handlers = Container.Kernel.GetHandlers(typeof(T)).Select(handler => handler.ComponentModel.Implementation);

            // Invoke registration
            base.LoadComponents<T>(condition);

            // Check registered types for additional registrations
            RegisterAdditionalDependencies(Container.Kernel.GetHandlers(typeof(T))
                                                    .Select(handler => handler.ComponentModel.Implementation)
                                                    .Except(handlers));
        }

        private void RegisterAdditionalDependencies(IEnumerable<Type> modulePlugins)
        {
            foreach (var implementation in modulePlugins)
            {
                var att = implementation.GetCustomAttribute<DependencyRegistrationAttribute>();
                if (att == null)
                    continue;

                var installer = att.InstallerMode == InstallerMode.All ? new AutoInstaller(implementation.Assembly)
                                                                       : new DependencyAutoInstaller(implementation.Assembly, att);
                ExecuteInstaller(installer);
                if (att.Initializer == null)
                    continue;

                if (!typeof(ISubInitializer).IsAssignableFrom(att.Initializer))
                    throw new InvalidCastException($"SubInitializer {att.Initializer.Name} of component {implementation.Name} does not implement interface ISubInitializer");

                // If someone registered this sub initializer before just skip
                if (!Container.Kernel.HasComponent(att.Initializer))
                    Container.Register(Component.For(typeof(ISubInitializer), att.Initializer).ImplementedBy(att.Initializer));
            }
        }
    }
}