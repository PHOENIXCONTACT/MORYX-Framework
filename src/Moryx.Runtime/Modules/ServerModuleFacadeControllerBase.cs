// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Container;

namespace Moryx.Runtime.Modules
{
    /// <summary>
    /// Base class for the facade contoller.
    /// </summary>
    /// <typeparam name="TConf">Configuration type of the server module.</typeparam>
    public abstract class ServerModuleFacadeControllerBase<TConf> : ServerModuleBase<TConf>
        where TConf : class, IConfig, new()
    {
        /// <summary>
        /// All facades that were activated
        /// </summary>
        private readonly ICollection<IFacadeControl> _activeFacades = new List<IFacadeControl>();

        /// <summary>
        /// Activate our public API facade
        /// </summary>
        protected void ActivateFacade(IFacadeControl facade)
        {
            // First activation
            if (facade.ValidateHealthState == null)
                facade.ValidateHealthState = ValidateHealthState;

            FillProperties(facade, FillProperty);
            facade.Activate();

            _activeFacades.Add(facade);
        }

        /// <summary>
        /// Deactivate our public facade
        /// </summary>
        protected void DeactivateFacade(IFacadeControl facade)
        {
            if (!_activeFacades.Contains(facade))
                return;

            facade.Deactivate();
            FillProperties(facade, (a, b) => null);

            _activeFacades.Remove(facade);

            var lifeCycleBoundFacade = facade as ILifeCycleBoundFacade;
            lifeCycleBoundFacade?.Deactivated();
        }


        /// <inheritdoc />
        protected internal sealed override void OnStarted()
        {
            base.OnStarted();

            foreach (var facade in _activeFacades.OfType<ILifeCycleBoundFacade>())
            {
                facade.Activated();
            }
        }

        private void FillProperties(object instance, Func<IContainer, PropertyInfo, object> fillingFunc)
        {
            // Fill everythin available in the container
            foreach (var prop in instance.GetType().GetProperties())
            {
                var type = prop.PropertyType;
                type = typeof(Array).IsAssignableFrom(type) ? type.GetElementType() : type;
                var implementations = Container.GetRegisteredImplementations(type);
                if (!implementations.Any())
                    continue;

                if (prop.SetMethod == null)
                    continue;

                prop.SetValue(instance, fillingFunc(Container, prop));
            }
        }

        private object FillProperty(IContainer container, PropertyInfo property)
        {
            var propType = property.PropertyType;
            if (typeof(Array).IsAssignableFrom(propType))
                return container.ResolveAll(propType.GetElementType());

            var strategyName = StrategyName(propType);
            return strategyName == null ? Container.Resolve(propType)
                                        : Container.Resolve(propType, strategyName);
        }

        private string StrategyName(Type dependencyType)
        {
            var config = ((IContainerHost)this).Strategies;
            return config.ContainsKey(dependencyType) ? config[dependencyType] : null;
        }
    }
}
